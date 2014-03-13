#region references

using System;
using System.Data;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ColloSys.UserInterface.Shared.Attributes;
using ColloSys.UserInterface.Areas.Generic.Models;

#endregion

namespace ColloSys.UserInterface.Areas.Generic.apiController
{
    public class ExecuteQueryApiController : ApiController
    {
        [HttpGet]
        [HttpTransaction]
        public HttpResponseMessage CheckandExecute(string query)
        {
            var checkQuery = query.Substring(0,3);
            var result = new QueryResult();
            result.AddQuery(query);
            try
            {
                switch (checkQuery.ToUpper())
                {
                    case "SEL":
                        var dataSelect = QueryExecuter.ExecuteNonScaler(query);
                        return Request.CreateResponse(HttpStatusCode.Created, dataSelect);

                    case "UPD":
                    case "DEL":
                    case "INS":
                        var dataUpdate = QueryExecuter.ExecuteNonQueryUpdateDelete(query);
                        return Request.CreateResponse(HttpStatusCode.Created, dataUpdate);
                    default:
                        return Request.CreateResponse(HttpStatusCode.Created, "QUERY INVALID");
                }
            }
            catch (Exception ex)
            {
               var dataTable = new DataTable();
                dataTable.Columns.Add("Exception");
                var dr = dataTable.NewRow();
                dr["Exception"] = ex;
                dataTable.Rows.Add(dr);
                return Request.CreateResponse(HttpStatusCode.Created, dataTable);
            }
        }
    }
}
