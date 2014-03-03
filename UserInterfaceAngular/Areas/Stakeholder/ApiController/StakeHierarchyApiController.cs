#region references

using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Collections.Generic;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.UserInterface.Shared.Attributes;
using NLog;

#endregion

namespace UserInterfaceAngular.app
{
    public class StakeHierarchyApiController : ApiController
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        #region GET

        // GET api/<controller>
        [HttpGet]
        [HttpTransaction]
        public IEnumerable<StkhHierarchy> Get()
        {
            try
            {
                var session = SessionManager.GetCurrentSession();
                return session.QueryOver<StkhHierarchy>()
                              .Where(x => x.Hierarchy != "Developer")
                              .List();
            }
            catch (Exception exception)
            {
                _log.ErrorException("Get : ", exception);
                throw new Exception(exception.Message, exception);
            }
        }

        #endregion

        #region POST

        // POST api/<controller>
        [HttpPost]
        [HttpTransaction(Persist = true)]
        public HttpResponseMessage Post(StkhHierarchy stakeHierarchy)
        {
            try
            {
                Save(stakeHierarchy);
                _log.Info("Stakeholder Hierarchy saved");
                return Request.CreateResponse(HttpStatusCode.Created, stakeHierarchy);
            }
            catch (Exception ex)
            {
                _log.ErrorException("Post : ", ex);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        #endregion

        #region PUT
        // PUT api/<controller>/5
        [HttpPut]
        [HttpTransaction(Persist = true)]
        public HttpResponseMessage Put(Guid Id, StkhHierarchy stakeHierarchy)
        {
            try
            {
                Save(stakeHierarchy);
                _log.Info("Stakeholder Hierarchy updated");
                return Request.CreateResponse(HttpStatusCode.Created, stakeHierarchy);
            }
            catch (Exception ex)
            {
                _log.ErrorException("Put : ", ex);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }
        #endregion

        #region StakeHierarchyService

        private static void Save(StkhHierarchy stkHierarchy)
        {
            using (var session = SessionManager.GetNewSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    session.SaveOrUpdate(stkHierarchy);
                    tx.Commit();
                }
            }
        }
        #endregion
    }
}