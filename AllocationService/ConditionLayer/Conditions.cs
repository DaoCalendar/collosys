using System;
using ColloSys.AllocationService.Generic;
using ColloSys.DataLayer.Enumerations;
using NHibernate;
using NHibernate.Criterion;
using NLog;

namespace ColloSys.AllocationService.ConditionLayer
{
    public class Conditions 
    {
        private static Conditions _conditon;
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        #region ctrs

        static Conditions()
        {
            _conditon = new Conditions();
        }

        private Conditions()
        { 
        }

        public static Conditions Condition
        {
            get { return _conditon; }
        }

        #endregion

        #region methods

        public static ICriteria GetDefaultCriteria(ICriteria criteria, ScbEnums.Products product)
        {
            //TODO: add isAllocatedFalse
            return criteria.Add(Restrictions.Eq("Product", product))
                           .Add(
                               Restrictions.Not(
                                   Restrictions.And(
                                       Restrictions.Eq("AllocStatus", ColloSysEnums.AllocStatus.AllocationError),
                                       Restrictions.Eq("NoAllocResons", ColloSysEnums.NoAllocResons.MissingPincode))))
                           //.Add(Restrictions.Eq("AllocStatus", ColloSysEnums.AllocStatus.None))
                           .Add(Restrictions.Not(Restrictions.Eq("Flag",ColloSysEnums.DelqFlag.R)))
                           .Add(Restrictions.Or(Restrictions.IsNull("AllocEndDate"),
                                                Restrictions.Le("AllocEndDate", Util.GetTodayDate())))
                           .Add(Restrictions.Gt("Pincode", Convert.ToUInt32(0)));
        }

        /// <summary>
        /// Set Criteria on basis of columnName, operator and columnvalue
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="columnName"></param>
        /// <param name="operatorName"></param>
        /// <param name="colValue"></param>
        /// <returns></returns>
        public static ICriteria SetCriteria(ICriteria criteria, string columnName, 
            ColloSysEnums.Operators operatorName, object colValue)
        {
            switch (operatorName)
            {
                case ColloSysEnums.Operators.GreaterThan:
                    criteria.Add(Restrictions.Gt(columnName, colValue));
                    break;
                case ColloSysEnums.Operators.GreaterThanEqualTo:
                    criteria.Add(Restrictions.Ge(columnName, colValue));
                    break;
                case ColloSysEnums.Operators.LessThan:
                    criteria.Add(Restrictions.Lt(columnName, colValue));
                    break;
                case ColloSysEnums.Operators.LessThanEqualTo:
                    criteria.Add(Restrictions.Le(columnName, colValue));
                    break;
                case ColloSysEnums.Operators.NotEqualTo:
                    criteria.Add(Restrictions.Not(Restrictions.Eq(columnName, colValue)));
                    break;
                case ColloSysEnums.Operators.EqualTo:
                    criteria.Add(Restrictions.Eq(columnName, colValue));
                    break;
                case ColloSysEnums.Operators.StartsWith:
                    criteria.Add(Restrictions.Like(columnName, colValue.ToString(), MatchMode.Start));
                    break;
                case ColloSysEnums.Operators.EndsWith:
                    criteria.Add(Restrictions.Like(columnName, colValue.ToString(), MatchMode.End));
                    break;
                case ColloSysEnums.Operators.Like:
                    criteria.Add(Restrictions.Like(columnName, colValue.ToString(), MatchMode.Anywhere));
                    break;
                case ColloSysEnums.Operators.Contains:
                    criteria.Add(Restrictions.Like(columnName, colValue.ToString(), MatchMode.Exact));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(operatorName.ToString());
            }
            return criteria;
        }

        #endregion
    }
}
