#region references

using System;
using System.Collections.Generic;
using System.Web.Http;
using AngularUI.Shared.apis;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Generic;
using ColloSys.QueryBuilder.GenericBuilder;

#endregion


namespace ColloSys.UserInterface.Areas.Generic.apiController
{
    public class KeyValueApiController : BaseApiController<GKeyValue>
    {
        private static readonly GKeyValueBuilder GKeyValueBuilder=new GKeyValueBuilder();
        [HttpGet]
        
        public IEnumerable<string> GetAreas()
        {
            return Enum.GetNames(typeof(ColloSysEnums.Activities));
        }

        [HttpGet]
        
        public IEnumerable<GKeyValue> GetKeyValues(ColloSysEnums.Activities area)
        {
            return GKeyValueBuilder.FilterBy(x => x.Area == area);
        }


    }
}