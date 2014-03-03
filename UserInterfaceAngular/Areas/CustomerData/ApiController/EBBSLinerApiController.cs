using System.Collections.Generic;
using System.Web.Http;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.UserInterface.Shared.Attributes;

namespace UserInterfaceAngular.app
{
    public class EBBSLinerApiController : ApiController
    {
        [HttpGet]
        [HttpTransaction]
        public IEnumerable<ELiner> Get()
        {
            var session = SessionManager.GetCurrentSession();
            return session.QueryOver<ELiner>().List();
        }
    }
}