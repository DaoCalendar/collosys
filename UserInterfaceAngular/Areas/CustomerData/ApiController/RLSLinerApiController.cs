#region references

using System.Collections.Generic;
using System.Web.Http;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.UserInterface.Shared.Attributes;

#endregion

namespace UserInterfaceAngular.app
{
    public class RLSLinerApiController : ApiController
    {
        [HttpGet]
        [HttpTransaction]
        public IEnumerable<RLiner> Get()
        {
            var session = SessionManager.GetCurrentSession();
            return session.QueryOver<RLiner>().List();
        }
    }
}