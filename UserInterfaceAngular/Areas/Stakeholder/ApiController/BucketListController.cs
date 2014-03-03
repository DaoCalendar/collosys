using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using NLog;

namespace UserInterfaceAngular.app
{
    public class BucketListController : BaseApiController
    {
        // GET api/bucketlist
        public IEnumerable<int> Get()
        {
            _log.Info("In getBucket list");
            return new int[] {0,1,2,3,4,5,6,7,8,9,10};
        }

      
    }
}
