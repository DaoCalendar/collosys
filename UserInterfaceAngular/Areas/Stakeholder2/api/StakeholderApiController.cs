#region references
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.QueryBuilder.GenericBuilder;
using ColloSys.QueryBuilder.StakeholderBuilder;
using ColloSys.UserInterface.Areas.Stakeholder2.Models;
using ColloSys.UserInterface.Shared;
using ColloSys.UserInterface.Shared.Attributes;
using NHibernate;
using NHibernate.Linq;
using NLog;
using Newtonsoft.Json.Linq;

#endregion

//stakeholders calls changed
//hierarchy calls changed
//namespace UserInterfaceAngular.app
namespace ColloSys.UserInterface.Areas.Stakeholder2.api
{
    public class StakeholderApiController : BaseApiController<Stakeholders>
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        private static readonly HierarchyQueryBuilder HierarchyQuery = new HierarchyQueryBuilder();
        private static readonly StakeQueryBuilder StakeQuery = new StakeQueryBuilder();
        private static readonly AddressQueryBuilder AddressQuery = new AddressQueryBuilder();
        private static readonly StakePaymentQueryBuilder StakePaymentBuilder = new StakePaymentQueryBuilder();
        private static readonly StakeWorkingQueryBuilder StakeWorkingBuilder = new StakeWorkingQueryBuilder();
        private static readonly ProductConfigBuilder ProductConfigBuilder = new ProductConfigBuilder();
        private static readonly GPincodeBuilder GPincodeBuilder = new GPincodeBuilder();

        [HttpGet]
        [HttpTransaction]
        public IEnumerable<StkhHierarchy> GetAllHierarchies()
        {
            return HierarchyQuery.FilterBy(x => x.Hierarchy != "Developer");
        }

        [HttpGet]
        [HttpTransaction]
        public bool UserIdVal(string userid)
        {
            var listOfuserId = StakeQuery.FilterBy(x => x.ExternalId == userid).ToList();

            return listOfuserId.Count != 0;
        }

        [HttpGet]
        [HttpTransaction]
        public IEnumerable<string> UserIdList()
        {
            return StakeQuery.GetAll().Select(x => x.ExternalId).As<List<String>>();
        }

        [HttpGet]
        [HttpTransaction]
        public bool CheckUserId(string id)
        {
            var idExists = StakeQuery.FilterBy(x => x.ExternalId == id);
            return (idExists.Count > 0);
        }


        [HttpGet]
        [HttpTransaction]
        public IEnumerable<ScbEnums.Products> GetProducts()
        {
            return ProductConfigBuilder.GetProducts();
        }

        [HttpGet]
        public IEnumerable<string> VariableLinerPolicies()
        {
            return new[] { "Collection Policy1", "Collection Policy2", "Collection Policy3" };
        }

        [HttpGet]
        public IEnumerable<string> VariableWriteoffPolicies()
        {
            return new[] { "Recovery Policy1", "Recovery Policy2", "Recovery Policy3" };
        }

        [HttpGet]
        public IEnumerable<int> GetBucketList()
        {
            _log.Info("In getBucket list");
            return new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        }

        [HttpGet]
        [HttpTransaction]
        public StkhHierarchy GetHierarchyWithId(Guid hierarchyId)
        {
            _log.Info("In load hierarchy with id");
            var data = HierarchyQuery.FilterBy(x => x.Id == hierarchyId)
                              .Select(x => x);
            return data.First();
        }

        [HttpGet]
        [HttpTransaction]
        public IEnumerable<string> GetStateList()
        {
            return GPincodeBuilder.StateList();
        }

        [HttpGet]
        [HttpTransaction]
        public IEnumerable<GPincode> GetPincodes(string pincode, string level)
        {
            return level == "City" ? GetPincodesCity(pincode) : GetPincodesArea(pincode);
        }

        [HttpGet]
        [HttpTransaction]
        public IEnumerable<GPincode> GetClusters(string cluster)
        {
            return GetClusterList(cluster);
        }
        [HttpGet]
        [HttpTransaction]
        public string GetRegionOfState(string state)
        {
            var data = GetRegionOnState(state);
            return data;
        }

        [HttpGet]
        [HttpTransaction]
        public IEnumerable<Stakeholders> GetReportingList(string reportsTo, string hierarchy)
        {
            var data = GetReportsToList(reportsTo, hierarchy);
            if (data != null)
                _log.Info("Reports to list loaded in StakeholderApi/GetReportingList");
            return data;
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
                //if (DateTime.MinValue == stakeholders.BirthDate)
                    //stakeholders.BirthDate = null;
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

        [HttpPost]
        [HttpTransaction(Persist = true)]
        public HttpResponseMessage DeleteLists(IList<ListValue> list)
        {
            _log.Info("In StakeholderApi/DeleteLists");
            try
            {
                if (ModelState.IsValid)
                {
                    var paymentlist = ListValueConvert.ConvertList(list, "Payment");
                    var addresslist = ListValueConvert.ConvertList(list, "Address");
                    var workinglist = ListValueConvert.ConvertList(list, "Working");
                    DeleteList(addresslist, paymentlist, workinglist);
                    var result =
                        Request.CreateResponse(HttpStatusCode.Created);

                    return result;
                }
                return Request.
                    CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                return Request.
                    CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        [HttpGet]
        [HttpTransaction]
        public IEnumerable<string> ListOfUserID()
        {
            var data = UsersIDList();
            if (data != null)
                _log.Info("List of user id loaded in Stakeholder/ListOfUserID");
            return data;
        }

        #region

        private static IEnumerable<GPincode> GetClusterList(string cluster)
        {
            try
            {
                var list = GPincodeBuilder.OnCluster(cluster);
                if (list.Count == 0) return null;

                return list.OrderBy(x => x.Key).Take(10).Select(entry => entry.First()).ToList();
            }
            catch (HibernateException exception)
            {
                LogManager.GetCurrentClassLogger().InfoException("Error while fetching Pincode list.", exception);
                throw;
            }
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

        private static string GetRegionOnState(string state)
        {
            return GPincodeBuilder.RegionOnState(state);
        }

        private static IEnumerable<Stakeholders> GetReportsToList(string reportsTo, string hierarchy)
        {
            var data = StakeQuery.FilterBy(x => x.Hierarchy.Designation == reportsTo && x.Hierarchy.Hierarchy == hierarchy)
                              .Select(x => x).ToList();

            LogManager.GetCurrentClassLogger()
                      .Info("StakeholderServices: Total Count for ReportsTo List " + data.Count());

            return data;
        }

        private static IEnumerable<Stakeholders> GetReportsToList(Guid hierarchyId)
        {
            var data = StakeQuery.OnHieararchyIdWithPayments(hierarchyId).ToList();

            if (data.Any() && (data.First().ReportingManager != Guid.Empty))
            {
                var reporttoId = data[0].ReportingManager;
                if (reporttoId != Guid.Empty)
                {
                    var stakeholder = StakeQuery.OnIdWithAllReferences(reporttoId).Hierarchy.Id;

                    var onelevelupperlist = StakeQuery.OnHierarchyId(stakeholder).ToList();
                    data.AddRange(onelevelupperlist);
                }
            }
            LogManager.GetCurrentClassLogger()
                       .Info("StakeholderServices: Total Count for ReportsTo List " + data.Count());

            return data;
        }

        private static void DeleteList(IEnumerable<Guid> addresslist, IEnumerable<Guid> paymentlist, IEnumerable<Guid> working)
        {
            foreach (var guid in addresslist)
            {
                AddressQuery.Delete(AddressQuery.Load(guid));
            }
            foreach (var guid in paymentlist)
            {
                StakePaymentBuilder.Delete(StakePaymentBuilder.Load(guid));
            }
            foreach (var guid in working)
            {
                StakeWorkingBuilder.Delete(StakeWorkingBuilder.Load(guid));
            }
        }

        private static IEnumerable<string> UsersIDList()
        {
            return StakeQuery.GetAll().Select(x => x.ExternalId).ToList();
        }

        #region save

        private static void Save(Stakeholders stakeholders)
        {
            stakeholders = SetStakeholder(stakeholders);
            StakeQuery.Save(stakeholders);
            if (stakeholders.GAddress.Any())
            {
                var listOfAddresses = SetGAddress(stakeholders);
                foreach (var gAddress in listOfAddresses)
                {
                    AddressQuery.Save(gAddress);
                }
            }
        }

        private static IEnumerable<StakeAddress> SetGAddress(Stakeholders stakeholders)
        {
            var gAddresses = stakeholders.GAddress;
            foreach (var gAddress in gAddresses)
            {
                //gAddress.Source = "Stakeholder";
                gAddress.Country = "India";
                // gAddress.SourceId = stakeholders.Id;
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

        #endregion

        #endregion
    }
}



//var hiegherhierarchyId = session.Query<StakeHierarchy>()
//                         .Where(x => x.ReportsTo == reporttoId)
//                         .Select(x => x.Id).Single();

//Stakeholders stake = null;
//StkhPayment payment = null;
//var listStake = session.QueryOver<Stakeholders>(() => stake)
//                       .Fetch(x => x.StkhPayments).Eager
//                       .JoinAlias(() => stake.StkhPayments, () => payment, JoinType.LeftOuterJoin)
//                       .Where(() => stake.HierarchyId == stakeholder)
//                       .And(() => stake.LeavingDate == null || stake.LeavingDate < DateTime.Now)
//                       //.And(() => payment.StartDate < DateTime.Now)
//                       //.And(() => payment.EndDate > DateTime.Now)
//                       .List();

