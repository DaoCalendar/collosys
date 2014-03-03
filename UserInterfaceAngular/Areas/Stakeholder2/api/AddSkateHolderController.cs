using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Generic;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.UserInterface.Areas.Stakeholder2.Models;
using ColloSys.UserInterface.Shared.Attributes;
using NHibernate.Linq;
using NLog;
using Newtonsoft.Json.Linq;

namespace ColloSys.UserInterface.Areas.Stakeholder2.api

{
    public class AddSkateHolderController : ApiController
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

        [HttpGet]
        [HttpTransaction]
        public IEnumerable<Stakeholders> GetReportsToInHierarchy(Guid reportsto)
        {
            var data = GetReportsToList(reportsto);
            if (data != null)
                _log.Info("Reports to list loaded in StakeholderApi/GetReportingList");
            return data;

        }

        [HttpPost]
        [HttpTransaction(Persist = true)]
        public HttpResponseMessage SaveStakeholder(Stakeholders stakeholders)
        {
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

        private static IEnumerable<string> UsersIDList()
        {
            var session = SessionManager.GetCurrentSession();
            var data = session.Query<Stakeholders>()
                              .Select(x => x.ExternalId).ToList();

            LogManager.GetCurrentClassLogger().Info("StakeholderServices: UsersIDList count: " + data.Count);

            return data;
        }

        private static void Save(Stakeholders stakeholders)
        {
            stakeholders = SetStakeholder(stakeholders);
            var session = SessionManager.GetCurrentSession();
            session.SaveOrUpdate(stakeholders);
            if (stakeholders.GAddress.Any())
            {
                var listOfAddresses = SetGAddress(stakeholders);
                foreach (var gAddress in listOfAddresses)
                {
                    session.SaveOrUpdate(gAddress);
                }
            }
        }

        private static IEnumerable<StakeAddress> SetGAddress(Stakeholders stakeholders)
        {
            var gAddresses = stakeholders.GAddress;
            foreach (var gAddress in gAddresses)
            {
                gAddress.Country = "India";
            }
            return gAddresses;
        }

        private static Stakeholders SetStakeholder(Stakeholders stakeholders)
        {
            //set payment
            SetPayment(stakeholders);
            //set working
            SetWorking(stakeholders);
            //set registration
            SetRegistration(stakeholders);
            return stakeholders;
        }

        private static void SetRegistration(Stakeholders stakeholders)
        {
            if (stakeholders.StkhRegistrations.Any())
            {
                foreach (var stkhRegistration in stakeholders.StkhRegistrations)
                {
                    stkhRegistration.Stakeholder = stakeholders;
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
                }
            }
        }

        private static IEnumerable<Stakeholders> GetReportsToList(Guid hierarchyId)
        {
            var session = SessionManager.GetCurrentSession();

            var data = session.Query<Stakeholders>()
                              .Fetch(x => x.StkhPayments)
                              .Where(
                                  x =>
                                  x.HierarchyId == hierarchyId &&
                                  (x.LeavingDate < DateTime.Now || x.LeavingDate == null))
                              .Select(x => x)
                              .OrderByDescending(x => x.StkhPayments.First(y => y.StartDate < DateTime.Now && y.EndDate > DateTime.Now))
                              .ToList();

            if (data.Any() && (data.First().ReportingManager != Guid.Empty))
            {
                var reporttoId = data[0].ReportingManager;
                if (reporttoId != Guid.Empty)
                {
                    var stakeholder = session.Query<Stakeholders>()
                                             .Fetch(x => x.StkhPayments)
                                             .Where(x => x.Id == reporttoId)
                                             .Select(x => x.HierarchyId).Single();


                    var onelevelupperlist = session.Query<Stakeholders>()
                                                   .Where(
                                                       x =>
                                                       x.HierarchyId == stakeholder &&
                                                       (x.LeavingDate < DateTime.Now || x.LeavingDate == null))
                                                   .Select(x => x).ToList();
                    data.AddRange(onelevelupperlist);
                }
            }
            LogManager.GetCurrentClassLogger()
                       .Info("StakeholderServices: Total Count for ReportsTo List " + data.Count());

            return data;
        }
    }
}
