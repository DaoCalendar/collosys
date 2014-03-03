using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace UserInterfaceAngular.app
{
    public class VariableWriteOffPolicyApiController : BaseApiController
    {
        // GET api/variablewriteoffpolicyapi
        public IEnumerable<string> Get()
        {
            return new string[] { "WriteOff Policy1", "WriteOff Policy2", "WriteOff Policy3" };
        }
    }
}
