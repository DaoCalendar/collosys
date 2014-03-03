using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Generic;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.UserInterface.Areas.Stakeholder2.Models;
using ColloSys.UserInterface.Shared.Attributes;
using NHibernate.Linq;
using NHibernate.Transform;
using NLog;
using Newtonsoft.Json.Linq;

namespace ColloSys.UserInterface.Areas.Stakeholder2.api
{
    public class AddStakeHolderApiController : ApiController
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        [HttpGet]
        [HttpTransaction]
        public AddStakeholderModel GetAllHierarchies()
        {
            var stake = new AddStakeholderModel();

            using (var session = SessionManager.GetCurrentSession())
            {
                using (var trans = session.BeginTransaction())
                {
                    stake.ListOfStakeHierarchy = session.QueryOver<StkhHierarchy>().Where(x => x.Hierarchy != "Developer")
                          .List();

                    var gKeyValue = session.QueryOver<GKeyValue>()
                                  .Where(x => x.Area == ColloSysEnums.Activities.Stakeholder)
                                  .List<GKeyValue>();

                    stake.FixedPay = gKeyValue.ToDictionary(keyValue => keyValue.Key, keyValue => decimal.Parse(keyValue.Value));

                    trans.Rollback();
                    return stake;
                }
            }
        }

        [HttpPost]
        [HttpTransaction]
        public IEnumerable<Stakeholders> GetReportsToInHierarchy(StkhHierarchy reportsto)
        {
            var data = GetReportsToList(reportsto);
            if (data != null)
                _log.Info("Reports to list loaded in StakeholderApi/GetReportingList");
            return data;

        }

        [HttpPost]
        [HttpTransaction(Persist = true)]
        public HttpResponseMessage SaveStakeholder(FinalPostModel finalPost)
        {
            var stakeholders = finalPost.Stakeholder;
            var currUserId = HttpContext.Current.User.Identity.Name;
            var sessions = SessionManager.GetCurrentSession();
            var currUser =
                sessions.QueryOver<Stakeholders>()
                        .Where(x => x.ExternalId == currUserId).SingleOrDefault();

            if (currUser != null && currUser.ReportingManager != Guid.Empty)
            {
                var reportsToUserId =
                    sessions.QueryOver<Stakeholders>()
                            .Where(x => x.Id == currUser.ReportingManager).SingleOrDefault().ExternalId;
                stakeholders.ApprovedBy = reportsToUserId;
            }
            IList<StkhWorking> workingList = new List<StkhWorking>();

            if (finalPost.Hierarchy.HasWorking && finalPost.Hierarchy.HasPayment)
            {
                IList<StkhPayment> paymentList = new List<StkhPayment>();

                foreach (var payWorkModel in finalPost.PayWorkModelList)
                {
                    foreach (var stkhWorking in payWorkModel.WorkList)
                    {
                        stkhWorking.StkhPayment = payWorkModel.Payment;
                        workingList.Add(stkhWorking);
                    }
                    AssignBillingPolicies(payWorkModel);
                    payWorkModel.Payment.StkhWorkings = payWorkModel.WorkList;
                    payWorkModel.Payment.Stakeholder = finalPost.Stakeholder;
                    paymentList.Add(payWorkModel.Payment);
                }
                stakeholders.StkhWorkings = workingList;
                foreach (var working in stakeholders.StkhWorkings)
                {
                    working.ApprovedBy = stakeholders.ApprovedBy;
                }
                stakeholders.StkhPayments = paymentList;
            }

            if (finalPost.Hierarchy.HasWorking && !finalPost.Hierarchy.HasPayment)
            {
                var worklist = new List<StkhWorking>();
                foreach (var payWorkModel in finalPost.PayWorkModelList)
                {
                    foreach (var working in payWorkModel.WorkList)
                    {
                        worklist.Add(working);
                        stakeholders.StkhWorkings.Add(working);
                    }
                }
                stakeholders.StkhWorkings = worklist;
            }

            foreach (var working in stakeholders.StkhWorkings)
            {
                working.StartDate = finalPost.Stakeholder.JoiningDate;
                working.Stakeholder = finalPost.Stakeholder;
                working.ApprovedBy = stakeholders.ApprovedBy;
            }

            foreach (var payment in stakeholders.StkhPayments)
            {
                payment.StartDate = finalPost.Stakeholder.JoiningDate;
                payment.Stakeholder = finalPost.Stakeholder;
                payment.ApprovedBy = stakeholders.ApprovedBy;
            }

            foreach (var stakeAddress in stakeholders.GAddress)
            {
                stakeAddress.Stakeholder = stakeholders;
            }
            foreach (var stakereg in stakeholders.StkhRegistrations)
            {
                stakereg.Stakeholder = stakeholders;
            }

            if (finalPost.IsEditMode)
            {
                var session = SessionManager.GetCurrentSession();
                SetApprovalStatusEdit(stakeholders);
                session.Merge(stakeholders);
                _log.Info("Stakeholder is saved");
                var result =
                    Request.CreateResponse(HttpStatusCode.Created, stakeholders);

                return result;
            }

            var usersList = UsersIDList();

            if (stakeholders.ExternalId != null)
            {
                var isUserIdExist = usersList.Any(x => x == stakeholders.ExternalId);
                if (isUserIdExist)
                {
                    //var message = "User already registered with this UserId";
                    var obj = new JObject { { "Message", "User already registered with this UserId" } };

                    return Request.CreateResponse(HttpStatusCode.BadRequest, obj);
                }
            }



            try
            {
                //save stakeholder here
                if (DateTime.MinValue == stakeholders.BirthDate)
                    stakeholders.BirthDate = null;
                SetApprovalStatusInsert(stakeholders);
                Save(stakeholders);
                _log.Info("Stakeholder is saved in StakeholderApi/Save");
                var result =
                    Request.CreateResponse(HttpStatusCode.Created, stakeholders);

                return result;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                return Request.
                    CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        public void AssignBillingPolicies(PayWorkModel payWork)
        {
            var session = SessionManager.GetCurrentSession();
            var collectionPolicy =
                session.QueryOver<BillingPolicy>()
                       .Where(x => x.Id == payWork.CollectionBillingPolicyId)
                       .SingleOrDefault();
            payWork.Payment.CollectionBillingPolicy = collectionPolicy;

            var recoveryPolicy =
                session.QueryOver<BillingPolicy>()
                .Where(x => x.Id == payWork.RecoveryBillingPolicyId)
                .SingleOrDefault();
            payWork.Payment.RecoveryBillingPolicy = recoveryPolicy;

        }

        private static void SetApprovalStatusInsert(Stakeholders stakeholders)
        {
            stakeholders.Status = ColloSysEnums.ApproveStatus.Submitted;
            stakeholders.RowStatus = RowStatus.Insert;
            foreach (var stkhWorking in stakeholders.StkhWorkings)
            {
                stkhWorking.Status = ColloSysEnums.ApproveStatus.Submitted;
                stkhWorking.RowStatus = RowStatus.Insert;
            }
        }

        private static void SetApprovalStatusEdit(Stakeholders stakeholders)
        {
            stakeholders.Status = stakeholders.Status == ColloSysEnums.ApproveStatus.Approved ? ColloSysEnums.ApproveStatus.Approved : stakeholders.Status;
            stakeholders.RowStatus = RowStatus.Update;
            SetApprovalForWorkedit(stakeholders);
        }

        private static void SetApprovalForWorkedit(Stakeholders stakeholders)
        {
            foreach (var stkhWorking in stakeholders.StkhWorkings)
            {
                if (stkhWorking.Status == ColloSysEnums.ApproveStatus.NotApplicable)
                    stkhWorking.Status = ColloSysEnums.ApproveStatus.Submitted;
            }
        }
        [HttpGet]
        [HttpTransaction]
        public FinalPostModel GetStakeholderEditMode(Guid stakeholderId)
        {
            var stakeholder = GetStakeholder(stakeholderId);
            var getfinalPostModel = ConvertToFinalPostModel(stakeholder);
            return getfinalPostModel;
        }

        private static FinalPostModel ConvertToFinalPostModel(Stakeholders stakeholders)
        {
            var finalPostModel = new FinalPostModel();
            finalPostModel.Stakeholder = stakeholders;
            finalPostModel.Hierarchy = stakeholders.Hierarchy;

            //set address of stakeholder
            if (stakeholders.GAddress.Any())
            {
                finalPostModel.Address = stakeholders.GAddress[0];
                finalPostModel.Pincode = (uint)finalPostModel.Address.Pincode;
            }

            //set registration of stakeholder
            if (stakeholders.StkhRegistrations.Any())
            {
                finalPostModel.Registration = stakeholders.StkhRegistrations[0];
            }

            finalPostModel.EmailId = stakeholders.EmailId;
            finalPostModel.IsEditMode = true;
            // finalPostModel.LocationLevel = stakeholders.LocationLevel;
            finalPostModel.ReportsTo = GetStakeholder(stakeholders.ReportingManager);
            finalPostModel.ReportsToList = (IList<Stakeholders>)GetReportsToList(stakeholders.Hierarchy);
            finalPostModel.ReportsToList.Remove(stakeholders);

            if (stakeholders.StkhPayments.Any())
            {
                stakeholders.StkhPayments = stakeholders.StkhPayments.Distinct().ToList();
                foreach (var stkhPayment in stakeholders.StkhPayments)
                {
                    var payworkmodel = new PayWorkModel();
                    payworkmodel.Payment = stkhPayment;
                    payworkmodel.WorkList =
                        stakeholders.StkhWorkings.Where(x => x.StkhPayment.Id == stkhPayment.Id).ToList();
                    payworkmodel.WorkList = payworkmodel.WorkList.Distinct().ToList();
                    finalPostModel.PayWorkModelList.Add(payworkmodel);
                }

            }
            else if (stakeholders.StkhWorkings.Any() && stakeholders.StkhPayments.Count == 0)
            {
                var payworkmodel = new PayWorkModel();
                payworkmodel.WorkList = stakeholders.StkhWorkings;
                finalPostModel.PayWorkModelList.Add(payworkmodel);
                finalPostModel.PayWorkModel = payworkmodel;
                finalPostModel.PayWorkModel = finalPostModel.PayWorkModelList[0];
            }

            return finalPostModel;
        }

        private static Stakeholders GetStakeholder(Guid stakeholderId)
        {
            Stakeholders stakeholder = null;
            var stake2 = SessionManager.GetCurrentSession()
                                       .QueryOver<Stakeholders>(() => stakeholder)
                                       .Fetch(x => x.Hierarchy).Eager
                                       .Fetch(x => x.StkhRegistrations).Eager
                                       .Fetch(x => x.GAddress).Eager
                                       .Fetch(x => x.StkhPayments).Eager
                                       .Fetch(x => x.StkhWorkings).Eager
                                       .Fetch(x => x.ApprovedBy).Eager
                                       .Where(() => stakeholder.Id == stakeholderId)
                                       .TransformUsing(Transformers.DistinctRootEntity)
                                       .List()
                                       .FirstOrDefault();
            if (stake2 != null)
            {
                stake2.StkhWorkings = (from d in stake2.StkhWorkings
                                       where d.Status != ColloSysEnums.ApproveStatus.Rejected
                                       select d).ToList();
                stake2.StkhPayments = (from d in stake2.StkhPayments
                                       where d.Status != ColloSysEnums.ApproveStatus.Rejected
                                       select d).ToList();
                var deletelist = (from d in stake2.StkhWorkings
                                  where d.Status == ColloSysEnums.ApproveStatus.Approved &&
                                        d.RowStatus == RowStatus.Delete
                                  select d).ToList();
                var deletepayment = (from d in stake2.StkhPayments
                                     where d.Status == ColloSysEnums.ApproveStatus.Approved &&
                                           d.RowStatus == RowStatus.Delete
                                     select d).ToList();
                deletelist.ForEach(x => stake2.StkhWorkings.Remove(x));
                deletepayment.ForEach(x => stake2.StkhPayments.Remove(x));
            }
            return stake2;
        }

        private static IEnumerable<string> UsersIDList()
        {
            var session = SessionManager.GetCurrentSession();
            var data = session.Query<Stakeholders>()
                              .Where(x => x.ExternalId != string.Empty)
                              .Select(x => x.ExternalId).ToList();

            LogManager.GetCurrentClassLogger().Info("StakeholderServices: UsersIDList count: " + data.Count);

            return data;
        }

        private static void Save(Stakeholders stakeholders)
        {
            var session = SessionManager.GetCurrentSession();
            session.SaveOrUpdate(stakeholders);
        }

        private static IEnumerable<Stakeholders> GetReportsToList(StkhHierarchy currentHierarchy)
        {
            Guid reportingHierarchy = currentHierarchy.ReportsTo;
            var session = SessionManager.GetCurrentSession();
            var list = new List<Stakeholders>();

            if (reportingHierarchy == Guid.Empty)
            {
                return list;
            }
            var firstLevelHierarchy = session.QueryOver<StkhHierarchy>()
                                             .Where(x => x.Id == reportingHierarchy)
                                             .SingleOrDefault();
            if (firstLevelHierarchy == null)
            {
                return list;
            }
            if (currentHierarchy.ReportingLevel == ColloSysEnums.ReportingLevel.AllLevels)
            {
                list = session.Query<Stakeholders>().ToList();
                return list;
            }
            var firstlevelData = session.Query<Stakeholders>()
                                        .Fetch(x => x.Hierarchy)
                                        .Fetch(x => x.StkhWorkings)
                                        .Where(x => x.Hierarchy.Id == reportingHierarchy &&
                                                    (x.LeavingDate < DateTime.Now || x.LeavingDate == null))
                                        .ToList();
            list.AddRange(firstlevelData);
            if (currentHierarchy.ReportingLevel == ColloSysEnums.ReportingLevel.OneLevelUp)
            {
                return list;
            }

            var secondLevelHierarchy = session.QueryOver<StkhHierarchy>()
                                              .Where(x => x.Id == firstLevelHierarchy.ReportsTo)
                                              .SingleOrDefault();
            if (secondLevelHierarchy == null)
            {
                return list;
            }
            var secondLevelData = session.Query<Stakeholders>()
                                         .Fetch(x => x.Hierarchy)
                                         .Fetch(x => x.StkhWorkings)
                                         .Where(x => x.Hierarchy.Id == secondLevelHierarchy.Id &&
                                                     (x.LeavingDate < DateTime.Now || x.LeavingDate == null))
                                         .ToList();

            list.AddRange(secondLevelData);
            if (currentHierarchy.ReportingLevel == ColloSysEnums.ReportingLevel.TwoLevelUp)
            {
                return list;
            }

            return list;
            //    var data = session.Query<Stakeholders>()
            //    //.Fetch(x => x.StkhPayments)
            //                  .Fetch(x => x.Hierarchy)
            //                  .Where(
            //                      x =>
            //                      x.Hierarchy.Id == reportingHierarchy &&
            //                      (x.LeavingDate < DateTime.Now || x.LeavingDate == null))
            //                  .Select(x => x)
            //    //.OrderByDescending(x => x.StkhPayments.First(y => y.StartDate < DateTime.Now && y.EndDate > DateTime.Now))
            //                  .ToList();

            //if (data.Any() && (data.First().ReportingManager != Guid.Empty))
            //{
            //    var reporttoId = data[0].ReportingManager;
            //    if (reporttoId != Guid.Empty)
            //    {
            //        var stakeholder = session.Query<Stakeholders>()
            //                                 .Fetch(x => x.StkhPayments)
            //                                 .Fetch(x => x.Hierarchy)
            //                                 .Where(x => x.Id == reporttoId)
            //                                 .Select(x => x.Hierarchy.Id).Single();


            //        var onelevelupperlist = session.Query<Stakeholders>()
            //                                       .Fetch(x => x.Hierarchy)
            //                                       .Where(
            //                                           x =>
            //                                           x.Hierarchy.Id == stakeholder &&
            //                                           (x.LeavingDate < DateTime.Now || x.LeavingDate == null))
            //                                       .Select(x => x).ToList();
            //        data.AddRange(onelevelupperlist);
            //    }
            //}

            //LogManager.GetCurrentClassLogger()
            //           .Info("StakeholderServices: Total Count for ReportsTo List " + data.Count());
            //data = (from d in data
            //        where d.Status != ColloSysEnums.ApproveStatus.Rejected
            //        select d).ToList();

            //return data;
        }
    }


}
