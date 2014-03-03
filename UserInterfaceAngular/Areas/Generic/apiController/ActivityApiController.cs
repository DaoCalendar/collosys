using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace UserInterfaceAngular.Areas.Generic.apiController
{
    public class ActivityApiController : ApiController
    {
        // GET api/<controller>
        [ActionName("DefaultAction")]
        public IEnumerable<string> Get()
        {
            return new string[] { "Allocation Sub Policy", "Allocation Policy", "Allocation Modification", "Allocation Approval" };
        }

    }
}