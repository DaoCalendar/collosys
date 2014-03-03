using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace UserInterfaceAngular.app
{
    public class CityApiController : BaseApiController
    {
        // GET api/cityapi
        public IEnumerable<string> Get()
        {
            return new string[] { "Pune", "Mumbai","Nasik","Delhi","Jaipur","Ahemadabad" };
        }
    }
}
