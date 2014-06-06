#region references

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AngularUI.Shared.apis;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Generic;
using ColloSys.DataLayer.Stakeholder;
using ColloSys.QueryBuilder.GenericBuilder;
using ColloSys.QueryBuilder.StakeholderBuilder;
using NLog;

#endregion

//stakeholders calls changed
//hierarchy calls changed
//namespace UserInterfaceAngular.app
namespace AngularUI.Stakeholder.AddEdit2.BasicInfo
{
    public class StakeholderApiController : BaseApiController<Stakeholders>
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        private static readonly HierarchyQueryBuilder HierarchyQuery = new HierarchyQueryBuilder();
        private static readonly StakeQueryBuilder StakeQuery = new StakeQueryBuilder();
        //private static readonly AddressQueryBuilder AddressQuery = new AddressQueryBuilder();
        //private static readonly StakePaymentQueryBuilder StakePaymentBuilder = new StakePaymentQueryBuilder();
        //private static readonly StakeWorkingQueryBuilder StakeWorkingBuilder = new StakeWorkingQueryBuilder();
        //private static readonly ProductConfigBuilder ProductConfigBuilder = new ProductConfigBuilder();
        private static readonly GPincodeBuilder GPincodeBuilder = new GPincodeBuilder();

        [HttpGet]

        public IEnumerable<StkhHierarchy> GetAllHierarchies()
        {
            return HierarchyQuery.FilterBy(x => x.Hierarchy != "Developer");
        }

        [HttpGet]

        public bool CheckUserId(string id)
        {
            var idExists = StakeQuery.FilterBy(x => x.ExternalId == id);
            return (idExists.Count > 0);
        }

        [HttpGet]

        public IEnumerable<GPincode> GetPincodes(string pincode, string level)
        {
            return level == "City" ? GetPincodesCity(pincode) : GetPincodesArea(pincode);
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

        public IEnumerable<Stakeholders> GetReportsToInHierarchy(Guid reportsto)
        {
            var data = GetReportsToList(reportsto);
            if (data != null)
                _log.Info("Reports to list loaded in StakeholderApi/GetReportingList");
            return data;

        }

        [HttpPost]
        public HttpResponseMessage SaveStake(Stakeholders data)
        {
            AddEditStakeholder.SetStakeholderObj(data);
            StakeQuery.Save(data);
            return Request.CreateResponse(HttpStatusCode.Created, data);
        }

        #region reportsTo

        [HttpGet]

        public IEnumerable<Stakeholders> GetReportingList(string reportsTo, string hierarchy)
        {
            var data = GetReportsToList(reportsTo, hierarchy);
            if (data != null)
                _log.Info("Reports to list loaded in StakeholderApi/GetReportingList");
            return data;
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

