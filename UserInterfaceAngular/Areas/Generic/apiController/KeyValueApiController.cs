using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Generic;
using ColloSys.UserInterface.Shared;
using ColloSys.UserInterface.Shared.Attributes;

namespace ColloSys.UserInterface.Areas.Generic.apiController
{
    public class KeyValueApiController : BaseApiController<GKeyValue>
    {
        [HttpGet]
        [HttpTransaction]
        public IEnumerable<string> GetAreas()
        {
            return Enum.GetNames(typeof(ColloSysEnums.Activities));
        }

        [HttpGet]
        [HttpTransaction]
        public IEnumerable<GKeyValue> GetKeyValues(ColloSysEnums.Activities area)
        {
            return Session.QueryOver<GKeyValue>().Where(x=>x.Area==area).List();
        }


    }
}