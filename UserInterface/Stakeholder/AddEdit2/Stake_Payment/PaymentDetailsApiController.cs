using ColloSys.DataLayer.SessionMgr;

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
using ColloSys.QueryBuilder.BillingBuilder;
using ColloSys.QueryBuilder.GenericBuilder;
using ColloSys.QueryBuilder.StakeholderBuilder;
using ColloSys.UserInterface.Areas.Stakeholder2.Models;
using NHibernate.Criterion;
using Newtonsoft.Json;

#endregion

//stakeholders calls changed
//hierarchy calls changed
namespace ColloSys.UserInterface.Areas.Stakeholder2.api
{
    public class PaymentDetailsApiController : ApiController
    {
        private static readonly StakeQueryBuilder StakeQuery = new StakeQueryBuilder();
        private static readonly HierarchyQueryBuilder HierarchyQuery = new HierarchyQueryBuilder();
        private static readonly GKeyValueBuilder GKeyValueBuilder = new GKeyValueBuilder();
        private static readonly GPincodeBuilder GPincodeBuilder = new GPincodeBuilder();
        private static readonly BillingPolicyBuilder BillingPolicyBuilder = new BillingPolicyBuilder();

        [HttpPost]
        
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
        
        public WorkingModel GetPincodeList(WorkingModel pindata)
        {
            if (pindata.QueryFor == LocationLevels.City)
                pindata.QueryFor = LocationLevels.District;

            pindata.GetGPincodeData(pindata);
            return pindata;
        }

        [HttpGet]
        
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
        
        public BillingPolicyLists GetLinerWriteOff(ScbEnums.Products product)
        {
            var data = new BillingPolicyLists
                {
                    LinerList = BillingPolicyBuilder.LinePolicies(product).ToList(),
                    WriteOffList = BillingPolicyBuilder.WriteoffPolicies(product).ToList()
                };
            return data;
        }

        public class BillingPolicyLists
        {
            public IList<BillingPolicy> LinerList { get; set; }
            public IList<BillingPolicy> WriteOffList { get; set; }
        }

        [HttpGet]
        
        public PaymentDetails GetPaymentDetails()
        {
            var payment = new PaymentDetails();

            var gKeyValue = GKeyValueBuilder.ForStakeholders();

            payment.FixedPay = gKeyValue.ToDictionary(keyValue => keyValue.Key, keyValue => decimal.Parse(keyValue.Value));

            return payment;
        }

        public IEnumerable<string> GetProductList()
        {
            return Enum.GetNames(typeof(ScbEnums.Products)).Where(x => x != "UNKNOWN")
                .ToList();
        }

        [HttpGet]
        
        public IEnumerable<string> GetStateList()
        {
            return GPincodeBuilder.StateList();
        }

        [HttpGet]
        
        public IEnumerable<GPincode> GetCompleteClusterData()
        {
            return GPincodeBuilder.GetAll();
        }

        public IEnumerable<RegionState> GetRegionList()
        {
            var regionList = new List<RegionState>();

            var states = GPincodeBuilder.StateList();

            foreach (var state in states)
            {
                var reg = GPincodeBuilder.OnState(state);
                if (reg != null) regionList.Add(new RegionState { Region = reg.Region, State = state });
            }

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
        
        public IEnumerable<GPincode> GetClusters(Guid Id)
        {
            var session = SessionManager.GetCurrentSession();
            var stk2 = StakeQuery.OnIdWithAllReferences(Id);

            var list = new List<string>();
            var criteria = session.CreateCriteria(typeof(GPincode));
            var locationLevel = JsonConvert.DeserializeObject<List<string>>(stk2.Hierarchy.LocationLevel).First();
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

            return gpincodeData;
        }

        [HttpPost]
        
        public IEnumerable<Stakeholders> WorkingReportsTo(StkhHierarchy hierarchyId)
        {
            Guid reportingHierarchy = hierarchyId.WorkingReportsTo;
            var reportsTolist = new List<Stakeholders>();

            if (reportingHierarchy == Guid.Empty)
            {
                return reportsTolist;
            }
            var firstLevelHierarchy = HierarchyQuery.FilterBy(x => x.Id == reportingHierarchy)
                                                    .SingleOrDefault();

            if (firstLevelHierarchy == null)
            {
                return reportsTolist;
            }
            if (hierarchyId.WorkingReportsLevel == ColloSysEnums.ReportingLevel.AllLevels)
            {
                reportsTolist = StakeQuery.GetAll().ToList();
                return reportsTolist;
            }
            var firstlevelData = StakeQuery.OnHierarchyId(reportingHierarchy);

            reportsTolist.AddRange(firstlevelData);
            if (hierarchyId.WorkingReportsLevel == ColloSysEnums.ReportingLevel.OneLevelUp)
            {
                return reportsTolist;
            }

            var secondLevelHierarchy = HierarchyQuery.FilterBy(x => x.Id == firstLevelHierarchy.ReportsTo)
                                                     .SingleOrDefault();
            if (secondLevelHierarchy == null)
            {
                return reportsTolist;
            }
            var secondLevelData = StakeQuery.OnHierarchyId(secondLevelHierarchy.Id);

            reportsTolist.AddRange(secondLevelData);
            if (hierarchyId.WorkingReportsLevel == ColloSysEnums.ReportingLevel.TwoLevelUp)
            {
                return reportsTolist;
            }

            return reportsTolist;
        }


        [HttpGet]
        
        public IEnumerable<Stakeholders> StakeHolderList(string product, Guid selHierarchyId)
        {
            //Get Hierarchy by select hierarchy
            var hierarchy = HierarchyQuery.FilterBy(x => x.Id == selHierarchyId).SingleOrDefault();

            // Find reporting hierarchy ReportsToId
            var reportingHierarchy = HierarchyQuery.FilterBy(x => x.Id == hierarchy.ReportsTo).SingleOrDefault();

            // list of reporting hierarchyId
            var reportingIdlist = StakeQuery.OnHierarchyId(reportingHierarchy.Id).ToList();


            var hlist = HierarchyQuery.FilterBy(x => x.Id == reportingHierarchy.ReportsTo).SingleOrDefault();
            if (hlist == null)
                return reportingIdlist;

            var hlist4 = StakeQuery.OnHierarchyId(hlist.Id).ToList();


            var listMarge = new List<Stakeholders>();
            listMarge.AddRange(reportingIdlist);
            listMarge.AddRange(hlist4);

            return listMarge;
        }

        [HttpGet]
        
        public string GetRegionOfState(string state)
        {
            var data = GetRegionOnState(state);
            return data;
        }

        [HttpGet]
        
        public IEnumerable<GPincode> GetPincodes(string pincode, string level)
        {
            return level == "City" ? GetPincodesCity(pincode) : GetPincodesArea(pincode);
        }

        #region

        //private static IEnumerable<GPincode> GetClusterList(string cluster)
        //{
        //    var session = SessionManager.GetCurrentSession();
        //    try
        //    {
        //        var list = session.Query<GPincode>()
        //                          .Where(x => x.Cluster.StartsWith(cluster))
        //                          .GroupBy(x => x.Cluster)
        //                          .ToList();
        //        if (list.Count == 0) return null;

        //        return list.OrderBy(x => x.Key).Take(10).Select(entry => entry.First()).ToList();
        //    }
        //    catch (HibernateException exception)
        //    {
        //        LogManager.GetCurrentClassLogger().InfoException("Error while fetching Pincode list.", exception);
        //        throw;
        //    }
        //}

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
