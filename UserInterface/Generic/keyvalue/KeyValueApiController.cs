#region references

using System;
using System.Collections.Generic;
using System.Web.Http;
using AngularUI.Shared.apis;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Generic;
using ColloSys.QueryBuilder.GenericBuilder;
using ColloSys.UserInterface.Shared;
using ColloSys.UserInterface.Shared.Attributes;

#endregion


namespace ColloSys.UserInterface.Areas.Generic.apiController
{
    public class KeyValueApiController : BaseApiController<GKeyValue>
    {
        private static readonly GKeyValueBuilder GKeyValueBuilder=new GKeyValueBuilder();
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
            return GKeyValueBuilder.FilterBy(x => x.Area == area);
        }


    }
}