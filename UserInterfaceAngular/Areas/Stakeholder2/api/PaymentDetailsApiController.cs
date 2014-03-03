using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;
using System.Web.Services.Description;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Generic;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.UserInterface.Areas.Stakeholder2.Models;
using ColloSys.UserInterface.Areas.Stakeholder2.ViewModels;
using ColloSys.UserInterface.Shared.Attributes;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using NLog;
using Newtonsoft.Json.Linq;

namespace ColloSys.UserInterface.Areas.Stakeholder2.api
{
    public class PaymentDetailsApiController : ApiController
    {


        [HttpPost]
        [HttpTransaction]
        public WorkingModel GetPincodeData(WorkingModel pindata)
        {
            switch (pindata.QueryFor)
            {
                case LocationLevels.State:
                    pindata.SelectedPincodeData.State = null;
                    pindata.SelectedPincodeData.Cluster = null;
                    pindata.SelectedPincodeData.District = null;
                    pindata.SelectedPincodeData.City = null;
                    pindata.SelectedPincodeData.Area = null;
                    break;
                case LocationLevels.Cluster:
                    pindata.SelectedPincodeData.Cluster = null;
                    pindata.SelectedPincodeData.District = null;
                    pindata.SelectedPincodeData.City = null;
                    pindata.SelectedPincodeData.Area = null;
                    break;
                case LocationLevels.District:
                    pindata.SelectedPincodeData.District = null;
                    pindata.SelectedPincodeData.City = null;
                    pindata.SelectedPincodeData.Area = null;
                    break;
                case LocationLevels.City:
                    pindata.SelectedPincodeData.City = null;
                    pindata.SelectedPincodeData.Area = null;
                    break;
                case LocationLevels.Area:
                    pindata.SelectedPincodeData.Area = null;
                    break;

            }
            pindata.SetWorkingList(pindata);
            return pindata;
        }
        [HttpPost]
        [HttpTransaction]
        public WorkingModel GetPincodeList(WorkingModel pindata)
        {
            if (pindata.QueryFor == LocationLevels.City)
                pindata.QueryFor = LocationLevels.District;

            pindata.GetGPincodeData(pindata);
            return pindata;
        }
        [HttpGet]
        [HttpTransaction]
        public HttpResponseMessage GetAllWorkingList()
        {
            var allLists = new
            {
                Products = GetProductList().ToList(),
                //States = GetStateList().ToList(),
                //Region = GetRegionList().ToList()
            };
            return Request.CreateResponse(HttpStatusCode.OK, allLists);

        }

        [HttpGet]
        [HttpTransaction]
        public BillingPolicyLists GetLinerWriteOff(ScbEnums.Products product)
        {
            var session = SessionManager.GetCurrentSession();
            var data = new BillingPolicyLists
                {
                    LinerList = session.QueryOver<BillingPolicy>()
                                       .Where(x => x.Products == product && x.Category == ScbEnums.Category.Liner)
                                       .List(),
                    WriteOffList = session.QueryOver<BillingPolicy>()
                    .Where(x => x.Products == product && x.Category == ScbEnums.Category.WriteOff)
                    .List()
                };
            return data;
        }


        public class BillingPolicyLists
        {
            public IList<BillingPolicy> LinerList { get; set; }
            public IList<BillingPolicy> WriteOffList { get; set; }
        }

        [HttpGet]
        [HttpTransaction]
        public PaymentDetails GetPaymentDetails()
        {
            var session = SessionManager.GetCurrentSession();

            var payment = new PaymentDetails();

            //payment.ListOfStakeHierarchy = session.QueryOver<StkhHierarchy>()
            //              .Where(x => x.Hierarchy != "Developer")
            //              .List();

            var gKeyValue = session.QueryOver<GKeyValue>()
                                   .Where(x => x.Area == ColloSysEnums.Activities.Stakeholder)
                                   .List<GKeyValue>();

            payment.FixedPay = gKeyValue.ToDictionary(keyValue => keyValue.Key, keyValue => decimal.Parse(keyValue.Value));

            return payment;
        }

        public IEnumerable<string> GetProductList()
        {
            return Enum.GetNames(typeof(ScbEnums.Products)).Where(x => x != "UNKNOWN")
                .ToList();
        }

        [HttpGet]
        [HttpTransaction]
        public IEnumerable<string> GetStateList()
        {
            var session = SessionManager.GetCurrentSession();
            var getStateList = session.Query<GPincode>()
                              .Select(x => x.State)
                              .Distinct().ToList();
            return getStateList;
        }

        [HttpGet]
        [HttpTransaction]
        public IEnumerable<GPincode> GetCompleteClusterData()
        {
            var session = SessionManager.GetCurrentSession();
            var cluster = session.QueryOver<GPincode>().List();
            return cluster;
        }

        public IEnumerable<RegionState> GetRegionList()
        {
            var regionList = new List<RegionState>();
            var session = SessionManager.GetCurrentSession();

            var states =
                session.Query<GPincode>().Select(x => x.State).Distinct().ToList();

            foreach (var state in states)
            {
                var reg = session.Query<GPincode>().FirstOrDefault(x => x.State == state);
                if (reg != null) regionList.Add(new RegionState { Region = reg.Region, State = state });
            }

            //var data = session.QueryOver<GPincode>().List();
            //var regionState = (from d in data
            //                   select new RegionState { Region = d.Region, State = d.State })
            //                   .Distinct(new RegionState.Comparer()).ToList();
            return regionList;

        }

        public class RegionState
        {
            public string Region { get; set; }
            public string State { get; set; }

            public class Comparer : IEqualityComparer<RegionState>
            {
                public bool Equals(RegionState x, RegionState y)
                {
                    return x.State.Trim().ToUpperInvariant() == y.State.Trim().ToUpperInvariant();
                }

                public int GetHashCode(RegionState obj)
                {
                    unchecked
                    {
                        return obj.State.Trim().ToUpperInvariant().GetHashCode();
                    }
                }
            }
        }


        [HttpGet]
        [HttpTransaction]
        public IEnumerable<GPincode> GetClusters(Guid Id)
        {
            //if (state == null)
            //{
            //    return null;
            //}
            Stakeholders stakeholders = null;
            StkhWorking working = null;
            StkhHierarchy hierarchy = null;
            var session = SessionManager.GetCurrentSession();
            var stk2 = session.QueryOver<Stakeholders>(() => stakeholders)
                              .Fetch(x => x.Hierarchy).Eager
                              .Fetch(x => x.StkhWorkings).Eager
                              .JoinQueryOver(() => stakeholders.StkhWorkings, () => working, JoinType.LeftOuterJoin)
                              .JoinQueryOver(() => stakeholders.Hierarchy, () => hierarchy, JoinType.LeftOuterJoin)
                              .Where(() => stakeholders.Id == Id)
                              .SingleOrDefault();
            var list = new List<string>();
            var criteria = session.CreateCriteria(typeof(GPincode));
            JavaScriptSerializer ser = new JavaScriptSerializer();
            var locationLevel = ser.Deserialize(stk2.Hierarchy.LocationLevel, typeof(List<string>))
                                   .As<List<string>>()
                                   .First();

            switch (locationLevel)
            {
                case "Region":
                    list = (from d in stk2.StkhWorkings
                            select d.Region).ToList();
                    criteria.Add(Restrictions.In("Region", list));
                    break;
                case "State":
                    list = (from d in stk2.StkhWorkings
                            select d.Region).ToList();
                    criteria.Add(Restrictions.In("Region", list));
                    break;
                case "City":
                    list = (from d in stk2.StkhWorkings
                            select d.City).ToList();
                    criteria.Add(Restrictions.In("City", list));
                    break;
                case "Cluster":
                    list = (from d in stk2.StkhWorkings
                            select d.Cluster).ToList();
                    criteria.Add(Restrictions.In("Cluster", list));
                    break;
            }
            var gpincodeData = criteria.List<GPincode>();
            //var stake = session.QueryOver<Stakeholders>().Where(x => x.Id == Id).SingleOrDefault();
            //var location = session.QueryOver<StkhWorking>()
            //                      .Where(x => x.Stakeholder.Id == stake.Id)
            //                      .SingleOrDefault().State;
            //var cluster = session.Query<GPincode>()
            //                     .Where(x => x.State == location)
            //                     .Distinct().ToList();
            return gpincodeData;
        }

        [HttpPost]
        [HttpTransaction]
        public IEnumerable<Stakeholders> WorkingReportsTo(StkhHierarchy hierarchyId)
        {
            //var session = SessionManager.GetCurrentSession();
            //Stakeholders stake = null;
            //StkhWorking stkh = null;
            //StkhHierarchy hir = null;
            //var reporteeList = session.QueryOver<Stakeholders>(() => stake)
            //                   .Fetch(x => x.Hierarchy).Eager
            //                   .Fetch(x => x.StkhPayments).Eager
            //                   .Fetch(x => x.StkhRegistrations).Eager
            //                   .Fetch(x => x.StkhPayments).Eager
            //                   .Fetch(x => x.StkhWorkings).Eager
            //                   .Fetch(x => x.GAddress).Eager
            //                   .JoinQueryOver(() => stake.StkhWorkings, () => stkh)
            //                   .JoinQueryOver(() => stake.Hierarchy, () => hir)
            //                   .Where(() => stkh.Products == product)
            //                   .Where(() => hir.WorkingReportsTo == hierarchyId)
            //                   .TransformUsing(Transformers.DistinctRootEntity)
            //                   .List();
            //return reporteeList;
            Guid reportingHierarchy = hierarchyId.WorkingReportsTo;
            var session = SessionManager.GetCurrentSession();
            var reportsTolist = new List<Stakeholders>();

            if (reportingHierarchy == Guid.Empty)
            {
                return reportsTolist;
            }
            var firstLevelHierarchy = session.QueryOver<StkhHierarchy>()
                                             .Where(x => x.Id == reportingHierarchy)
                                             .SingleOrDefault();
            if (firstLevelHierarchy == null)
            {
                return reportsTolist;
            }
            if (hierarchyId.WorkingReportsLevel == ColloSysEnums.ReportingLevel.AllLevels)
            {
                reportsTolist = session.Query<Stakeholders>().ToList();
                return reportsTolist;
            }
            var firstlevelData = session.Query<Stakeholders>()
                                        .Fetch(x => x.Hierarchy)
                                        .Fetch(x => x.StkhWorkings)
                                        .Where(x => x.Hierarchy.Id == reportingHierarchy &&
                                                    (x.LeavingDate < DateTime.Now || x.LeavingDate == null))
                                        .ToList();
            reportsTolist.AddRange(firstlevelData);
            if (hierarchyId.WorkingReportsLevel == ColloSysEnums.ReportingLevel.OneLevelUp)
            {
                return reportsTolist;
            }

            var secondLevelHierarchy = session.QueryOver<StkhHierarchy>()
                                              .Where(x => x.Id == firstLevelHierarchy.WorkingReportsTo)
                                              .SingleOrDefault();
            if (secondLevelHierarchy == null)
            {
                return reportsTolist;
            }
            var secondLevelData = session.Query<Stakeholders>()
                                         .Fetch(x => x.Hierarchy)
                                         .Fetch(x => x.StkhWorkings)
                                         .Where(x => x.Hierarchy.Id == secondLevelHierarchy.Id &&
                                                     (x.LeavingDate < DateTime.Now || x.LeavingDate == null))
                                         .ToList();

            reportsTolist.AddRange(secondLevelData);
            if (hierarchyId.WorkingReportsLevel == ColloSysEnums.ReportingLevel.TwoLevelUp)
            {
                return reportsTolist;
            }

            return reportsTolist;
        }


        [HttpGet]
        [HttpTransaction]
        public IEnumerable<Stakeholders> StakeHolderList(string product, Guid selHierarchyId)
        {
            var eProduct = (ScbEnums.Products)Enum.Parse(typeof(ScbEnums.Products), product);

            using (var session = SessionManager.GetCurrentSession())
            {
                Stakeholders stake2 = null;

                //Get Hierarchy by select hierarchy
                var hierarchy = session.QueryOver<StkhHierarchy>()
                                  .Where(x => x.Id == selHierarchyId).SingleOrDefault();

                // Find reporting hierarchy ReportsToId
                var reportingHierarchy =
                    session.QueryOver<StkhHierarchy>().Where(x => x.Id == hierarchy.ReportsTo).SingleOrDefault();
                StkhHierarchy hierarchy2 = null;

                // list of reporting hierarchyId
                var reportingIdlist = session.QueryOver<Stakeholders>(() => stake2)
                                    .Fetch(x => x.Hierarchy).Eager
                                    .JoinQueryOver(() => stake2.Hierarchy, () => hierarchy2)
                                    .Where(() => hierarchy2.Id == reportingHierarchy.Id)
                                    .List();

                var hlist = session.QueryOver<StkhHierarchy>().Where(x => x.Id == reportingHierarchy.ReportsTo).SingleOrDefault();
                if (hlist == null)
                    return reportingIdlist;

                StkhHierarchy hierarchy3 = null;
                var hlist4 = session.QueryOver<Stakeholders>(() => stake2)
                                   .Fetch(x => x.Hierarchy).Eager
                                   .JoinQueryOver(() => stake2.Hierarchy, () => hierarchy3)
                                   .Where(() => hierarchy3.Id == hlist.Id)
                                   .List();

                var slist = session.QueryOver<Stakeholders>().List();

                var listMarge = new List<Stakeholders>();
                listMarge.AddRange(reportingIdlist);
                listMarge.AddRange(hlist4);

                //product base
                //return listMarge.Where(x=>x.StkhWorkings.First().Products == eProduct);

                return listMarge;
            }
        }

        //[HttpGet]
        //[HttpTransaction]
        //public IEnumerable<GPincode> GetClusters(string cluster)
        //{
        //    return GetClusterList(cluster);
        //}

        [HttpGet]
        [HttpTransaction]
        public string GetRegionOfState(string state)
        {
            var data = GetRegionOnState(state);
            return data;
        }

        [HttpGet]
        [HttpTransaction]
        public IEnumerable<GPincode> GetPincodes(string pincode, string level)
        {
            return level == "City" ? GetPincodesCity(pincode) : GetPincodesArea(pincode);
        }

        #region

        private static IEnumerable<GPincode> GetClusterList(string cluster)
        {
            var session = SessionManager.GetCurrentSession();
            try
            {
                var list = session.Query<GPincode>()
                                  .Where(x => x.Cluster.StartsWith(cluster))
                                  .GroupBy(x => x.Cluster)
                                  .ToList();
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
            var session = SessionManager.GetCurrentSession();
            var list = session.Query<GPincode>()
                              .Where(x => x.Pincode.ToString().StartsWith(pin) || x.City.StartsWith(pin))
                              .Take(100)
                              .ToList();
            if (list.Count == 0) return null;

            var uniq = (from l in list group l by l.City into g select g.First()).ToList();
            uniq.RemoveAll(x => x.City.Trim().Equals("-"));
            return uniq.Take(10);
        }

        private static IEnumerable<GPincode> GetPincodesArea(string pin)
        {
            var session = SessionManager.GetCurrentSession();
            var list = session.Query<GPincode>()
                              .Where(x => x.Pincode.ToString().StartsWith(pin) || x.Area.StartsWith(pin))
                              .Select(x => x)
                              .Take(100).ToList();
            if (list.Count == 0) return null;

            var uniq = (from l in list group l by l.Area into g select g.First()).ToList();
            uniq.RemoveAll(x => x.City.Trim().Equals("-"));
            return uniq.Take(10);
        }

        private static string GetRegionOnState(string state)
        {
            var session = SessionManager.GetCurrentSession();
            var list = session.Query<GPincode>()
                              .Where(x => x.State == state)
                              .Select(x => x.Region)
                              .Distinct()
                              .FirstOrDefault();
            return list;
        }

        #endregion


        public IEnumerable<RegionState> list { get; set; }
    }

    public class PaymentDetails
    {
        public List<string> LinerPolicy { get { return Enum.GetNames(typeof(LinerPolicies)).ToList(); } }

        public List<string> WriteOfPolicy { get { return Enum.GetNames(typeof(WriteoffPolicies)).ToList(); } }

        //public IList<StkhHierarchy> ListOfStakeHierarchy { get; set; }

        public IDictionary<string, decimal> FixedPay { get; set; }

        public enum LinerPolicies
        {
            Collection_Policy1,
            Collection_Policy2,
            Collection_Policy3
        }

        public enum WriteoffPolicies
        {
            Recovery_Policy1,
            Recovery_Policy2,
            Recovery_Policy3
        }
    }
}
