#region references

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AngularUI.Shared.apis;
using ColloSys.DataLayer.Allocation;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SharedDomain;
using ColloSys.QueryBuilder.AllocationBuilder;
using ColloSys.QueryBuilder.ClientDataBuilder;
using ColloSys.QueryBuilder.StakeholderBuilder;
using ColloSys.Shared.NgGrid;
using ColloSys.Shared.Types4Product;
using NHibernate.Criterion;
using NHibernate.SqlCommand;

#endregion


//stakeholders calls changed
namespace ColloSys.UserInterface.Areas.Allocation.apiController
{
    public class ApproveViewAllocationApiController : BaseApiController<Allocations>
    {
        private static readonly StakeQueryBuilder StakeQuery = new StakeQueryBuilder();
        private static readonly AllocBuilder AllocBuilder = new AllocBuilder();
        private static readonly InfoBuilder InfoBuilder = new InfoBuilder();

        public HttpResponseMessage GetScbSystems()
        {
            var systems = Enum.GetNames(typeof(ScbEnums.Products))
                              .Where(x => x != ScbEnums.Products.UNKNOWN.ToString());
            return Request.CreateResponse(HttpStatusCode.OK, systems);
        }

        [HttpPost]
        public HttpResponseMessage FetchPageData(ViewAllocationFilter viewAllocationFilter)
        {
            var data = GetAllocData(viewAllocationFilter);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpGet]
        public IEnumerable<Allocations> GetData()
        {
            var query = AllocBuilder.ApplyRelations();
            return AllocBuilder.Execute(query).Take(10).ToList();
        }

        [HttpPost]
        public HttpResponseMessage ApproveAllocations(ChangeAllocationData changeAllocationModel)
        {
            var allocs = changeAllocationModel.AllocList;

            var cInfoList = new List<CustomerInfo>();
            foreach (var cAlloc in allocs)
            {
                var query = InfoBuilder.ApplyRelations();
                query.Where(x => x.Id == cAlloc.Info.Id);
                var info = InfoBuilder.Execute(query).First(); //InfoBuilder.Load(cAlloc.Info.Id);
                var forApproveAlloc = info.Allocs.SingleOrDefault(x => x.Id == cAlloc.Id);
                info.AllocStatus = cAlloc.AllocStatus;
                if (forApproveAlloc != null)
                    forApproveAlloc.Status = ColloSysEnums.ApproveStatus.Approved;

                var oldAlloc = info.Allocs.SingleOrDefault(x => x.Id == cAlloc.OrigEntityId);
                if (oldAlloc != null)
                {
                    oldAlloc.EndDate = cAlloc.StartDate.AddDays(-1);
                    oldAlloc.Status = ColloSysEnums.ApproveStatus.Approved;
                }

                cInfoList.Add(info);
            }
            InfoBuilder.Save(cInfoList);
            return Request.CreateResponse(HttpStatusCode.OK,
                                          GetAllocData(changeAllocationModel));

        }

        [HttpPost]
        public HttpResponseMessage RejectChangeAllocations(ChangeAllocationData changeAllocationModel)
        {
            var allocs = changeAllocationModel.AllocList;
            var cInfoList = new List<CustomerInfo>();
            foreach (var cAlloc in allocs)
            {
                cAlloc.Status = ColloSysEnums.ApproveStatus.Approved;
                var query = InfoBuilder.ApplyRelations();
                query.Where(x => x.Id == cAlloc.Info.Id);
                var info = InfoBuilder.Execute(query).First();
                var oldAlloc = info.Allocs.Single(x => x.Id == cAlloc.OrigEntityId);
                oldAlloc.Status = (oldAlloc.AllocStatus == ColloSysEnums.AllocStatus.AllocationError)
                                      ? ColloSysEnums.ApproveStatus.NotApplicable
                                      : ColloSysEnums.ApproveStatus.Approved;
                info.Allocs.Remove(cAlloc);
                cInfoList.Add(info);
            }
            InfoBuilder.Save(cInfoList);
            return Request.CreateResponse(HttpStatusCode.OK,
                                          GetAllocData(changeAllocationModel));

        }

        [HttpGet]
        public IEnumerable<Stakeholders> GetStakeholders(ScbEnums.Products products)
        {
            var data = StakeQuery.OnProduct(products);
            return data;
        }

        [HttpPost]
        public HttpResponseMessage ChangeAllocations(ChangeAllocationData changeAllocationModel)
        {
            if (changeAllocationModel == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            var allocs = changeAllocationModel.AllocList;
            var changeAllocStatus = changeAllocationModel.ChangeAllocStatus;
            var noAllocReason = ColloSysEnums.NoAllocResons.None;

            var cInfoList = new List<CustomerInfo>();
            foreach (var cAlloc in allocs)
            {
                var query = InfoBuilder.ApplyRelations();
                query.Where(x => x.Id == cAlloc.Info.Id);
                var info = InfoBuilder.Execute(query).First(); //InfoBuilder.Load<CustomerInfo>(cAlloc.Info.Id)
                info.NoAllocResons = noAllocReason;
                var oldAlloc = info.Allocs.Single(x => x.Id == cAlloc.Id);
                oldAlloc.Status = ColloSysEnums.ApproveStatus.Changed;

                var newCAlloc = GetNewAlloc<Allocations>(cAlloc, changeAllocStatus);
                newCAlloc.NoAllocResons = noAllocReason;
                info.NoAllocResons = noAllocReason;
                info.Allocs.Add(newCAlloc);
                cInfoList.Add(info);
            }
            InfoBuilder.Save(cInfoList);
            return Request.CreateResponse(HttpStatusCode.OK,
                                          GetAllocData(changeAllocationModel));
        }

        private T GetNewAlloc<T>(T alloc, ColloSysEnums.AllocStatus changeAllocStatus) where T : Allocations
        {
            alloc.OrigEntityId = alloc.Id;
            alloc.ResetUniqueProperties();
            alloc.StartDate = DateTime.Today.AddDays(1);
            alloc.EndDate = alloc.StartDate.AddMonths(1).AddDays(-1);
            alloc.Status = ColloSysEnums.ApproveStatus.Submitted;
            alloc.AllocStatus = changeAllocStatus;
            alloc.AllocSubpolicy = null;
            alloc.AllocPolicy = null;
            if (changeAllocStatus != ColloSysEnums.AllocStatus.AllocateToStakeholder)
                alloc.Stakeholder = null;

            return alloc;
        }

        //private void SaveAllocationChanges<T>(IEnumerable<T> allocData) where T : Entity
        //{
        //    var allocObjects = allocData as IList<T> ?? allocData.ToList();

        //    if (!allocObjects.Any())
        //    {
        //        return;
        //    }

        //    foreach (var allocObject in allocObjects)
        //    {
        //        Session.SaveOrUpdate(allocObject);
        //    }
        //}


        private static GridInitData GetAllocData(ViewAllocationFilter viewAllocationFilter)
        {
            var dateFrom = string.IsNullOrWhiteSpace(viewAllocationFilter.FromDate) ? DateTime.MinValue : Convert.ToDateTime(viewAllocationFilter.FromDate);
            var dateTo = string.IsNullOrWhiteSpace(viewAllocationFilter.ToDate) ? DateTime.MaxValue : Convert.ToDateTime(viewAllocationFilter.ToDate);


            var allcType = ClassType.GetAllocDataClassTypeByTableNameForAlloc(viewAllocationFilter.Products);
            var aliseName = allcType.Name;
            var infoName = "Info";// typeof(CustomerInfo).Name;
            var memberAlloc = new MemberHelper<Allocations>();

            var detachedCriteria = DetachedCriteria.For(allcType, aliseName);
            detachedCriteria.CreateAlias(aliseName + ".Stakeholder", "Stakeholder", JoinType.LeftOuterJoin);
            detachedCriteria.CreateAlias(aliseName + ".AllocSubpolicy", "AllocSubpolicy", JoinType.LeftOuterJoin);
            detachedCriteria.CreateAlias(string.Format(aliseName + "." + infoName), infoName, JoinType.InnerJoin);
            detachedCriteria.Add(Restrictions.Eq(infoName + ".Product", viewAllocationFilter.Products));
            if (viewAllocationFilter.AllocationStatus != ColloSysEnums.AllocStatus.None)
            {
                detachedCriteria.Add(Restrictions.Eq("AllocStatus", viewAllocationFilter.AllocationStatus));
            }

            if (viewAllocationFilter.AllocationStatus != ColloSysEnums.AllocStatus.AllocationError)
            {
                detachedCriteria.Add(Restrictions.Eq("Status", viewAllocationFilter.AproveStatus));
            }
            else
            {
                detachedCriteria.Add(Restrictions.Eq("Status", ColloSysEnums.ApproveStatus.NotApplicable));
            }

            var startDateRes = Restrictions.And(
                  Restrictions.Ge(memberAlloc.GetName(x => x.StartDate), dateFrom.Date),
                  Restrictions.Le(memberAlloc.GetName(x => x.StartDate), dateTo.Date));
            var endDateRes = Restrictions.And(
                  Restrictions.Ge(memberAlloc.GetName(x => x.EndDate), dateFrom.Date),
                  Restrictions.Le(memberAlloc.GetName(x => x.EndDate), dateTo.Date));

            detachedCriteria.Add(Restrictions.Or(startDateRes, endDateRes));


            var gridData = new GridInitData(detachedCriteria, allcType)
            {
                ScreenName = ColloSysEnums.GridScreenName.Allocation
            };

            gridData.QueryParams.GridConfig.columnDefs.Clear();

            // add Allocation Subpolicy Name
            var memberSubpolicy = new MemberHelper<AllocSubpolicy>();
            var subpolicyType = typeof(AllocSubpolicy);
            var properySubpolicyName = subpolicyType.GetProperty(memberSubpolicy.GetName(x => x.Name));
            gridData.AddNewColumn(properySubpolicyName, subpolicyType.Name, "Subpolicy Name");

            // Add Stakholder name Column
            var memberStakeholder = new MemberHelper<Stakeholders>();
            var stakeholderType = typeof(Stakeholders);
            var properyStakeholder = stakeholderType.GetProperty(memberStakeholder.GetName(x => x.Name));
            gridData.AddNewColumn(properyStakeholder, stakeholderType.Name, "Stakeholder Name");

            // add Info Columns
            //var memberInfo = new MemberHelper<CustomerInfo>();
            var infoType = typeof(CustomerInfo);

            var infoColumns = GetSharedInfoPropertiesName();
            for (int i = 0; i < infoColumns.Count; i++)
            {
                var property = infoType.GetProperty(infoColumns[i]);
                gridData.AddNewColumn(property, infoName);
            }

            // add Alloc Columns
            var allocColumns = GetsharedAllocExcelProperties();
            for (int i = 0; i < allocColumns.Count; i++)
            {
                var property = allcType.GetProperty(allocColumns[i]);
                gridData.AddNewColumn(property);
            }

            return gridData;
        }

        #region Grid Columns

        public static IList<string> GetSharedInfoPropertiesName()
        {
            var memberHelper = new MemberHelper<CustomerInfo>();
            return new List<string>
                {
                    memberHelper.GetName(x => x.AccountNo),
                    memberHelper.GetName(x => x.CustomerName),
                    memberHelper.GetName(x => x.CustStatus)
                };
        }

        public static IList<string> GetsharedAllocExcelProperties(ColloSysEnums.FileAliasName? aliasName = null)
        {
            var memberHelper = new MemberHelper<Allocations>();
            return new List<string>
                {
                    memberHelper.GetName(x => x.IsAllocated),
                    memberHelper.GetName(x => x.AllocStatus),
                    memberHelper.GetName(x => x.NoAllocResons),
                    memberHelper.GetName(x => x.Bucket),
                    memberHelper.GetName(x => x.AmountDue),
                    memberHelper.GetName(x => x.StartDate),
                    memberHelper.GetName(x => x.EndDate),
                    memberHelper.GetName(x => x.ChangeReason),
                    memberHelper.GetName(x => x.WithTelecalling),
                    memberHelper.GetName(x => x.ApprovedBy),
                    memberHelper.GetName(x => x.ApprovedOn),
                    memberHelper.GetName(x => x.Status),
                    memberHelper.GetName(x => x.Description),
                };
        }

        #endregion
    }

    public class ChangeAllocationData : ViewAllocationFilter
    {
        public IEnumerable<Allocations> AllocList { get; set; }
        public ColloSysEnums.AllocStatus ChangeAllocStatus { get; set; }
    }

    public class ViewAllocationFilter
    {
        public ScbEnums.Products Products { get; set; }
        public ColloSysEnums.AllocStatus AllocationStatus { get; set; }
        public ColloSysEnums.ApproveStatus AproveStatus { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }
}

//var serializer = new JavaScriptSerializer();
//switch (products)
//{
//    case ScbEnums.Products.CC:

//        break;
//    case ScbEnums.Products.SME_BIL:
//    case ScbEnums.Products.SME_ME:
//    case ScbEnums.Products.SME_LAP:
//    case ScbEnums.Products.MORT:
//    case ScbEnums.Products.AUTO:
//    case ScbEnums.Products.PL:
//        var rallocdata = serializer.Deserialize<IEnumerable<Alloc>>(allocs.ToString()).ToList();

//        var rInfoList = new List<Info>();
//        foreach (var rAlloc in rallocdata)
//        {
//            var info = Session.Load<Info>(rAlloc.Info.Id);
//            info.NoAllocResons = noAllocReason;
//            var oldAlloc = info.Allocs.Single(x => x.Id == rAlloc.Id);
//            oldAlloc.Status = ColloSysEnums.ApproveStatus.Changed;

//            var newRAlloc = GetNewAlloc<Alloc>(rAlloc, changeAllocStatus);
//            newRAlloc.NoAllocResons = noAllocReason;
//            info.NoAllocResons = noAllocReason;
//            info.Allocs.Add(newRAlloc);
//            rInfoList.Add(info);
//        }
//        SaveAllocationChanges(rInfoList);
//        break;
//    case ScbEnums.Products.AUTO_OD:
//    case ScbEnums.Products.SMC:
//    case ScbEnums.Products.SME_LAP_OD:
//        var eallocdata = serializer.Deserialize<IEnumerable<Alloc>>(allocs.ToString()).ToList();

//        var eInfoList = new List<Info>();
//        foreach (var eAlloc in eallocdata)
//        {
//            var info = Session.Load<Info>(eAlloc.Info.Id);
//            info.NoAllocResons = noAllocReason;
//            var oldAlloc = info.Allocs.Single(x => x.Id == eAlloc.Id);
//            oldAlloc.Status = ColloSysEnums.ApproveStatus.Changed;

//            var newEAlloc = GetNewAlloc<Alloc>(eAlloc, changeAllocStatus);
//            newEAlloc.NoAllocResons = noAllocReason;
//            info.NoAllocResons = noAllocReason;
//            info.Allocs.Add(newEAlloc);
//            eInfoList.Add(info);
//        }
//        SaveAllocationChanges(eInfoList);
//        break;
//    default:
//        throw new ArgumentOutOfRangeException();
//}



//var serializer = new JavaScriptSerializer();
//switch (products)
//{
//    case ScbEnums.Products.CC:

//        break;
//    case ScbEnums.Products.SME_BIL:
//    case ScbEnums.Products.SME_ME:
//    case ScbEnums.Products.SME_LAP:
//    case ScbEnums.Products.MORT:
//    case ScbEnums.Products.AUTO:
//    case ScbEnums.Products.PL:
//        var rallocdata = serializer.Deserialize<IEnumerable<Alloc>>(allocs.ToString()).ToList();
//        var rInfoList = new List<Info>();
//        foreach (var rAlloc in rallocdata)
//        {
//            rAlloc.Status = ColloSysEnums.ApproveStatus.Approved;
//            var info = Session.Load<Info>(rAlloc.Info.Id);
//            var oldAlloc = info.Allocs.Single(x => x.Id == rAlloc.OrigEntityId);
//            oldAlloc.Status = (oldAlloc.AllocStatus == ColloSysEnums.AllocStatus.AllocationError)
//                                  ? ColloSysEnums.ApproveStatus.NotApplicable
//                                  : ColloSysEnums.ApproveStatus.Approved;
//            info.Allocs.Remove(rAlloc);
//            rInfoList.Add(info);
//        }
//        SaveAllocationChanges(rInfoList);
//        break;
//    case ScbEnums.Products.AUTO_OD:
//    case ScbEnums.Products.SMC:
//    case ScbEnums.Products.SME_LAP_OD:
//        var eallocdata = serializer.Deserialize<IEnumerable<Alloc>>(allocs.ToString()).ToList();
//        var eInfoList = new List<Info>();
//        foreach (var eAlloc in eallocdata)
//        {
//            eAlloc.Status = ColloSysEnums.ApproveStatus.Approved;
//            var info = Session.Load<Info>(eAlloc.Info.Id);
//            var oldAlloc = info.Allocs.Single(x => x.Id == eAlloc.OrigEntityId);
//            oldAlloc.Status = (oldAlloc.AllocStatus == ColloSysEnums.AllocStatus.AllocationError)
//                                  ? ColloSysEnums.ApproveStatus.NotApplicable
//                                  : ColloSysEnums.ApproveStatus.Approved;
//            info.Allocs.Remove(eAlloc);
//            eInfoList.Add(info);
//        }
//        SaveAllocationChanges(eInfoList);
//        break;
//    default:
//        throw new ArgumentOutOfRangeException();
//}


//var serializer = new JavaScriptSerializer();
//switch (products)
//{
//    case ScbEnums.Products.CC:

//        break;
//    case ScbEnums.Products.SME_BIL:
//    case ScbEnums.Products.SME_ME:
//    case ScbEnums.Products.SME_LAP:
//    case ScbEnums.Products.MORT:
//    case ScbEnums.Products.AUTO:
//    case ScbEnums.Products.PL:
//        var rallocdata = serializer.Deserialize<IEnumerable<Alloc>>(allocs.ToString()).ToList();
//        var rInfoList = new List<Info>();
//        foreach (var rAlloc in rallocdata)
//        {
//            var info = Session.Load<Info>(rAlloc.Info.Id);
//            var forApproveAlloc = info.Allocs.SingleOrDefault(x => x.Id == rAlloc.Id);
//            info.AllocStatus = rAlloc.AllocStatus;
//            if (forApproveAlloc != null)
//                forApproveAlloc.Status = ColloSysEnums.ApproveStatus.Approved;

//            var oldAlloc = info.Allocs.SingleOrDefault(x => x.Id == rAlloc.OrigEntityId);
//            if (oldAlloc != null)
//            {
//                oldAlloc.EndDate = rAlloc.StartDate.AddDays(-1);
//                oldAlloc.Status = ColloSysEnums.ApproveStatus.Approved;
//            }

//            rInfoList.Add(info);
//        }
//        SaveAllocationChanges(rInfoList);
//        break;
//    case ScbEnums.Products.AUTO_OD:
//    case ScbEnums.Products.SMC:
//    case ScbEnums.Products.SME_LAP_OD:
//        var eallocdata = serializer.Deserialize<IEnumerable<Alloc>>(allocs.ToString()).ToList();
//        var eInfoList = new List<Info>();
//        foreach (var eAlloc in eallocdata)
//        {
//            var info = Session.Load<Info>(eAlloc.Info.Id);
//            var forApproveAlloc = info.Allocs.SingleOrDefault(x => x.Id == eAlloc.Id);
//            info.AllocStatus = eAlloc.AllocStatus;
//            if (forApproveAlloc != null)
//                forApproveAlloc.Status = ColloSysEnums.ApproveStatus.Approved;

//            var oldAlloc = info.Allocs.SingleOrDefault(x => x.Id == eAlloc.OrigEntityId);
//            if (oldAlloc != null)
//            {
//                oldAlloc.EndDate = eAlloc.StartDate.AddDays(-1);
//                oldAlloc.Status = ColloSysEnums.ApproveStatus.Approved;
//            }

//            eInfoList.Add(info);
//        }
//        SaveAllocationChanges(eInfoList);
//        break;
//    default:
//        throw new ArgumentOutOfRangeException();
//}


//private static void SetAllocStatusForAllocData(ColloSysEnums.AllocStatus allocStatus, List<SharedAlloc> Allocdata, List<object> list)
//     {
//         foreach (var alloc in Allocdata)
//         {
//             if (alloc.GetType().IsAssignableFrom(typeof(CAlloc)))
//             {
//                 var cAlloc = (CAlloc)alloc;
//                 cAlloc.CInfo.AllocStatus = allocStatus;
//                 list.Add(cAlloc);
//             }

//             if (alloc.GetType().IsAssignableFrom(typeof(EAlloc)))
//             {
//                 var eAlloc = (EAlloc)alloc;
//                 eAlloc.EInfo.AllocStatus = allocStatus;
//                 list.Add(eAlloc);
//             }
//             if (alloc.GetType().IsAssignableFrom(typeof(RAlloc)))
//             {
//                 var rAlloc = (RAlloc)alloc;
//                 rAlloc.RInfo.AllocStatus = allocStatus;
//                 list.Add(rAlloc);
//             }
//         }
//     }

