#region references

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AngularUI.Shared.apis;
using AngularUI.Stakeholder.addedit.Working;
using AngularUI.Stakeholder.AddEdit2.Working;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Stakeholder;
using ColloSys.QueryBuilder.StakeholderBuilder;

#endregion

namespace AngularUI.Stakeholder.AddEdit2.BasicInfo
{
    //TODO: 1. remove pincode typeahead
    //TODO: 2. reporting list
    //TODO: 3. return HttpResponseMessage
    public class StakeholderApiController : BaseApiController<Stakeholders>
    {
        private static readonly HierarchyQueryBuilder HierarchyQuery = new HierarchyQueryBuilder();
        private static readonly StakeQueryBuilder StakeQuery = new StakeQueryBuilder();

        [HttpGet]
        public IEnumerable<StkhHierarchy> GetAllHierarchies()
        {
            return HierarchyQuery.FilterBy(x => x.Hierarchy != "Developer");
        }

        [HttpGet]
        public HttpResponseMessage CheckUserId(string id)
        {
            if (id == null || id.Count() != 7)
                return Request.CreateResponse(HttpStatusCode.OK, "success");

            var idExists = StakeQuery.FilterBy(x => x.ExternalId == id);

            return (idExists.Count > 0)
                ? Request.CreateErrorResponse(HttpStatusCode.Conflict, "error")
                : Request.CreateResponse(HttpStatusCode.OK, "success");
        }

        [HttpGet]
        public IEnumerable<Stakeholders> GetReportsToData(Guid hierarchyId, ColloSysEnums.ReportingLevel level)
        {
            return WorkingPaymentHelper.GetReportsOnreportingLevel(hierarchyId, level);
        }

        [HttpPost]
        public HttpResponseMessage SaveStake(Stakeholders data)
        {
            AddEditStakeholder.SetStakeholderObj(data);
            StakeQuery.Save(data);
            return Request.CreateResponse(HttpStatusCode.Created, data);
        }
    }
}


//[HttpGet]
//public IEnumerable<GPincode> GetPincodes(string pincode, string level)
//{
//    return level == "City" ? GetPincodesCity(pincode) : GetPincodesArea(pincode);
//}


//private static IEnumerable<GPincode> GetPincodesCity(string pin)
//{
//    var list = GPincodeBuilder.OnPinOrCity(pin).ToList();
//    if (list.Count == 0) return null;

//    var uniq = (from l in list group l by l.City into g select g.First()).ToList();
//    uniq.RemoveAll(x => x.City.Trim().Equals("-"));
//    return uniq.Take(10);
//}


//private static IEnumerable<GPincode> GetPincodesArea(string pin)
//{
//    var list = GPincodeBuilder.OnPinOrArea(pin).ToList();
//    if (list.Count == 0) return null;

//    var uniq = (from l in list group l by l.Area into g select g.First()).ToList();
//    uniq.RemoveAll(x => x.City.Trim().Equals("-"));
//    return uniq.Take(10);
//}

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

