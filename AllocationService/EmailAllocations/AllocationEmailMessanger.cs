using ColloSys.DataLayer.Allocation;

#region references

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.Services.Shared;
using ColloSys.DataLayer.SharedDomain;
using ColloSys.QueryBuilder.AllocationBuilder;
using ColloSys.QueryBuilder.StakeholderBuilder;
using ColloSys.Shared.ExcelWriter;
using ColloSys.Shared.Types4Product;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NLog;

#endregion

//stakeholders callls changed
namespace ColloSys.AllocationService.EmailAllocations
{
    public class AllocationEmailMessanger : IAllocationEmailMessanger
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private static readonly StakeQueryBuilder StakeQueryBuilder = new StakeQueryBuilder();
        private static readonly AllocBuilder AllocBuilder=new AllocBuilder();

        public IEnumerable<StakeholdersStat> GetStakeholderWithManger()
        {
            var stakeholerInitialData = StakeQueryBuilder
                .FilterBy(x => x.Status == ColloSysEnums.ApproveStatus.Approved).ToList();
           
            var listOfStakeholderAndMangers = (from d in stakeholerInitialData
                                               select new StakeholdersStat()
                                                   {
                                                       AllocatedStakeholder = d,
                                                       Manager = GetReportingManger(d.ReportingManager)
                                                   }).ToList();
            Log.Info(string.Format("Stakeholders For mailing process Loaded {0}", listOfStakeholderAndMangers.Count));
            return listOfStakeholderAndMangers;
        }

        public bool InitSendingMail(IEnumerable<StakeholdersStat> listOfStakeholdersAndMangers)
        {
            foreach (ScbEnums.Products products in Enum.GetValues(typeof(ScbEnums.Products)))
            {
                if (products == ScbEnums.Products.UNKNOWN)
                    continue;
                var allocData = GetAllocationData(products);

                if (allocData == null || !allocData.Any())
                    continue;

                SendEmail(listOfStakeholdersAndMangers, allocData, products);
            }
            return true;
        }

        private void SendEmail(IEnumerable<StakeholdersStat> listOfStakeholdersAndMangers, IEnumerable<Allocations> allocData, ScbEnums.Products products)
        {
            Log.Info("In SendMail");
            foreach (var allocated in listOfStakeholdersAndMangers)
            {
                var allocationList = (from d in allocData
                                      where d.Stakeholder.Id == allocated.AllocatedStakeholder.Id
                                      select d).ToList();
                if (!allocationList.Any())
                {
                    Log.Info(string.Format("No Allocations for {0}", products.ToString()));
                    continue;
                }
                var listOfAllocationStat = SetAllocationStat(allocationList);

                if (!listOfAllocationStat.Any())
                {
                    Log.Info(string.Format("No AllocationStat for {0}", products.ToString()));
                    continue;
                }

                var filename = string.Format("{0}_{1}.xlsx", allocated.AllocatedStakeholder.Name, DateTime.Now.ToString("yyyyMMdd_HHmmss"));

                Log.Info(string.Format("FileName: {0} for Product {1}", filename, products.ToString()));

                var fileInfo = new FileInfo(filename);
                var columnPositionInfo = GenerateColumnsForExcel();

                try
                {
                    ClientDataWriter.ListToExcel(listOfAllocationStat, fileInfo, columnPositionInfo);
                    Log.Info(string.Format("Data successfully write to excel for {0}", products.ToString()));
                }
                catch (Exception e)
                {
                    Log.Error(string.Format("Error occure in writing data to excel file for {0}" +
                                            "and exception is : {1}", products.ToString(), e.Message));

                    continue;
                }

                if (string.IsNullOrEmpty(allocated.Manager.EmailId))
                {
                    Log.Info(string.Format("Email id empty for {0} in processing product {1}",
                        allocated.AllocatedStakeholder.Name, products.ToString()));
                    continue;
                }


                var subjectForEmail = string.Format("[ColloSys] Allocations for {0}-{1}",
                                                    allocated.AllocatedStakeholder.Name, DateTime.Now.ToString("yyyyMMdd"));

                try
                {
                    EmailService.EmailReport(fileInfo, allocated.Manager.EmailId, subjectForEmail);
                    EmailService.EmailReport(fileInfo,allocated.AllocatedStakeholder.EmailId,subjectForEmail);
                }
                catch (Exception e)
                {
                    Log.Error(string.Format("Error occure in sending mail for {0}" +
                                            "and exception is : {1}", products.ToString(), e.Message));
                }
            }
        }

        private IList<ColumnPositionInfo> GenerateColumnsForExcel()
        {
            uint position = 0;
            var columnPositionInfo = new List<ColumnPositionInfo>
                {
                    new ColumnPositionInfo
                        {
                            DisplayName = "AllocationPolicy",
                            FieldName = "PolicyName",
                            Position = (++position),
                        },
                    new ColumnPositionInfo
                        {
                            DisplayName = "AllocationSubPolicy",
                            FieldName = "SubPolicyName",
                            Position = (++position),
                        },
                    new ColumnPositionInfo
                        {
                            DisplayName = "Stakeholder",
                            FieldName = "StakeholderName",
                            Position = (++position),
                        },
                    new ColumnPositionInfo
                        {
                            DisplayName = "Start Date",
                            FieldName = "StartDate",
                            Position = (++position),
                        },
                    new ColumnPositionInfo
                        {
                            DisplayName = "End Date",
                            FieldName = "EndDate",
                            Position = (++position),
                        },
                    new ColumnPositionInfo
                        {
                            DisplayName = "Product",
                            FieldName = "Product",
                            Position = (++position),
                        },
                    new ColumnPositionInfo
                        {
                            DisplayName = "Account No",
                            FieldName = "AccountNo",
                            Position = ++position,
                        },
                    new ColumnPositionInfo
                        {
                            DisplayName = "Customer Name",
                            FieldName = "CustomerName",
                            Position = ++position,
                        },
                    new ColumnPositionInfo
                        {
                            DisplayName = "Pincode",
                            FieldName = "Pincode",
                            Position = ++position,
                        },
                    new ColumnPositionInfo
                        {
                            DisplayName = "Total Due",
                            FieldName = "TotalDue",
                            Position = ++position,
                        },
                };
            return columnPositionInfo;
        }

        private IEnumerable<Allocations> GetAllocationData(ScbEnums.Products products)
        {
            var criteria = AllocBuilder.CriteriaForEmail();
            Log.Info(string.Format("Criteria for {0} is {1}", products.ToString(), criteria));
            //fetch data
            var data = criteria.List<Allocations>();

            Log.Info(string.Format("Allocation data loaded for {0} and count= {1}",
                products.ToString(), data.Count));

            return data;
        }

        //TODO:change call here
        private Stakeholders GetReportingManger(Guid reportingManager)
        {
            if (reportingManager == Guid.Empty)
            {
                return new Stakeholders();
            }
            return StakeQueryBuilder.FilterBy(x => x.Id == reportingManager).Single();
           
        }

        #region Set AllocationStat

        private IList<AllocationStat> SetAllocationStat(IEnumerable<Allocations> allocationList)
        {
            var allocationStats = ConvertForAllocInfo(allocationList);
            return allocationStats;
        }

        private List<AllocationStat> ConvertForAllocInfo(IEnumerable<Allocations> allocationList)
        {
            return allocationList.Select(alloc => new AllocationStat()
                {
                    AccountNo = alloc.Info.AccountNo,
                    PolicyName = alloc.AllocPolicy.Name,
                    SubPolicyName = alloc.AllocSubpolicy.Name,
                    StakeholderName = alloc.Stakeholder.Name,
                    StartDate = alloc.StartDate.ToString("yyyy-MM-dd"),
                    EndDate = alloc.EndDate.HasValue
                                  ? alloc.EndDate.ToString()
                                  : string.Empty,
                    Product = alloc.AllocPolicy.Products.ToString(),
                    TotalDue = alloc.Info.TotalDue.ToString(),
                    CustomerName = alloc.Info.CustomerName,
                    Pincode = alloc.Info.Pincode.ToString()
                }).ToList();
        }

        #endregion

    }
}


//private ICriteria CreateCriteria(Type className, ISession session)
//{
//    //create criteria
//    var criteria = session.CreateCriteria(className, "Alloc");

//    criteria.CreateAlias("Alloc.Info", "Info", JoinType.InnerJoin);
//    criteria.CreateAlias("Alloc.Stakeholder", "Stakeholder", JoinType.InnerJoin);
//    criteria.CreateAlias("Alloc.AllocPolicy", "AllocPolicy", JoinType.InnerJoin);
//    criteria.CreateAlias("Alloc.AllocSubpolicy", "AllocSubpolicy", JoinType.InnerJoin);
//    //add condition for createdon and alloc status
//    criteria.Add(Restrictions.Ge("CreatedOn", DateTime.Today));
//    criteria.Add(Restrictions.Le("CreatedOn", DateTime.Today.AddDays(1)));
//    criteria.Add(Restrictions.Or(
//        Restrictions.Eq("Info.AllocStatus", ColloSysEnums.AllocStatus.AsPerWorking),
//        Restrictions.Eq("Info.AllocStatus", ColloSysEnums.AllocStatus.AllocateToStakeholder)));
//    return criteria;
//}



//Log.Info(string.Format("Allocated Stake {0} and Mangers are {1}",
//    listOfStakeholderAndMangers.Count(x=>x.AllocatedStakeholder),
//    listOfStakeholderAndMangers.Count(x => x.Manager)));
//var listOfStakeholderModel = (from d in stakeholerInitialData
//                              select
//                                  new
//                                      {
//                                          AllocatedStakeholder = d,
//                                          Manager = GetReportingManger(d.ReportingManager)
//                                      }).ToList();
//private List<AllocationStat> ConvertForRAllocInfo(List<Alloc> allocationList)
//{
//    var list = new List<AllocationStat>();
//    foreach (var sharedAlloc in allocationList)
//    {
//        var alloc = (RAlloc)sharedAlloc;
//        var allocationStat = new AllocationStat()
//            {
//                AccountNo = alloc.RInfo.AccountNo,
//                PolicyName = alloc.AllocPolicy.Name,
//                SubPolicyName = alloc.AllocSubpolicy.Name,
//                StakeholderName = alloc.Stakeholder.Name,
//                StartDate = alloc.StartDate.ToString("yyyy-MM-dd"),
//                EndDate = alloc.EndDate.HasValue ? alloc.EndDate.ToString() : string.Empty,
//                Product = alloc.AllocPolicy.Products.ToString(),
//                TotalDue = alloc.RInfo.TotalDue.ToString(),
//                CustomerName = alloc.RInfo.CustomerName,
//                Pincode = alloc.RInfo.Pincode.ToString()
//            };
//        list.Add(allocationStat);
//    }
//    return list;
//}
//private List<AllocationStat> ConvertForEAllocInfo(List<Alloc> allocationList)
//{
//    var list = new List<AllocationStat>();
//    foreach (var sharedAlloc in allocationList)
//    {
//        var alloc = (EAlloc)sharedAlloc;
//        var allocationStat = new AllocationStat()
//            {
//                AccountNo = alloc.EInfo.AccountNo,
//                PolicyName = alloc.AllocPolicy.Name,
//                SubPolicyName = alloc.AllocSubpolicy.Name,
//                StakeholderName = alloc.Stakeholder.Name,
//                StartDate = alloc.StartDate.ToString("yyyy-MM-dd"),
//                EndDate = alloc.EndDate.HasValue ? alloc.EndDate.ToString() : string.Empty,
//                Product = alloc.AllocPolicy.Products.ToString(),
//                TotalDue = alloc.EInfo.TotalDue.ToString(),
//                CustomerName = alloc.EInfo.CustomerName,
//                Pincode = alloc.EInfo.Pincode.ToString()
//            };
//        list.Add(allocationStat);
//    }
//    return list;
//}
//var systemOnProduct = Util.GetSystemOnProduct(products);
//switch (systemOnProduct)
//{
//    case ScbEnums.ScbSystems.CCMS:

//        break;
//    case ScbEnums.ScbSystems.EBBS:
//        allocationStats = ConvertForEAllocInfo(allocationList);
//        break;
//    case ScbEnums.ScbSystems.RLS:
//        allocationStats = ConvertForRAllocInfo(allocationList);
//        break;
//    default:
//        throw new ArgumentOutOfRangeException();
//}