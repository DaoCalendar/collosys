using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.UserInterface.Shared;
using ColloSys.UserInterface.Shared.Attributes;
using NHibernate.Linq;
using NLog;
using UserInterfaceAngular.app;

namespace UserInterfaceAngular.webapi
{
    public class RegistrationController : BaseApiController<StkhRegistration>
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        [HttpGet]
        [HttpTransaction]
        public IEnumerable<string> ListOfRegistrationNo()
        {
            _log.Info("In Stakeholders registration no list");
            var data = RegistrationNoList();
            return data;
        }

        [HttpGet]
        [HttpTransaction]
        public IEnumerable<string> ListOfPanNo()
        {
            _log.Info("In Stakeholders Pan no list");
            var data = PanNoList();
            return data;
        }

        [HttpGet]
        [HttpTransaction]
        public IEnumerable<string> ListOfTanNo()
        {
            _log.Info("In Stakeholders Tan no list");
            var data = TanNoList();
            return data;
        }

        [HttpGet]
        [HttpTransaction]
        public IEnumerable<string> ListOfServiceNo()
        {
            _log.Info("In Stakeholders service no list");
            var data = ServiceNoList();
            return data;
        }

        [HttpGet]
        [HttpTransaction]
        public bool CheckPanNo(string panno)
        {
            var list = PanNoList();
            var result = list.Contains(panno);
            return !result;
        }

        #region StakeholderServices
        #region registration list

        private static IEnumerable<string> RegistrationNoList()
        {
            var session = SessionManager.GetCurrentSession();

            var data = session.Query<StkhRegistration>()
                              .Select(x => x.RegistrationNo).ToList();
            LogManager.GetCurrentClassLogger().Info("StakeholderServices: RegistrationNo list count: " + data.Count);

            return data;
        }

        private static IEnumerable<string> PanNoList()
        {
            var session = SessionManager.GetCurrentSession();
            var data = session.Query<StkhRegistration>()
                              .Select(x => x.PanNo).ToList();

            LogManager.GetCurrentClassLogger().Info("StakeholderServices: Panno list count: " + data.Count);

            return data;
        }

        private static IEnumerable<string> TanNoList()
        {
            var session = SessionManager.GetCurrentSession();
            var data = session.Query<StkhRegistration>()
                              .Select(x => x.TanNo).ToList();

            LogManager.GetCurrentClassLogger().Info("StakeholderServices: Tan no list count: " + data.Count);

            return data;
        }

        private static IEnumerable<string> ServiceNoList()
        {
            var session = SessionManager.GetCurrentSession();
            var data = session.Query<StkhRegistration>()
                              .Select(x => x.ServiceTaxno).ToList();

            LogManager.GetCurrentClassLogger().Info("StakeholderServices: ServiceTaxNo list count: " + data.Count);

            return data;
        }

        #endregion
        #endregion
    }
}
