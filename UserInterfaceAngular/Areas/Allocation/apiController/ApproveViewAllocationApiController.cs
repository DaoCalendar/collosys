using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;
using ColloSys.DataLayer.Allocation;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.SharedDomain;
using ColloSys.Shared.NgGrid;
using ColloSys.Shared.Types4Product;
using ColloSys.UserInterface.Areas.Allocation.ViewModels;
using ColloSys.UserInterface.Shared;
using ColloSys.UserInterface.Shared.Attributes;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using Newtonsoft.Json.Linq;

namespace ColloSys.UserInterface.Areas.Allocation.apiController
{


    public class ApproveViewAllocationApiController : BaseApiController<CAlloc>
    {

        public HttpResponseMessage GetScbSystems()
        {
            var systems = Enum.GetNames(typeof(ScbEnums.Products))
                              .Where(x => x != ScbEnums.Products.UNKNOWN.ToString());
            return Request.CreateResponse(HttpStatusCode.OK, systems);
        }

        [HttpPost]
        [HttpTransaction]
        public HttpResponseMessage FetchPageData(ViewAllocationFilter viewAllocationFilter)
        {
            return Request.CreateResponse(HttpStatusCode.OK, GetAllocData(viewAllocationFilter));
        }

        [HttpGet]
        [HttpTransaction]
        public IEnumerable<CAlloc> GetData()
        {
            var session = SessionManager.GetCurrentSession();
            var data = session.QueryOver<CAlloc>().Where(x => x.IsAllocated)
                .Fetch(x => x.CInfo).Eager
                .Take(10).List();
            return data;
        }


        [HttpPost]
        [HttpTransaction(Persist = true)]
        public HttpResponseMessage ApproveAllocations(ChangeAllocationData changeAllocationModel)
        {
            var products = changeAllocationModel.Products;
            var allocs = changeAllocationModel.AllocList;
            var allocStatus = changeAllocationModel.AllocationStatus;

            var serializer = new JavaScriptSerializer();
            switch (products)
            {
                case ScbEnums.Products.CC:
                    var callocdata = serializer.Deserialize<IEnumerable<CAlloc>>(allocs.ToString()).ToList();
                    var cInfoList = new List<CInfo>();
                    foreach (var cAlloc in callocdata)
                    {
                        var info = Session.Load<CInfo>(cAlloc.CInfo.Id);
                        var forApproveAlloc = info.CAllocs.SingleOrDefault(x => x.Id == cAlloc.Id);
                        info.AllocStatus = cAlloc.AllocStatus;
                        if (forApproveAlloc != null)
                            forApproveAlloc.Status = ColloSysEnums.ApproveStatus.Approved;

                        var oldAlloc = info.CAllocs.SingleOrDefault(x => x.Id == cAlloc.OrigEntityId);
                        if (oldAlloc != null)
                        {
                            oldAlloc.EndDate = cAlloc.StartDate.AddDays(-1);
                            oldAlloc.Status=ColloSysEnums.ApproveStatus.Approved;
                        }

                        cInfoList.Add(info);
                    }
                    SaveAllocationChanges(cInfoList);
                    break;
                case ScbEnums.Products.SME_BIL:
                case ScbEnums.Products.SME_ME:
                case ScbEnums.Products.SME_LAP:
                case ScbEnums.Products.MORT:
                case ScbEnums.Products.AUTO:
                case ScbEnums.Products.PL:
                    var rallocdata = serializer.Deserialize<IEnumerable<RAlloc>>(allocs.ToString()).ToList();
                    var rInfoList = new List<RInfo>();
                    foreach (var rAlloc in rallocdata)
                    {
                        var info = Session.Load<RInfo>(rAlloc.RInfo.Id);
                        var forApproveAlloc = info.RAllocs.SingleOrDefault(x => x.Id == rAlloc.Id);
                        info.AllocStatus = rAlloc.AllocStatus;
                        if (forApproveAlloc != null)
                            forApproveAlloc.Status = ColloSysEnums.ApproveStatus.Approved;

                        var oldAlloc = info.RAllocs.SingleOrDefault(x => x.Id == rAlloc.OrigEntityId);
                        if (oldAlloc != null)
                        {
                            oldAlloc.EndDate = rAlloc.StartDate.AddDays(-1);
                            oldAlloc.Status = ColloSysEnums.ApproveStatus.Approved;
                        }

                        rInfoList.Add(info);
                    }
                    SaveAllocationChanges(rInfoList);
                    break;
                case ScbEnums.Products.AUTO_OD:
                case ScbEnums.Products.SMC:
                case ScbEnums.Products.SME_LAP_OD:
                    var eallocdata = serializer.Deserialize<IEnumerable<EAlloc>>(allocs.ToString()).ToList();
                    var eInfoList = new List<EInfo>();
                    foreach (var eAlloc in eallocdata)
                    {
                        var info = Session.Load<EInfo>(eAlloc.EInfo.Id);
                        var forApproveAlloc = info.EAllocs.SingleOrDefault(x => x.Id == eAlloc.Id);
                        info.AllocStatus = eAlloc.AllocStatus;
                        if (forApproveAlloc != null)
                            forApproveAlloc.Status = ColloSysEnums.ApproveStatus.Approved;

                        var oldAlloc = info.EAllocs.SingleOrDefault(x => x.Id == eAlloc.OrigEntityId);
                        if (oldAlloc != null)
                        {
                            oldAlloc.EndDate = eAlloc.StartDate.AddDays(-1);
                            oldAlloc.Status = ColloSysEnums.ApproveStatus.Approved;
                        }

                        eInfoList.Add(info);
                    }
                    SaveAllocationChanges(eInfoList);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }


            return Request.CreateResponse(HttpStatusCode.OK,
                                          GetAllocData(changeAllocationModel));

        }

        [HttpPost]
        [HttpTransaction(Persist = true)]
        public HttpResponseMessage RejectChangeAllocations(ChangeAllocationData changeAllocationModel)
        {
            var products = changeAllocationModel.Products;
            var allocs = changeAllocationModel.AllocList;
            var allocStatus = changeAllocationModel.AllocationStatus;

            var serializer = new JavaScriptSerializer();
            switch (products)
            {
                case ScbEnums.Products.CC:
                    var callocdata = serializer.Deserialize<IEnumerable<CAlloc>>(allocs.ToString()).ToList();
                    var cInfoList = new List<CInfo>();
                    foreach (var cAlloc in callocdata)
                    {
                        cAlloc.Status = ColloSysEnums.ApproveStatus.Approved;
                        var info = Session.Load<CInfo>(cAlloc.CInfo.Id);
                        var oldAlloc = info.CAllocs.Single(x => x.Id == cAlloc.OrigEntityId);
                        oldAlloc.Status = (oldAlloc.AllocStatus == ColloSysEnums.AllocStatus.AllocationError)
                                              ? ColloSysEnums.ApproveStatus.NotApplicable
                                              : ColloSysEnums.ApproveStatus.Approved;
                        info.CAllocs.Remove(cAlloc);
                        cInfoList.Add(info);
                    }
                    SaveAllocationChanges(cInfoList);
                    break;
                case ScbEnums.Products.SME_BIL:
                case ScbEnums.Products.SME_ME:
                case ScbEnums.Products.SME_LAP:
                case ScbEnums.Products.MORT:
                case ScbEnums.Products.AUTO:
                case ScbEnums.Products.PL:
                    var rallocdata = serializer.Deserialize<IEnumerable<RAlloc>>(allocs.ToString()).ToList();
                    var rInfoList = new List<RInfo>();
                    foreach (var rAlloc in rallocdata)
                    {
                        rAlloc.Status = ColloSysEnums.ApproveStatus.Approved;
                        var info = Session.Load<RInfo>(rAlloc.RInfo.Id);
                        var oldAlloc = info.RAllocs.Single(x => x.Id == rAlloc.OrigEntityId);
                        oldAlloc.Status = (oldAlloc.AllocStatus == ColloSysEnums.AllocStatus.AllocationError)
                                              ? ColloSysEnums.ApproveStatus.NotApplicable
                                              : ColloSysEnums.ApproveStatus.Approved;
                        info.RAllocs.Remove(rAlloc);
                        rInfoList.Add(info);
                    }
                    SaveAllocationChanges(rInfoList);
                    break;
                case ScbEnums.Products.AUTO_OD:
                case ScbEnums.Products.SMC:
                case ScbEnums.Products.SME_LAP_OD:
                    var eallocdata = serializer.Deserialize<IEnumerable<EAlloc>>(allocs.ToString()).ToList();
                    var eInfoList = new List<EInfo>();
                    foreach (var eAlloc in eallocdata)
                    {
                        eAlloc.Status = ColloSysEnums.ApproveStatus.Approved;
                        var info = Session.Load<EInfo>(eAlloc.EInfo.Id);
                        var oldAlloc = info.EAllocs.Single(x => x.Id == eAlloc.OrigEntityId);
                        oldAlloc.Status = (oldAlloc.AllocStatus == ColloSysEnums.AllocStatus.AllocationError)
                                              ? ColloSysEnums.ApproveStatus.NotApplicable
                                              : ColloSysEnums.ApproveStatus.Approved;
                        info.EAllocs.Remove(eAlloc);
                        eInfoList.Add(info);
                    }
                    SaveAllocationChanges(eInfoList);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }


            return Request.CreateResponse(HttpStatusCode.OK,
                                          GetAllocData(changeAllocationModel));

        }

        [HttpGet]
        [HttpTransaction]
        public IEnumerable<Stakeholders> GetStakeholders(ScbEnums.Products products)
        {
            Stakeholders stakeholders = null;
            StkhWorking workings = null;
            StkhHierarchy hierarchy = null;
            var session = SessionManager.GetCurrentSession();
            var data = session.QueryOver(() => stakeholders)
                                      .Fetch(x => x.StkhWorkings).Eager
                                      .JoinAlias(() => stakeholders.StkhWorkings, () => workings, JoinType.LeftOuterJoin)
                                      .JoinAlias(() => stakeholders.Hierarchy, () => hierarchy,
                                                 JoinType.LeftOuterJoin)
                                      .Where(() => workings.Products == products)
                                      .And(() => hierarchy.IsInAllocation)
                                      .And(() => stakeholders.JoiningDate < DateTime.Today.Date)
                                      .And(() => stakeholders.LeavingDate == null ||
                                                 stakeholders.LeavingDate > DateTime.Today.Date)
                                      .TransformUsing(Transformers.DistinctRootEntity)
                                      .List();
            return data;
        }

        [HttpPost]
        [HttpTransaction(Persist = true)]
        public HttpResponseMessage ChangeAllocations(ChangeAllocationData changeAllocationModel)
        {
            if (changeAllocationModel == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            var products = changeAllocationModel.Products;
            var allocs = changeAllocationModel.AllocList;
            var allocStatus = changeAllocationModel.AllocationStatus;
            var changeAllocStatus = changeAllocationModel.ChangeAllocStatus;
            var noAllocReason = ColloSysEnums.NoAllocResons.None;

            var serializer = new JavaScriptSerializer();
            switch (products)
            {
                case ScbEnums.Products.CC:
                    var callocdata = serializer.Deserialize<IEnumerable<CAlloc>>(allocs.ToString()).ToList();

                    var cInfoList = new List<CInfo>();
                    foreach (var cAlloc in callocdata)
                    {
                        var info = Session.Load<CInfo>(cAlloc.CInfo.Id);
                        info.NoAllocResons = noAllocReason;
                        var oldAlloc = info.CAllocs.Single(x => x.Id == cAlloc.Id);
                        oldAlloc.Status = ColloSysEnums.ApproveStatus.Changed;

                        var newCAlloc = GetNewAlloc<CAlloc>(cAlloc, changeAllocStatus);
                        newCAlloc.NoAllocResons = noAllocReason;
                        info.NoAllocResons = noAllocReason;
                        info.CAllocs.Add(newCAlloc);
                        cInfoList.Add(info);
                    }
                    SaveAllocationChanges(cInfoList);
                    break;
                case ScbEnums.Products.SME_BIL:
                case ScbEnums.Products.SME_ME:
                case ScbEnums.Products.SME_LAP:
                case ScbEnums.Products.MORT:
                case ScbEnums.Products.AUTO:
                case ScbEnums.Products.PL:
                    var rallocdata = serializer.Deserialize<IEnumerable<RAlloc>>(allocs.ToString()).ToList();

                    var rInfoList = new List<RInfo>();
                    foreach (var rAlloc in rallocdata)
                    {
                        var info = Session.Load<RInfo>(rAlloc.RInfo.Id);
                        info.NoAllocResons = noAllocReason;
                        var oldAlloc = info.RAllocs.Single(x => x.Id == rAlloc.Id);
                        oldAlloc.Status = ColloSysEnums.ApproveStatus.Changed;

                        var newRAlloc = GetNewAlloc<RAlloc>(rAlloc, changeAllocStatus);
                        newRAlloc.NoAllocResons = noAllocReason;
                        info.NoAllocResons = noAllocReason;
                        info.RAllocs.Add(newRAlloc);
                        rInfoList.Add(info);
                    }
                    SaveAllocationChanges(rInfoList);
                    break;
                case ScbEnums.Products.AUTO_OD:
                case ScbEnums.Products.SMC:
                case ScbEnums.Products.SME_LAP_OD:
                    var eallocdata = serializer.Deserialize<IEnumerable<EAlloc>>(allocs.ToString()).ToList();

                    var eInfoList = new List<EInfo>();
                    foreach (var eAlloc in eallocdata)
                    {
                        var info = Session.Load<EInfo>(eAlloc.EInfo.Id);
                        info.NoAllocResons = noAllocReason;
                        var oldAlloc = info.EAllocs.Single(x => x.Id == eAlloc.Id);
                        oldAlloc.Status = ColloSysEnums.ApproveStatus.Changed;

                        var newEAlloc = GetNewAlloc<EAlloc>(eAlloc, changeAllocStatus);
                        newEAlloc.NoAllocResons = noAllocReason;
                        info.NoAllocResons = noAllocReason;
                        info.EAllocs.Add(newEAlloc);
                        eInfoList.Add(info);
                    }
                    SaveAllocationChanges(eInfoList);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return Request.CreateResponse(HttpStatusCode.OK,
                                          GetAllocData(changeAllocationModel));
        }

        private T GetNewAlloc<T>(T alloc, ColloSysEnums.AllocStatus changeAllocStatus) where T : SharedAlloc
        {
            alloc.OrigEntityId = alloc.Id;
            alloc.ResetUniqueProperties();
            alloc.StartDate = DateTime.Today.AddDays(1);
            alloc.Status = ColloSysEnums.ApproveStatus.Submitted;
            alloc.AllocStatus = changeAllocStatus;
            alloc.AllocSubpolicy = null;
            alloc.AllocPolicy = null;
            if (changeAllocStatus != ColloSysEnums.AllocStatus.AllocateToStakeholder)
                alloc.Stakeholder = null;

            return alloc;
        }

        private void SaveAllocationChanges<T>(IEnumerable<T> allocData) where T : Entity
        {
            var allocObjects = allocData as IList<T> ?? allocData.ToList();

            if (!allocObjects.Any())
            {
                return;
            }

            foreach (var allocObject in allocObjects)
            {
                Session.SaveOrUpdate(allocObject);
            }
        }


        private static GridInitData GetAllocData(ViewAllocationFilter viewAllocationFilter)
        {
            var dateFrom = string.IsNullOrWhiteSpace(viewAllocationFilter.FromDate) ? DateTime.MinValue : Convert.ToDateTime(viewAllocationFilter.FromDate);
            var dateTo = string.IsNullOrWhiteSpace(viewAllocationFilter.ToDate) ? DateTime.MaxValue : Convert.ToDateTime(viewAllocationFilter.ToDate);


            var allcType = ClassType.GetAllocDataClassTypeByTableNameForAlloc(viewAllocationFilter.Products);
            var firstChar = allcType.Name.Substring(0, 1);
            var aliseName = allcType.Name;
            var infoName = firstChar + "Info";
            var memberAlloc = new MemberHelper<SharedAlloc>();

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


            var gridData = new GridInitData(detachedCriteria, allcType);
            gridData.ScreenName = ColloSysEnums.GridScreenName.Allocation;
            gridData.QueryParams.GridConfig.columnDefs.Clear();

            // add Allocation Subpolicy Name
            var memberSubpolicy = new MemberHelper<AllocSubpolicy>();
            var subpolicyType = typeof(AllocSubpolicy);
            var properySubpolicyName = subpolicyType.GetProperty(memberSubpolicy.GetName(x => x.Name));
            gridData.AddNewColumn(properySubpolicyName, "AllocSubpolicy", "SubpolicyName");

            // Add Stakholder name Column
            var memberStakeholder = new MemberHelper<Stakeholders>();
            var stakeholderType = typeof(Stakeholders);
            var properyStakeholder = stakeholderType.GetProperty(memberStakeholder.GetName(x => x.Name));
            gridData.AddNewColumn(properyStakeholder, "Stakeholder", "StakeholderName");

            // add Info Columns
            var infoType = typeof(CInfo).Assembly.GetTypes().SingleOrDefault(x => x.Name == infoName);
            if (infoType == null)
                return gridData;

            var infoColumns = GetSharedInfoPropertiesName();
            for (int i = 0; i < infoColumns.Count; i++)
            {
                var property = infoType.GetProperty(infoColumns[i]);
                gridData.AddNewColumn(property, infoType.Name);
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
            var memberHelper = new MemberHelper<CInfo>();
            return new List<string>
                {
                    memberHelper.GetName(x => x.AccountNo),
                    memberHelper.GetName(x => x.CustomerName),
                    memberHelper.GetName(x => x.CustStatus)
                };
        }

        public static IList<string> GetsharedAllocExcelProperties(ColloSysEnums.FileAliasName? aliasName = null)
        {
            var memberHelper = new MemberHelper<CAlloc>();
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
        public object AllocList { get; set; }
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

