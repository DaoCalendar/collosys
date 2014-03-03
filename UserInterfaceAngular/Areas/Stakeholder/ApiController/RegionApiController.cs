using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace UserInterfaceAngular.app
{
    public class RegionApiController : BaseApiController
    {
        // GET api/regionapi
        public IEnumerable<string> Get()
        {
            _log.Info("In Region api");
            return new string[] { "North", "South","East","West" };
        }
    }
}
