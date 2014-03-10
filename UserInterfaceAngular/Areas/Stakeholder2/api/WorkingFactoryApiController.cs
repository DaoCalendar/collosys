#region references

using System.Web.Http;
using ColloSys.UserInterface.Areas.Stakeholder2.Models;
using ColloSys.UserInterface.Shared.Attributes;

#endregion

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
