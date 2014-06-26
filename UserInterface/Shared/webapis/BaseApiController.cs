#region references

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Web.Http;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.SessionMgr;
using ColloSys.QueryBuilder.BaseTypes;
using NHibernate;
using NLog;

#endregion

namespace AngularUI.Shared.apis
{
    public abstract class BaseApiController<TEntity> : ApiController where TEntity : Entity, new()
    {
        #region properties

        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private IRepository<TEntity> _repository = new Repository<TEntity>();  
        protected ISession Session
        {
            get { return SessionManager.GetCurrentSession(); }
        }

        #endregion

        #region restful api

        [HttpGet]
        public virtual HttpResponseMessage Get()
        {
            try
            {
                var data = BaseGet();
                return Request.CreateResponse(HttpStatusCode.OK, data);
            }
            catch (HibernateException e)
            {
                _log.Error("ApiController: Error in GetList: " + e);
                return Request.CreateResponse(HttpStatusCode.ExpectationFailed, e);
            }
            catch (Exception e)
            {
                _log.Error("ApiController: Error in GetList: " + e);
                return Request.CreateResponse(HttpStatusCode.ExpectationFailed, e);
            }
        }

        [HttpGet]
        public HttpResponseMessage Get(Guid id)
        {
            try
            {
                var data = BaseGet(id);
                return Request.CreateResponse(HttpStatusCode.OK, data);
            }
            catch (HibernateException e)
            {
                _log.Error("ApiController: Error in Get: " + e);
                return Request.CreateResponse(HttpStatusCode.ExpectationFailed, e);
            }
            catch (Exception e)
            {
                _log.Error("ApiController: Error in Get: " + e);
                return Request.CreateResponse(HttpStatusCode.ExpectationFailed, e);
            }
        }

        // POST api/baseapi
        [HttpPost]
        public HttpResponseMessage Post(TEntity entity)
        {
            try
            {
                if (entity == null)
                {
                    throw new NullReferenceException("Please provide non-null object for POST.");
                }

                var data = BasePost(entity);
                return Request.CreateResponse(HttpStatusCode.Created, data);
            }
            catch (NullReferenceException e)
            {
                _log.Error("ApiController: Error in Post: " + e);
                return Request.CreateResponse(HttpStatusCode.BadRequest, e);
            }
            catch (HibernateException e)
            {
                _log.Error("ApiController: Error in Post: " + e);
                return Request.CreateResponse(HttpStatusCode.ExpectationFailed, e);
            }
            catch (Exception e)
            {
                _log.Error("ApiController: Error in Post: " + e);
                return Request.CreateResponse(HttpStatusCode.ExpectationFailed, e);
            }
        }

        [HttpPut]
        public virtual HttpResponseMessage Put(Guid id, TEntity obj)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    throw new InvalidDataException("Please use PUT to save transient entities.");
                }
                if (obj == null)
                {
                    throw new NullReferenceException("Cannot save null or empty entities.");
                }
                if (id != obj.Id)
                {
                    throw new InvalidDataException("Forged request. Id do not match with entity-id.");
                }

                var data = BasePut(id, obj);
                return Request.CreateResponse(HttpStatusCode.OK, data);
            }
            catch (NullReferenceException e)
            {
                _log.Error("ApiController: Error in Put: " + e);
                return Request.CreateResponse(HttpStatusCode.BadRequest, e);
            }
            catch (InvalidDataException e)
            {
                _log.Error("ApiController: Error in Put: " + e);
                return Request.CreateResponse(HttpStatusCode.BadRequest, e);
            }
            catch (HibernateException e)
            {
                _log.Error("ApiController: Error in Put: " + e);
                return Request.CreateResponse(HttpStatusCode.ExpectationFailed, e);
            }
            catch (Exception e)
            {
                _log.Error("ApiController: Error in Put: " + e);
                return Request.CreateResponse(HttpStatusCode.ExpectationFailed, e);
            }
        }

        [HttpDelete]
        public virtual HttpResponseMessage Delete(Guid id)
        {
            try
            {
                if (id.Equals(Guid.Empty))
                {
                    throw new InvalidDataException("Id provided is empty. No entity with such id exist.");
                }
                BaseDelete(SessionManager.GetCurrentSession().Load<TEntity>(id));
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (InvalidDataException e)
            {
                _log.Error("ApiController: Error in Delete: " + e);
                return Request.CreateResponse(HttpStatusCode.BadRequest, e);
            }
            catch (HibernateException e)
            {
                _log.Error("ApiController: Error in Delete: " + e);
                return Request.CreateResponse(HttpStatusCode.ExpectationFailed, e);
            }
            catch (Exception e)
            {
                _log.Error("ApiController: Error in Delete: " + e);
                return Request.CreateResponse(HttpStatusCode.ExpectationFailed, e);
            }
        }

        #endregion

        #region api override
        protected virtual IEnumerable<TEntity> BaseGet()
        {
            return _repository.GetAll();
        }

        protected virtual TEntity BaseGet(Guid id)
        {
            return _repository.Get(id);
        }

        protected virtual TEntity BasePut(Guid id, TEntity obj)
        {
            return _repository.Save(obj);
        }

        protected virtual TEntity BasePost(TEntity obj)
        {
            return _repository.Save(obj);
        }

        protected virtual void BaseDelete(TEntity obj)
        {
            _repository.Delete(obj);
        }

        #endregion

        protected string GetUsername()
        {
            var username = string.Empty;
            var jobject = Request.Headers.Authorization;
            if (jobject != null && !string.IsNullOrWhiteSpace(jobject.Scheme)) 
                username = jobject.Scheme;
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new AuthenticationException("User has not logged in");
            }

            return username;
        }
    }
}



//protected  JsonResult Json(object data,
//    // ReSharper disable OptionalParameterHierarchyMismatch
//                                    string contentType = "application/json",
//                                    Encoding contentEncoding = null,
//                                    JsonRequestBehavior behavior = JsonRequestBehavior.AllowGet)
//// ReSharper restore OptionalParameterHierarchyMismatch
//{
//    return new JsonNetResult
//    {
//        Data = data,
//        ContentType = contentType,
//        ContentEncoding = Encoding.UTF8,
//        JsonRequestBehavior = behavior
//    };
//}

