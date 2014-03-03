using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.UserInterface.Shared;
using ColloSys.UserInterface.Shared.Attributes;
using NLog;

namespace UserInterfaceAngular.app
{
    public class LocationController : BaseApiController<GPincode>
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        [HttpGet]
        [HttpTransaction]
        protected override IEnumerable<GPincode> BaseGet()
        {
            _log.Info("In Location web api");

            var data = SessionManager.GetCurrentSession()
                        .QueryOver<GPincode>()
                        .List().Distinct();
            //SessionManager.GetRepository<GPincode>()
            //                .GetAll()
            //                .Distinct();
            _log.Info("StakeholderServices: Total Count from GetLocations loaded " + data.Count());

            //_log.Info(data);

            return data;
        }


    }
}
