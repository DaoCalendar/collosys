using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ColloSys.DataLayer.Services.StakeholdersServices;

namespace UserInterfaceAngular.app
{
    public class ProductApiController : BaseApiController
    {
        // GET api/default1
        public IEnumerable<string> Get()
        {
            _log.Info("In Product web api");

            var data = StakeholderServices.GetProducts();

            _log.Info(data);

            return data;
        }
    }
}
