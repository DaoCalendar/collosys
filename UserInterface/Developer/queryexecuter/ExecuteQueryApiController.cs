#region references

using System.Net;
using System.Net.Http;
using System.Web.Http;
using ColloSys.UserInterface.Areas.Generic.Models;
using ColloSys.UserInterface.Shared.Attributes;

#endregion

namespace AngularUI.Developer.queryexecuter
{
    public class ExecuteQueryApiController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage CheckandExecute(string query)
        {
            var checkQuery = query.Trim().Substring(0, 3);
            var result = new QueryResult();
            result.AddQuery(query);
            switch (checkQuery.ToUpper())
            {
                case "SEL":
                    var dataSelect = QueryExecuter.ExecuteSelect(query);
                    return Request.CreateResponse(HttpStatusCode.OK, dataSelect);

                case "UPD":
                case "DEL":
                case "INS":
                    var dataUpdate = QueryExecuter.ExecuteDataChange(query);
                    return Request.CreateResponse(HttpStatusCode.OK, dataUpdate);
                default:
                    return Request.CreateResponse(HttpStatusCode.OK, "QUERY INVALID");
            }
        }
    }
}
