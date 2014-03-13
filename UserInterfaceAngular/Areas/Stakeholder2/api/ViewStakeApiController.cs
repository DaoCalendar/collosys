#region references

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Security;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.Services.Shared;
using ColloSys.QueryBuilder.GenericBuilder;
using ColloSys.QueryBuilder.StakeholderBuilder;
using ColloSys.UserInterface.Areas.Generic.Models;
using ColloSys.UserInterface.Areas.Stakeholder2.Models;
using ColloSys.UserInterface.AuthNAuth.Membership;
using NHibernate.Linq;
using ColloSys.UserInterface.Shared.Attributes;
using NLog;

#endregion

//stakeholders calls changed
//hierarchy calls changed
namespace ColloSys.UserInterface.Areas.Stakeholder2.api
{

    public class ViewStakeApiController : ApiController
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private static readonly HierarchyQueryBuilder HierarchyQuery = new HierarchyQueryBuilder();
        private static readonly StakeQueryBuilder StakeQuery = new StakeQueryBuilder();
        private static readonly GPincodeBuilder GPincodeBuilder=new GPincodeBuilder();
        private static readonly GPermissionBuilder GPermissionBuilder=new GPermissionBuilder();

        [HttpGet]
        [HttpTransaction]
        public IEnumerable<StkhHierarchy> GetStakeHierarchy()
        {
            return HierarchyQuery.GetOnExpression(x => x.Hierarchy != "Developer").ToList();
        }

        [HttpGet]
        [HttpTransaction]
        public IEnumerable<Stakeholders> GetAllStakeHolders()
        {
            var query = StakeQuery.WithRelation();

            var allData = StakeQuery.ExecuteQuery(query).ToList();
            //remove rejected stakeholders
            allData = (from d in allData
                       where d.Status != ColloSysEnums.ApproveStatus.Rejected
                       select d).ToList();

            RemoveUnusedPaymentsWorkings(allData);
            return allData;
        }

        [HttpGet]
        [HttpTransaction]
        public IEnumerable<GPincode> GetPincodes(string pincode, string level)
        {
            return level == "City" ? GetPincodesCity(pincode) : GetPincodesArea(pincode);
        }

        [HttpPost]
        [HttpTransaction(Persist = true)]
        public HttpResponseMessage SetLeaveForStakeholder(ManageWorkingModel manageWorkingModel)
        {
            var changesStakeholders = ManageWorkingModel.ChangeWorking(manageWorkingModel);

            return Request.CreateResponse(HttpStatusCode.OK, changesStakeholders);
        }

        private static IEnumerable<GPincode> GetPincodesCity(string pin)
        {
            var list = GPincodeBuilder.OnPinOrCity(pin).ToList();
            if (list.Count == 0) return null;

            var uniq = (from l in list group l by l.City into g select g.First()).ToList();
            uniq.RemoveAll(x => x.City.Trim().Equals("-"));
            return uniq.Take(10);
        }

        private static IEnumerable<GPincode> GetPincodesArea(string pin)
        {
            var list = GPincodeBuilder.OnPinOrArea(pin).ToList();
            if (list.Count == 0) return null;

            var uniq = (from l in list group l by l.Area into g select g.First()).ToList();
            uniq.RemoveAll(x => x.City.Trim().Equals("-"));
            return uniq.Take(10);
        }

        [HttpGet]
        [HttpTransaction]
        public HttpResponseMessage AllData()
        {
            var allLists = new
            {
                completeData = GetAllStakeHolders(),
                hierarchyDesignation = GetStakeHierarchy(),
                currUserData = AuthService.GetPremissionsForCurrentUser()
                .Where(x => x.Activity == ColloSysEnums.Activities.Stakeholder).SingleOrDefault(),
                products = Enum.GetNames(typeof(ScbEnums.Products)).Where(x => x != ScbEnums.Products.UNKNOWN.ToString()).ToList()
            };
            return Request.CreateResponse(HttpStatusCode.OK, allLists);
        }

        [HttpGet]
        [HttpTransaction]
        public IEnumerable<Stakeholders> GetReportee(Guid stakeId)
        {
            var query = StakeQuery.WithRelation().Where(x => x.ReportingManager == stakeId);
            return StakeQuery.ExecuteQuery(query).Distinct().ToList();
        }

        [HttpGet]
        [HttpTransaction]
        public int GetTotalCount(Guid hierarchyId, string filterView)
        {
            var query = StakeQuery.WithRelation();
            if (filterView == "All")
            {
                query.Where(x =>
                            x.Hierarchy.Id == hierarchyId && x.Status == ColloSysEnums.ApproveStatus.Approved);
                var count = StakeQuery.ExecuteQuery(query).Count();
                return count;
            }
            if (filterView == "Active")
            {
                query.Where(
                    x => x.Hierarchy.Id == hierarchyId && (x.LeavingDate <= DateTime.Now || x.LeavingDate == null))
                     .And(x => x.Status == ColloSysEnums.ApproveStatus.Approved);
                var count = StakeQuery.ExecuteQuery(query).Count();
                return count;
            }
            if (filterView == "InActive")
            {
                query.Where(x => x.Hierarchy.Id == hierarchyId && x.LeavingDate <= DateTime.Now)
                     .And(x => x.Status == ColloSysEnums.ApproveStatus.Approved);
                var count = StakeQuery.ExecuteQuery(query).Count();
                return count;
            }
            if (filterView == "ViewPending")
            {
                query.Where(x => x.Hierarchy.Id == hierarchyId && x.Status == ColloSysEnums.ApproveStatus.Submitted)
                     .And(x => x.ApprovedBy == HttpContext.Current.User.Identity.Name);
                var count = StakeQuery.ExecuteQuery(query).Count();
                return count;
            }
            if (filterView == "ReportingTo")
            {
                query.Where(x => x.Hierarchy.ReportsTo == hierarchyId)
                     .And(x => x.Status == ColloSysEnums.ApproveStatus.Approved);
                var count = StakeQuery.ExecuteQuery(query).Count();
                return count;
            }

            return 0;
        }


        [HttpGet]
        [HttpTransaction]
        public int GetTotalCountforPending(string filterView)
        {
            var query = StakeQuery.WithRelation();
            if (filterView == "PendingForAll")
            {
                query.Where(x => x.Status == ColloSysEnums.ApproveStatus.Submitted);
                var count = StakeQuery.ExecuteQuery(query).Count();
                return count;
            }
            if (filterView == "PendingForMe")
            {
                query.Where(x => x.Status == ColloSysEnums.ApproveStatus.Submitted
                                 && x.ApprovedBy == HttpContext.Current.User.Identity.Name);
                var count = StakeQuery.ExecuteQuery(query).Count();
                return count;
            }
            return 0;
        }

        [HttpGet]
        [HttpTransaction]
        public int GetTotalCountForProduct(ScbEnums.Products product)
        {
            var query = StakeQuery.WithRelation();
            Stakeholders stake = null;
            StkhWorking stkh = null;
            query.JoinQueryOver(() => stake.StkhWorkings, () => stkh)
                 .Where(() => stkh.Products == product)
                 .And(() => stake.Status == ColloSysEnums.ApproveStatus.Approved);

            var count = StakeQuery.ExecuteQuery(query).Count();
            return count;
        }

        [HttpGet]
        [HttpTransaction]
        public int GetTotalCountForStake(Guid Id)
        {
            var query = StakeQuery.WithRelation();
            query.Where(x => x.ReportingManager == Id)
                 .And(x => x.Status == ColloSysEnums.ApproveStatus.Approved);
            var count = StakeQuery.ExecuteQuery(query).Count();
            return count;
        }

        [HttpGet]
        [HttpTransaction]
        public IEnumerable<Stakeholders> GetStakeholder(Guid hierarchyId, int start, int size, string filterView)
        {
            if (hierarchyId == Guid.Empty)
            {
                return null;
            }
            Stakeholders stake = null;
            StkhHierarchy stkh = null;
            var query = StakeQuery.WithRelation();
            query.JoinQueryOver(() => stake);
            if (filterView == "All")
            {
                query.JoinQueryOver(() => stake.Hierarchy, () => stkh)
                     .Where(() => stkh.Id == hierarchyId)
                     .And(() => stake.Status == ColloSysEnums.ApproveStatus.Approved);
            }
            if (filterView == "Active")
            {
                query.JoinQueryOver(() => stake.Hierarchy, () => stkh)
                     .Where(
                         () =>
                         stkh.Id == hierarchyId && (stake.LeavingDate <= DateTime.Now || stake.LeavingDate == null))
                     .And(() => stake.Status == ColloSysEnums.ApproveStatus.Approved);
            }
            if (filterView == "Inactive")
            {
                query.JoinQueryOver(() => stake.Hierarchy, () => stkh)
                     .Where(() => stkh.Id == hierarchyId && stake.LeavingDate <= DateTime.Now)
                     .And(() => stake.Status == ColloSysEnums.ApproveStatus.Approved);
            }
            if (filterView == "ViewPending")
            {
                query.JoinQueryOver(() => stake.Hierarchy, () => stkh)
                     .Where(() => stkh.Id == hierarchyId)
                     .And(() => stake.ApprovedBy == HttpContext.Current.User.Identity.Name
                                && stake.Status == ColloSysEnums.ApproveStatus.Submitted);
            }
            if (filterView == "ReportingTo")
            {
                query.JoinQueryOver(() => stake.Hierarchy, () => stkh)
                     .Where(() => stkh.Id == hierarchyId)
                     .And(() => stake.Status == ColloSysEnums.ApproveStatus.Approved);
            }
            var result = StakeQuery.ExecuteQuery(query).Skip(start).Take(size).ToList();
            return RemoveUnusedPaymentsWorkings(result);
        }


        [HttpGet]
        [HttpTransaction]
        public IEnumerable<Stakeholders> GetReportees(Guid Id, int start, int size)
        {
            var query = StakeQuery.WithRelation();
            query.Where(x => x.ReportingManager == Id)
                 .And(x => x.Status == ColloSysEnums.ApproveStatus.Approved);
            var stakeholder = StakeQuery.ExecuteQuery(query).Skip(start).Take(size).ToList();
            return RemoveUnusedPaymentsWorkings(stakeholder);
        }

        [HttpGet]
        [HttpTransaction]
        public IEnumerable<Stakeholders> GetStakeByProduct(ScbEnums.Products product, int start, int size)
        {
            Stakeholders stake = null;
            StkhWorking stkh = null;

            var query = StakeQuery.WithRelation();
            query.JoinQueryOver(() => stake)
                .JoinQueryOver(() => stake.StkhWorkings, () => stkh)
                 .Where(() => stkh.Products == product)
                 .And(() => stake.Status == ColloSysEnums.ApproveStatus.Approved);
            var stakeholder = StakeQuery.ExecuteQuery(query).Skip(start).Take(size).ToList();
            return RemoveUnusedPaymentsWorkings(stakeholder);
        }

        [HttpGet]
        [HttpTransaction]
        public IEnumerable<Stakeholders> GetPendingStakeholder(int start, int size, string filterView)
        {
            var query = StakeQuery.WithRelation();
            Stakeholders stake = null;
            query.JoinQueryOver(() => stake);

            if (filterView == "PendingForAll")
            {
                query.Where(() => stake.Status == ColloSysEnums.ApproveStatus.Submitted);
            }

            if (filterView == "PendingForMe")
            {
                query.Where(() => stake.Status == ColloSysEnums.ApproveStatus.Submitted
                                  && stake.ApprovedBy == HttpContext.Current.User.Identity.Name);
            }
            var stakeholder = StakeQuery.ExecuteQuery(query).Skip(start).Take(size).ToList();
            return RemoveUnusedPaymentsWorkings(stakeholder);
        }

        [HttpGet]
        [HttpTransaction]
        public HttpResponseMessage GetStakeholderData(Guid hierarchyId, int start, int size, string filterView)
        {
            var stkhData = (List<Stakeholders>)GetStakeholder(hierarchyId, start, size, filterView);

            var data = new
                {
                    stkhholderData = stkhData,
                    reportingManager = GetReportingManager(stkhData)
                };
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpGet]
        [HttpTransaction]
        public HttpResponseMessage GetPendingStkhData(int start, int size, string filterView)
        {
            var stkhData = (List<Stakeholders>)GetPendingStakeholder(start, size, filterView);

            var data = new
                {
                    stkhholderData = stkhData,
                    reportingManager = GetReportingManager(stkhData)
                };
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpGet]
        [HttpTransaction]
        public HttpResponseMessage GetStkhDataForProduct(ScbEnums.Products product, int start, int size)
        {
            var stkhData = (List<Stakeholders>)GetStakeByProduct(product, start, size);

            var data = new
                {
                    stkhholderData = stkhData,
                    reportingManager = GetReportingManager(stkhData)
                };
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpPost]
        [HttpTransaction]
        public IEnumerable<Stakeholders> GetReportingManager(List<Stakeholders> data)
        {
            if (data != null)
            {
                var reportingManagerList = new List<Stakeholders>();
                foreach (var item in data)
                {
                    Stakeholders item1 = item;
                    if (item1.ReportingManager == Guid.Empty)
                    {
                        var stake = new Stakeholders();
                        reportingManagerList.Add(stake);
                    }
                    else
                    {
                        reportingManagerList.AddRange(StakeQuery.GetOnExpression(x => item1 != null && x.Id == item1.ReportingManager));
                    }
                }
                return reportingManagerList;
            }
            return null;
        }

        [HttpGet]
        [HttpTransaction]
        public HttpResponseMessage GetStkhDataByStakeHolder(Guid Id, int start, int size)
        {
            var stkhData = (List<Stakeholders>)GetReportees(Id, start, size);

            var data = new
                {
                    stkhholderData = stkhData,
                    reportingManager = GetReportingManager(stkhData)
                };
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpGet]
        [HttpTransaction]
        public IEnumerable<Stakeholders> GetReportsToStake(Guid stakeId)
        {
            Stakeholders stake = null;
            StkhHierarchy stkh = null;
            var query = StakeQuery.WithRelation();
            query.JoinQueryOver(() => stake)
                 .JoinQueryOver(() => stake.Hierarchy, () => stkh)
                 .Where(() => stkh.Id == stakeId);

            var stakeholder = StakeQuery.ExecuteQuery(query).ToList();
            return stakeholder;
        }

        private static IEnumerable<Stakeholders> RemoveUnusedPaymentsWorkings(IList<Stakeholders> stakeholderses)
        {
            foreach (var stakeholderse in stakeholderses)
            {
                stakeholderse.StkhPayments = stakeholderse.StkhPayments.Distinct().ToList();
                stakeholderse.StkhWorkings = stakeholderse.StkhWorkings.Distinct().ToList();
                stakeholderse.StkhRegistrations = stakeholderse.StkhRegistrations.Distinct().ToList();
                stakeholderse.GAddress = stakeholderse.GAddress.Distinct().ToList();
                stakeholderse.StkhWorkings = (from d in stakeholderse.StkhWorkings
                                              where d.Status != ColloSysEnums.ApproveStatus.Rejected
                                              select d).ToList();
                stakeholderse.StkhPayments = (from d in stakeholderse.StkhPayments
                                              where d.Status != ColloSysEnums.ApproveStatus.Rejected
                                              select d).ToList();
                var deletelist = (from d in stakeholderse.StkhWorkings
                                  where d.Status == ColloSysEnums.ApproveStatus.Approved &&
                                        d.RowStatus == RowStatus.Delete
                                  select d).ToList();
                var deletepayment = (from d in stakeholderse.StkhPayments
                                     where d.Status == ColloSysEnums.ApproveStatus.Approved &&
                                           d.RowStatus == RowStatus.Delete
                                     select d).ToList();
                deletelist.ForEach(x => stakeholderse.StkhWorkings.Remove(x));
                deletepayment.ForEach(x => stakeholderse.StkhPayments.Remove(x));
            }
            return stakeholderses;
        }
        [HttpGet]
        [HttpTransaction]
        public IEnumerable<GPermission> GetPermissions()
        {
            var query = GPermissionBuilder.WithRelation();
            return GPermissionBuilder.ExecuteQuery(query).ToList();
        }

        [HttpGet]
        [HttpTransaction]
        public IEnumerable<Stakeholders> GetStakeListForManageWorking(Guid stakeholders, Guid hierarcyId)
        {
            if (hierarcyId == Guid.Empty)
                return new List<Stakeholders>();

            //get first level stakeholders
            var list = StakeQuery.OnHierarchyId(hierarcyId)
                                 .Where(x => x.Id != stakeholders)
                                 .ToList();

            var oneLevelBelowHierarchyList = HierarchyQuery
                .GetOnExpression(x => x.ReportsTo == hierarcyId);

            foreach (var stkhHierarchy in oneLevelBelowHierarchyList)
            {
                var oneLevelBelowList = StakeQuery.OnHierarchyId(stkhHierarchy.Id);

                if (oneLevelBelowList.Any())
                    list.AddRange(oneLevelBelowList);
            }

            return list;
        }

        //TODO:Amol
        [HttpPost]
        [HttpTransaction(Persist = true)]
        public void SaveApprovedAndRejectUser(Stakeholders stakeholders)
        {
            var hierarchy = GetHierarchy(stakeholders.Hierarchy.Designation, stakeholders.Hierarchy.Hierarchy).ToList().FirstOrDefault();

            _log.Info("Hierarchy received for user in approve");
            _log.Info("User Creation started");

            if (stakeholders.Status.Equals(ColloSysEnums.ApproveStatus.Rejected))
            {
                SetApproveRejectStatus(stakeholders, ColloSysEnums.ApproveStatus.Rejected);
                var stake = Update(stakeholders);
                _log.Info("Stakeholder Rejected" + stake.Id);
            }
            else
            {
                SetApproveRejectStatus(stakeholders, ColloSysEnums.ApproveStatus.Approved);
                foreach (var stkhWorking in stakeholders.StkhWorkings)
                {
                    SetApproveRejectStatus(stkhWorking, ColloSysEnums.ApproveStatus.Approved);
                }
                foreach (var stkhPayment in stakeholders.StkhPayments)
                {
                    SetApproveRejectStatus(stkhPayment, ColloSysEnums.ApproveStatus.Approved);
                }

                if (hierarchy != null && (hierarchy.IsUser))
                {
                    var provider = new FnhMembershipProvider();
                    MembershipCreateStatus status;
                    var role = hierarchy;
                    provider.CreateUser(stakeholders.ExternalId, stakeholders.EmailId, stakeholders.JoiningDate, role, out status);
                    var body = EmailNotification.TemplateForApproveuser(stakeholders.Name, stakeholders.ExternalId);
                    _log.Info("Sending mail to User");

                    //MailReport.SendMail(body, stakeholders.EmailId);

                    _log.Info("Mail sended to user");
                    _log.Info("User created");
                }
                Update(stakeholders);
                _log.Info("Stakeholder Approved and user created");
            }
        }

        [HttpPost]
        [HttpTransaction(Persist = true)]
        public void SaveApprovedWorkings(Stakeholders stakeholders)
        {
            foreach (var stkhWorking in stakeholders.StkhWorkings)
            {
                if (stkhWorking.Status == ColloSysEnums.ApproveStatus.Submitted)
                {
                    SetApproveRejectStatus(stkhWorking, ColloSysEnums.ApproveStatus.Approved);
                    if (stakeholders.Hierarchy.HasPayment)
                    {
                        var working = stkhWorking;
                        var payment = (from d in stakeholders.StkhPayments
                                       where d.Id == working.StkhPayment.Id
                                       select d).Single();
                        SetApproveRejectStatus(payment, ColloSysEnums.ApproveStatus.Approved);
                        SetApproveRejectStatus(stkhWorking.StkhPayment, ColloSysEnums.ApproveStatus.Approved);
                    }
                }

            }
            Update(stakeholders);
            _log.Info("Stakeholders Workings Approved");
        }

        [HttpPost]
        [HttpTransaction(Persist = true)]
        public void SaveRejectedWorkings(Stakeholders stakeholders)
        {
            foreach (var stkhWorking in stakeholders.StkhWorkings)
            {
                if (stkhWorking.Status == ColloSysEnums.ApproveStatus.Submitted && stkhWorking.RowStatus != RowStatus.Delete)
                {
                    SetApproveRejectStatus(stkhWorking, ColloSysEnums.ApproveStatus.Rejected);
                    if (stakeholders.Hierarchy.HasPayment)
                    {
                        var working = stkhWorking;
                        var payment = (from d in stakeholders.StkhPayments
                                       where d.Id == working.StkhPayment.Id
                                       select d).Single();
                        SetApproveRejectStatus(payment, ColloSysEnums.ApproveStatus.Rejected);
                        SetApproveRejectStatus(stkhWorking.StkhPayment, ColloSysEnums.ApproveStatus.Rejected);
                    }
                }
                else if (stkhWorking.Status == ColloSysEnums.ApproveStatus.Submitted && stkhWorking.RowStatus == RowStatus.Delete)
                {
                    SetApproveRejectStatus(stkhWorking, ColloSysEnums.ApproveStatus.Approved);
                    stkhWorking.RowStatus = RowStatus.Update;
                    if (stakeholders.Hierarchy.HasPayment)
                    {
                        var working = stkhWorking;
                        var payment = (from d in stakeholders.StkhPayments
                                       where d.Id == working.StkhPayment.Id
                                       select d).Single();
                        SetApproveRejectStatus(payment, ColloSysEnums.ApproveStatus.Approved);
                        payment.RowStatus = RowStatus.Update;
                        SetApproveRejectStatus(stkhWorking.StkhPayment, ColloSysEnums.ApproveStatus.Approved);
                        stkhWorking.StkhPayment.RowStatus = RowStatus.Update;
                    }
                }
            }
            Update(stakeholders);
            _log.Info("Stakeholders Workings Rejected");
        }

        private static void SetApproveRejectStatus(IApproverComponent approved, ColloSysEnums.ApproveStatus status)
        {
            approved.Status = status;
            approved.ApprovedBy = AuthService.CurrentUser;
            approved.ApprovedOn = DateTime.Now;
        }

        [HttpPost]
        [HttpTransaction(Persist = true)]
        public void SaveListApprovedAndRejectUser(IEnumerable<Stakeholders> stakeholderses)
        {
            foreach (var stake in stakeholderses)
            {
                SaveApprovedAndRejectUser(stake);
            }
        }
        [HttpPost]
        [HttpTransaction(Persist = true)]
        public void SavePushToHigher(Stakeholders data)
        {
            if (data.Hierarchy.HasWorking && !data.Hierarchy.HasPayment)
            {
                foreach (var stkh in data.StkhWorkings)
                {
                    stkh.Stakeholder = data;
                }
                foreach (var stkh in data.GAddress)
                {
                    stkh.Stakeholder = data;
                }
                foreach (var stkh in data.StkhRegistrations)
                {
                    stkh.Stakeholder = data;
                }
            }
            if (data.Hierarchy.HasWorking && data.Hierarchy.HasPayment)
            {
                foreach (var working in data.StkhWorkings)
                {
                    working.Stakeholder = data;
                }

                foreach (var payment in data.StkhPayments)
                {
                    payment.Stakeholder = data;
                    payment.StkhWorkings = data.StkhWorkings;
                }
                foreach (var stkh in data.GAddress)
                {
                    stkh.Stakeholder = data;
                }
                foreach (var stkh in data.StkhRegistrations)
                {
                    stkh.Stakeholder = data;
                }
            }
            StakeQuery.Save(data);
        }

        #region approve individual

        public static Stakeholders ApproveIndividual(Stakeholders stakeholders, string approve, string userName)
        {
            switch (approve)
            {
                case "Working":
                    LogManager.GetCurrentClassLogger().Info("StakeholderServices: Stakeholders Working set for approve with Stake id: " + stakeholders.Id);
                    SetApprovedWorking(stakeholders, userName);
                    break;
                case "Payment":
                    LogManager.GetCurrentClassLogger().Info("StakeholderServices: Stakeholders Payment set for approve with Stake id: " + stakeholders.Id);
                    SetApprovedPayment(stakeholders, userName);
                    break;
                case "Address":
                    LogManager.GetCurrentClassLogger().Info("StakeholderServices: Stakeholders Address set for approve with Stake id: " + stakeholders.Id);
                    SetApprovedAddress(stakeholders, userName);
                    break;
            }
            Update(stakeholders);
            return stakeholders;
        }

        private static void SetApprovedAddress(Stakeholders stakeholders, string userName)
        {
            foreach (var address in stakeholders.GAddress)
            {
                address.Status = ColloSysEnums.ApproveStatus.Approved;
                address.ApprovedBy = userName;
                address.ApprovedOn = DateTime.Now;
            }
        }

        private static void SetApprovedPayment(Stakeholders stakeholders, string userName)
        {
            foreach (var payment in stakeholders.StkhPayments)
            {
                payment.Status = ColloSysEnums.ApproveStatus.Approved;
                payment.ApprovedBy = userName;
                payment.ApprovedOn = DateTime.Now;
            }
        }

        private static void SetApprovedWorking(Stakeholders stakeholders, string userName)
        {
            foreach (var working in stakeholders.StkhWorkings)
            {
                working.Status = ColloSysEnums.ApproveStatus.Approved;
                working.ApprovedBy = userName;
                working.ApprovedOn = DateTime.Now;
            }
        }

        public static Stakeholders Update(Stakeholders stakeholders)
        {
            stakeholders = SetStakeholder(stakeholders);
            StakeQuery.Save(stakeholders);
            LogManager.GetCurrentClassLogger().Info("StakeholderServices: Stakeholder Updated with ID: " + stakeholders.Id);
            return stakeholders;
        }

        private static Stakeholders SetStakeholder(Stakeholders stakeholders)
        {
            //set working
            SetWorking(stakeholders);
            //set payment
            SetPayment(stakeholders);

            //set working for payment
            SetPaymentWorking(stakeholders);
            //set registration
            SetRegistration(stakeholders);

            //set address
            SetGAddress(stakeholders);
            return stakeholders;
        }

        private static void SetGAddress(Stakeholders stakeholders)
        {
            foreach (var gAddress in stakeholders.GAddress)
            {
                gAddress.Country = "India";
                gAddress.Stakeholder = stakeholders;

            }
        }

        private static void SetRegistration(Stakeholders stakeholders)
        {
            if (stakeholders.StkhRegistrations.Any())
            {
                foreach (var stkhRegistration in stakeholders.StkhRegistrations)
                {
                    stkhRegistration.Stakeholder = stakeholders;
                    stkhRegistration.Description = stakeholders.Description;
                }
            }
        }

        private static void SetPayment(Stakeholders stakeholders)
        {
            if (stakeholders.StkhPayments.Any())
            {
                foreach (var stkhPayment in stakeholders.StkhPayments)
                {
                    stkhPayment.Stakeholder = stakeholders;
                    stkhPayment.Description = stakeholders.Description;
                }
            }
        }

        private static void SetPaymentWorking(Stakeholders stakeholders)
        {
            if (stakeholders.StkhPayments.Any())
            {
                foreach (var stkhPayment in stakeholders.StkhPayments)
                {
                    stkhPayment.StkhWorkings = stakeholders.StkhWorkings;
                }
            }
        }

        private static void SetWorking(Stakeholders stakeholders)
        {
            if (stakeholders.StkhWorkings.Any())
            {
                foreach (var gWorking in stakeholders.StkhWorkings)
                {
                    gWorking.Stakeholder = stakeholders;
                    gWorking.Description = stakeholders.Description;
                }
            }
        }
        #endregion

        public static IEnumerable<StkhHierarchy> GetHierarchy(string designation, string hierarchy)
        {
            var data = HierarchyQuery.OnDesignationHierarchy(designation, hierarchy);
            if (data.Any())
                LogManager.GetCurrentClassLogger().Info("StakeholderServices: Total Hierarchy loaded " + data.Count());
            return data;

        }
    }


}
//[HttpGet]
//[HttpTransaction]
//public IEnumerable<Stakeholders> GetInActiveData(Guid hierarchyId)
//{
//    if (hierarchyId == Guid.Empty)
//    {
//        return null;
//    }
//    var session = SessionManager.GetCurrentSession();
//    Stakeholders stake = null;
//    StkhHierarchy stkh = null;

//    //session.QueryOver<Stakeholders>()
//    //        .Fetch(x => x.Hierarchy).Eager
//    //        .Fetch(x => x.StkhPayments).Eager
//    //       .Where(x => x.Hierarchy.Hierarchy == hierarchy)
//    //       .Where(x => x.Hierarchy.Designation == designation && x.LeavingDate <= DateTime.Now)
//    //       .TransformUsing(Transformers.DistinctRootEntity)
//    //       .List();
//    var stakeholder = session.QueryOver<Stakeholders>(() => stake)
//                         .Fetch(x => x.Hierarchy).Eager
//                         .Fetch(x => x.StkhPayments).Eager
//                         .Fetch(x => x.StkhRegistrations).Eager
//                         .Fetch(x => x.StkhPayments).Eager
//                         .Fetch(x => x.StkhWorkings).Eager
//                         .JoinQueryOver(() => stake.Hierarchy, () => stkh)
//                         .Where(() => stkh.Id==hierarchyId && stake.LeavingDate <= DateTime.Now)
//                         .TransformUsing(Transformers.DistinctRootEntity)
//                         .List();
//    return stakeholder;

//}
//[HttpGet]
//[HttpTransaction]
//public IEnumerable<Stakeholders> GetActiveData(Guid hierarchyId)
//{
//    if (hierarchyId == Guid.Empty)
//    {
//        return null;
//    }
//    Stakeholders stake = null;
//    StkhHierarchy stkh = null;
//    var session = SessionManager.GetCurrentSession();

//    //var stake =
//    //    session.QueryOver<Stakeholders>()
//    //           .Fetch(x => x.Hierarchy).Eager
//    //           .Fetch(x => x.StkhPayments).Eager
//    //           .Where(x => x.Hierarchy.Hierarchy == hierarchy)
//    //           .Where(x => x.Hierarchy.Designation == designation)
//    //           .Where(x => x.LeavingDate >= DateTime.Now || x.LeavingDate == null)
//    //           .TransformUsing(Transformers.DistinctRootEntity)
//    //           .List();

//    //return stake;
//    var stakeholder = session.QueryOver<Stakeholders>(() => stake)
//                       .Fetch(x => x.Hierarchy).Eager
//                       .Fetch(x => x.StkhPayments).Eager
//                       .Fetch(x => x.StkhRegistrations).Eager
//                       .Fetch(x => x.StkhPayments).Eager
//                       .Fetch(x => x.StkhWorkings).Eager
//                       .JoinQueryOver(() => stake.Hierarchy, () => stkh)
//                       .Where(() => stkh.Id == hierarchyId)
//                       .TransformUsing(Transformers.DistinctRootEntity)
//                       .List();
//    return stakeholder;
//}