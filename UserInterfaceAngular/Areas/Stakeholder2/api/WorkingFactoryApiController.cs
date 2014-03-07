using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ColloSys.UserInterface.Areas.Generic.ViewModels;
using ColloSys.UserInterface.Areas.Stakeholder2.Models;
using ColloSys.UserInterface.Shared.Attributes;

namespace ColloSys.UserInterface.Areas.Stakeholder2.api
{
    public class WorkingFactoryApiController : ApiController
    {
        [HttpPost]
        [HttpTransaction]
        public WorkingModel GetPincodeData(WorkingModel pindata)
        {
            pindata.SetWorkingList(pindata);
            return pindata;
        }
    }
}
