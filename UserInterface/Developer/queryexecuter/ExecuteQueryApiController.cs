#region references

using System.Net;
using System.Net.Http;
using System.Web.Http;
using ColloSys.UserInterface.Areas.Generic.Models;

#endregion

namespace AngularUI.Developer.queryexecuter
{
    public class ExecuteQueryApiController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage CheckandExecute(QueryParams query)
        {
            var checkQuery = query.Query.Trim().Substring(0, 3);
            var result = new QueryResult();
            result.AddQuery(query.Query);
            switch (checkQuery.ToUpper())
            {
                case "SEL":
                    var dataSelect = QueryExecuter.ExecuteSelect(query.Query);
                    return Request.CreateResponse(HttpStatusCode.OK, dataSelect);

                case "UPD":
                case "DEL":
                case "INS":
                    var dataUpdate = QueryExecuter.ExecuteDataChange(query.Query);
                    return Request.CreateResponse(HttpStatusCode.OK, dataUpdate);
                default:
                    return Request.CreateResponse(HttpStatusCode.OK, "QUERY INVALID");
            }
        }

        public class QueryParams
        {
            public string Query;
        }
    }
}
