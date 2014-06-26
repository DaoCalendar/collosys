#region references

using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AngularUI.Shared.apis;
using AngularUI.Stakeholder.addedit.BasicInfo;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Stakeholder;
using ColloSys.QueryBuilder.GenericBuilder;
using ColloSys.QueryBuilder.StakeholderBuilder;

#endregion

namespace AngularUI.Stakeholder.view
{
    public class ViewStakeApiController : BaseApiController<Stakeholders>
    {
        private static readonly StakeQueryBuilder StakeQuery = new StakeQueryBuilder();

        [HttpGet]
        public HttpResponseMessage GetAllStakeHolders()
        {
            var list = StakeQuery.GetAllStakeholders();
            return Request.CreateResponse(HttpStatusCode.OK, list);
        }

        [HttpGet]
        public HttpResponseMessage GetFilteredList(string filterParam)
        {
            var data = FilterDataHelper.GetFilteredStakeData(filterParam);

            return Request.CreateResponse(HttpStatusCode.OK, data);
        }
    }



    public class StkhId
    {
        public Guid Id { get; set; }
    }
}