using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace UserInterfaceAngular.app
{
    public class ClusterApiController : BaseApiController
    {
        // GET api/clusterapi
        public IEnumerable<string> Get()
        {
            _log.Info("In Cluster webapi");
            return new string[] { "North", "East","South","West","Central" };
        }
    }
}
