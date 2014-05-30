#region references

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AngularUI.Shared.apis;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.QueryBuilder.GenericBuilder;
using ColloSys.QueryBuilder.Test.GenerateDb;
using NHibernate.Linq;

#endregion

namespace AngularUI.Generic.permissions
{
    public class PermissionApiController : BaseApiController<GPermission>
    {
        private readonly GPermissionBuilder _permQuery = new GPermissionBuilder();

        [HttpGet]
        public HttpResponseMessage GetStakeHierarchy()
        {
            var hierData = Session.QueryOver<StkhHierarchy>().List();
            return Request.CreateResponse(HttpStatusCode.OK, hierData);
        }

        #region get permissions
        private void ParseList(GPermission permissions)
        {
            if (permissions.Childrens == null || permissions.Childrens.Count == 0) return;
            foreach (var gPermission in permissions.Childrens)
            {
                ParseList(gPermission);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetPermission(Guid id)
        {
            var hierarchy = Session.Get<StkhHierarchy>(id);
            var devPermission = PermissionManager.CreateDevPermissions(hierarchy);
            if (hierarchy.Hierarchy == "devadmin")
            {
                PermissionManager.SetAccess(devPermission, true);
                return Request.CreateResponse(HttpStatusCode.OK, devPermission); ;
            }


            var userPermission = Session.Query<GPermission>()
                                     .Where(x => x.Role.Id == id && x.Parent == null)
                                     .Fetch(x => x.Role)
                                     .FirstOrDefault();

            var usermenu = devPermission.Childrens.First(x => x.Activity == ColloSysEnums.Activities.User);
            if (userPermission == null)
            {
                PermissionManager.SetAccess(usermenu, true);
                return Request.CreateResponse(HttpStatusCode.OK, devPermission); ; 
            }

            ParseList(userPermission);
            PermissionManager.SetAccess(devPermission, false);
            devPermission.HasAccess = true;
            userPermission.HasAccess = true;
            PermissionManager.UpdateRoot(devPermission, userPermission);
            return Request.CreateResponse(HttpStatusCode.OK, devPermission); ; 
        }
        #endregion

        #region save
        private void SetParents(GPermission data)
        {
            if (data.Childrens == null || data.Childrens.Count == 0)
            {
                return;
            }

            foreach (var children in data.Childrens)
            {
                children.Parent = data;
                SetParents(children);
            }
        }

        [HttpPost]
        public HttpResponseMessage SavePerm(GPermission data)
        {
            SetParents(data);
            _permQuery.Save(data);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }
        #endregion
    }
}
