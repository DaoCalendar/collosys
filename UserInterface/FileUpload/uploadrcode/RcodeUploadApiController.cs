#region references

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Web.Http;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.SharedDomain;
using ColloSys.Shared.ExcelWriter;
using ColloSys.Shared.Types4Product;
using NHibernate.Criterion;
using NLog;

#endregion

namespace AngularUI.FileUpload.uploadrcode
{
    public class RcodeUploadApiController : ApiController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [HttpGet]
        public HttpResponseMessage FetchProducts()
        {
            var values = Enum.GetValues(typeof(ScbEnums.Products));
            var list = values.Cast<ScbEnums.Products>().ToList()
                             .Where(x => x.ToString() != ScbEnums.Products.UNKNOWN.ToString())
                             .OrderBy(x => x.ToString());
            return Request.CreateResponse(HttpStatusCode.OK, list);
        }

        
    }

    public class RcodeInfo
    {
        // ReSharper disable UnusedAutoPropertyAccessor.Global
        public string AccountNo { get; set; }
        public string CustomerName { get; set; }
        public string Rcode { get; set; }
        // ReSharper restore UnusedAutoPropertyAccessor.Global
    }
}