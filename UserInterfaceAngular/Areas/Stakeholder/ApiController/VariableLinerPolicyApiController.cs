using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace UserInterfaceAngular.app
{
    public class VariableLinerPolicyApiController : BaseApiController
    {
        // GET api/variablelinerpolicyapi
        public IEnumerable<string> Get()
        {
            return new string[] { "Liner Policy1", "Liner Policy2", "Liner Policy3" };
        }
    }
}
