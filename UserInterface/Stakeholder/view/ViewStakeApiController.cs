#region references

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
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

        [HttpPost]
        public HttpResponseMessage GetFilteredList(PaginationParam filterParam)
        {
            var data = FilterDataHelper.GetFilteredStakeData(filterParam);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpGet]
        public HttpResponseMessage GetStakeById(string param)
        {
            var data = new List<Stakeholders>();
            if (Regex.IsMatch(param, @"^\d+$"))
            {
                if (param.Count() == 7) data.Add(StakeQuery.GetStakeByExtId(param));
            }
            data.AddRange(StakeQuery.GetStakeByName(param));

            return Request.CreateResponse(HttpStatusCode.OK, data);
        }
    }



    public class StkhId
    {
        public Guid Id { get; set; }
    }

    public class PaginationParam
    {
        public int page { get; set; }
        public int size { get; set; }
        public string name { get; set; }
        public string filter { get; set; }
    }
}