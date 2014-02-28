
#region References

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ColloSys.AllocationService.ConditionLayer;
using ColloSys.AllocationService.DBLayer;
using ColloSys.DataLayer.Allocation;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.SharedDomain;
using ColloSys.Shared.ConfigSectionReader;
using ColloSys.Shared.Types4Product;
using Iesi.Collections.Generic;
using NHibernate;
using ColloSys.AllocationService.Logging;
using NHibernate.SqlCommand;
using NLog;

#endregion


namespace ColloSys.AllocationService.AllocationLayer
{
    public static class Allocation
    {
        #region Memebers

        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        #endregion

        #region Initialise Database connection and logger

        static Allocation()
        {

            //Session = SessionManager.GetCurrentSession();
            NLogConfig.InitConFig(ColloSysParam.WebParams.LogPath, ColloSysParam.WebParams.LogLevel);
        }

        #endregion

        #region allocation condition

        public static IEnumerable<Entity> StartAllocationProcessV2(ScbEnums.Products product, ScbEnums.Category category)
        {
            Log.Info(string.Format("allocation process strarted with product {0} and category {1}", product, category));

            var allocationlist = new List<Entity>();

            //get class type for generate criteria
            Type getType = ClassType.GetTypeByProductCategoryForAlloc(product, category);
            Log.Info("Allocation for class name: " + getType.Name);

            //get policy for product 
            var policy = DbLayer.GetAllocationPolicy(product, category);
            if (policy == null)
            {
                Log.Info("No policy for " + product + " and " + category);
                return null;
            }
            Log.Debug("Allocation policy loaded");

            //get list of subpolicies
            var subpolicyList = policy.AllocRelations
                                      .OrderBy(r => r.Priority)
                                      .Select(r => r.AllocSubpolicy)
                                      .ToList();
            foreach (var subpolicy in subpolicyList)
            {
                subpolicy.AllocRelations = new HashedSet<AllocRelation>(policy.AllocRelations.Where(x => x.AllocSubpolicy.Id == subpolicy.Id).ToList());
            }
            Log.Info("Conditions count under Policy.SubPolicy: " + subpolicyList.Select(x => x.Conditions.Count));

            foreach (var subpolicy in subpolicyList)
            {
                //get data on created criteria
                IList dataOnCondition;
                //create criteria 
                using (var session = SessionManager.GetNewSession())
                {
                    using (var trans = session.BeginTransaction())
                    {
                        ICriteria criteria = session.CreateCriteria(getType, getType.Name);

                        //get list of conditions
                        var conditionList = subpolicy.Conditions;

                        //get default criteria
                        criteria = Conditions.GetDefaultCriteria(criteria, product);
                        Log.Debug("Default Criteria Created:" + criteria);

                        //create criteria for all conditions for single subpolicy
                        criteria = CreateCriteriaOnCondition(conditionList, getType, criteria);
                        Log.Info("Criteria on Condition:" + criteria);

                        dataOnCondition = criteria.List();
                        trans.Rollback();
                    }
                }
                Log.Info("Total no a/c for allocation: " + dataOnCondition.Count);

                if (dataOnCondition.Count > 0)
                {
                    switch (subpolicy.AllocateType)
                    {
                        case ColloSysEnums.AllocationType.DoNotAllocate:
                            allocationlist = DoNotAllocate.SetDoNotAllocateAc(getType, dataOnCondition,
                                                                         subpolicy.AllocRelations.First(),
                                                                         product);
                            break;
                        case ColloSysEnums.AllocationType.AllocateAsPerPolicy:
                            allocationlist = AllocateAsPerPolicy.Init(getType, dataOnCondition
                                                                      , subpolicy.AllocRelations.First(), product);
                            break;

                        case ColloSysEnums.AllocationType.AllocateToStkholder:
                            allocationlist = AllocateToStakeholder.Init(getType, dataOnCondition,
                                                                        subpolicy.AllocRelations.First(),
                                                                         product);
                            break;
                        case ColloSysEnums.AllocationType.HandleByTelecaller:
                            //allocationlist = HandleByTelecaller.Init(getType, dataOnCondition,
                            //                                         subpolicy.AllocRelations.First(), product);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    //set approve status, approvedate, approved by for allocation 
                    allocationlist.ForEach(x => { ((Alloc) x).Status = ColloSysEnums.ApproveStatus.Approved;
                                                    ((Alloc) x).ApprovedBy = "Policy";
                                                    ((Alloc) x).ApprovedOn = DateTime.Today.Date;
                    });

                    DbLayer.SaveObjectList(dataOnCondition);
                    DbLayer.SaveList(allocationlist);
                }
            }
            return allocationlist;
        }

        private static ICriteria CreateCriteriaOnCondition(IEnumerable<AllocCondition> conditionList, Type getType, ICriteria criteria)
        {
            foreach (var condition in conditionList)
            {
                var columnName = ConditionValues.ColumnName(condition);
                var value = ConditionValues.Value(condition);
                var operatorName = ConditionValues.Operator(condition);

                value = ConditionValues.ConvertValue(getType, value, columnName, criteria);

                criteria = Conditions.SetCriteria(criteria, columnName, operatorName, value);
            }
            return criteria;
        }

        #endregion
    }
}

//public static void GetConditionForCurrentMonth(string productName, string categoryName)
//       {

//           var getApprovedCondtions = GetApprovedCondtions();

//           var product = productName;
//           var category = categoryName;

//           var getConditionForProduct = GetConditionForProduct(getApprovedCondtions, product);

//           //var countConditionsOnProduct = getConditionForProduct.Count();
//           string columnName;
//           object value;
//           string operatorName;

//           var condition = GetConditionString(getConditionForProduct, out columnName, out value, out operatorName);

//           ExeCondition(getConditionForProduct, columnName, value, operatorName);
//       }

//       private static void ExeCondition(AllocRelation getConditionForProduct, string columnName, object value, string operatorName)
//       {
//           if (getConditionForProduct.AllocSubpolicy.DoAllocate)
//           {
//           }

//           if (!getConditionForProduct.AllocSubpolicy.DoAllocate)
//           {
//               var getdata = GetDataOnCondition<RWriteoff>(columnName, value, operatorName);

//               var baseDate = DateTime.Today;
//               var thisMonthStart = baseDate.AddDays(1 - baseDate.Day);
//               var thisMonthEnd = thisMonthStart.AddMonths(1).AddSeconds(-1);

//               var list = (from object data in getdata
//                           select new RAlloc
//                               {
//                                   IsAllocated = false,
//                                   IsReferred = false,
//                                   RWriteoff = (RWriteoff)data,
//                                   StartDate = thisMonthStart,
//                                   EndDate = thisMonthEnd,
//                                   AmountDue = 0,
//                                   Bucket = 2,
//                                   WithTelecalling = false
//                               }).ToList();
//           }
//       }

//       private static IList GetDataOnCondition<T>(string columnName, object value, string operatorName) where T : Entity
//       {
//           var criteria = _session.CreateCriteria<T>();
//           var colValue = ConvertValue<T>(value, columnName);

//           switch (operatorName)
//           {
//               case "GreaterThan":
//                   criteria.Add(Restrictions.Gt(columnName, colValue));
//                   break;

//               case "LessThan":
//                   criteria.Add(Restrictions.Lt(columnName, colValue));
//                   break;

//               case "GreaterThanEqualTo":
//                   criteria.Add(Restrictions.Ge(columnName, colValue));
//                   break;

//               case "LessThanEqualTo":
//                   criteria.Add(Restrictions.Le(columnName, colValue));
//                   break;

//               case "EqualTo":
//                   criteria.Add(Restrictions.Eq(columnName, colValue));
//                   break;

//               case "NotEqualTo":
//                   criteria.Add(!Restrictions.Eq(columnName, colValue));
//                   break;

//               case "Contains":
//                   criteria.Add(Restrictions.Like(columnName, colValue.ToString(), MatchMode.Exact));
//                   break;

//               case "Start With":
//                   criteria.Add(Restrictions.Like(columnName, colValue.ToString(), MatchMode.Start));
//                   break;

//               case "End With":
//                   criteria.Add(Restrictions.Like(columnName, colValue.ToString(), MatchMode.End));
//                   break;
//           }

//           //var getdata = _session.CreateCriteria<RWriteoff>()
//           //                      .Add(Restrictions.Gt(columnName, Convert.ToUInt32(value)))
//           //                      .List();
//           var getdata = criteria.List();
//           return getdata;
//       }

//       private static string GetConditionString(AllocRelation getConditionForProduct, out string columnName, out object value, out string operatorName)
//       {
//           columnName = getConditionForProduct.AllocSubpolicy.AllocSubPolicyCondition.ColumnName;
//           operatorName = getConditionForProduct.AllocSubpolicy.AllocSubPolicyCondition.Operator;
//           value = getConditionForProduct.AllocSubpolicy.AllocSubPolicyCondition.Value;
//           var condition = columnName + " " + operatorName + " " + value;
//           return condition;
//       }

//       private static AllocRelation GetConditionForProduct(IEnumerable<AllocRelation> getApprovedCondtions, string Product)
//       {

//           var getConditionForProduct = (from approvedCondtion in getApprovedCondtions
//                                         where approvedCondtion.AllocPolicy.Products == Product
//                                               && approvedCondtion.AllocPolicy.Category == EnumHelper.Category.WriteOff
//                                         select approvedCondtion).Single();
//           return getConditionForProduct;
//       }

//       private static IEnumerable<AllocRelation> GetApprovedCondtions()
//       {
//           var conditionsRelations = _session.QueryOver<AllocRelation>()
//                                             .Where(x => x.StartDate <= DateTime.Now && x.EndDate >= DateTime.Now)
//                                             .List();

//           var getApprovedCondtions = conditionsRelations.Where(c => c.AllocPolicy.Status == EnumHelper.ApproveStatus.Approved);
//           return getApprovedCondtions;
//       }

//       private static object ConvertValue<T>(object value, string columnName) where T : Entity
//       {
//           var propertyInfo = typeof(T).GetProperty(columnName);
//           var type = propertyInfo.PropertyType.FullName;

//           switch (type)
//           {
//               case "System.UInt16":
//                   value = Convert.ToUInt16(value);
//                   break;

//               case "System.UInt32":
//                   value = Convert.ToUInt32(value);
//                   break;

//               case "System.UInt64":
//                   value = Convert.ToUInt64(value);
//                   break;

//               case "System.Int16":
//                   value = Convert.ToInt16(value);
//                   break;

//               case "System.Int32":
//                   value = Convert.ToInt32(value);
//                   break;

//               case "System.Int64":
//                   value = Convert.ToInt64(value);
//                   break;

//               case "System.Decimal":
//                   value = Convert.ToDecimal(value);
//                   break;

//               case "System.Guid":
//                   value = Guid.Parse(value.ToString());
//                   break;

//               case "System.DateTime":
//                   value = Convert.ToDateTime(value);
//                   break;

//               case "System.Char":
//                   value = Convert.ToChar(value);
//                   break;

//               case "System.Byte":
//                   value = Convert.ToByte(value);
//                   break;

//               case "System.Boolean":
//                   value = Convert.ToBoolean(value);
//                   break;
//           }

//           return value;
//       }



//public static bool StartAllocationProcess(string productName, EnumHelper.Category categoryName)
//       {
//           //get class type for generate criteria
//           Type getType = ClassType.GetType(productName, categoryName);

//           //create criteria 
//           var criteria = _session.CreateCriteria(getType);

//           //get valid single condition for product and category
//           var condition = Conditions.Condition.GetSingleCondition(productName, categoryName);

//           //get valid multiple conditions for product and category
//           var conditionlist = Conditions.Condition.GetConditions(productName, categoryName);


//           //set values for colummnName, value and opertor
//           var columnName = ConditionValues.ColumnName(condition.AllocSubpolicy.Conditions.First());
//           var value = ConditionValues.Value(condition.AllocSubpolicy.Conditions.First());
//           var operatorName = ConditionValues.Operator(condition.AllocSubpolicy.Conditions.First());

//           //set value respective to column type
//           value = ConditionValues.ConvertValue(getType, value, columnName);

//           //set criteria upon the condition
//           criteria = Conditions.SetCriteria(criteria, columnName, operatorName, value);

//           //get data on created criteria
//           var dataOnCondition = criteria.List();

//           if (dataOnCondition.Count > 0)
//           {
//               if (!condition.AllocSubpolicy.DoAllocate)
//               {
//                   //get list of do not allocate a/c 
//                   var allocationlist = Allocate.DoNotAllocate(getType, dataOnCondition, condition);

//                   //save that list
//                   var session = SessionManager.GetCurrentSession();
//                   session.SaveOrUpdate(allocationlist);
//               }
//           }

//           return true;
//       }


//public static bool StartAllocationProcessMultiple(string productName, EnumHelper.Category categoryName)
//        {
//            //get class type for generate criteria
//            Type getType = ClassType.GetType(productName, categoryName);

//            //create criteria 
//            var criteria = _session.CreateCriteria(getType);

//            //get valid multiple condition for product and category
//            var subpolicyList = Conditions.Condition.GetConditions(productName, categoryName);

//            foreach (var subpolicy in subpolicyList)
//            {
//                //set values for colummnName, value and opertor
//                var columnName = ConditionValues.ColumnName(subpolicy.AllocSubpolicy.Conditions.First());
//                var value = ConditionValues.Value(subpolicy.AllocSubpolicy.Conditions.First());
//                var operatorName = ConditionValues.Operator(subpolicy.AllocSubpolicy.Conditions.First());
//                //set value respective to column type
//                value = ConditionValues.ConvertValue(getType, value, columnName);

//                //set criteria upon the condition
//                criteria = Conditions.SetCriteria(criteria, columnName, operatorName, value);
//            }


//            //get data on created criteria
//            var dataOnCondition = criteria.List();

//            if (dataOnCondition.Count > 0)
//            {
//                foreach (var condition in subpolicyList)
//                {
//                    if (!condition.AllocSubpolicy.DoAllocate)
//                    {
//                        //get list of do not allocate a/c 
//                        var allocationlist = Allocate.DoNotAllocate(getType, dataOnCondition, condition);

//                        //save that list
//                        var session = SessionManager.GetCurrentSession();
//                        session.SaveOrUpdate(allocationlist);
//                    }   
//                }

//            }

//            return true;
//        }
