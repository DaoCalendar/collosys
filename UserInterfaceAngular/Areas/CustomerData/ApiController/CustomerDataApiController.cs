#region references

using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Enumerations;
using ColloSys.UserInterface.Areas.CustomerData.ViewModels;
using ColloSys.UserInterface.Shared.Attributes;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Web.Http;
using UserInterfaceAngular.NgGrid;

#endregion


namespace UserInterfaceAngular.app
{
    public class CustomerDataApiController : ApiController
    {
        [HttpGet]
        [HttpTransaction]
        public JObject GetSystemCategory()
        {
            var data = new Dictionary<string, object>();
            data["systems"] = Enum.GetNames(typeof(ScbEnums.ScbSystems));
            data["categories"] = Enum.GetNames(typeof(ScbEnums.Category));

            return JObject.FromObject(data);
        }

        [HttpGet]
        [HttpTransaction]
        public NgGridOptions GetNgGridOptions(ScbEnums.ScbSystems system, ScbEnums.Category category)
        {
            return CustomerDataViewModel.GetNgGrid(system, category);
        }
    }
}