#region references

using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AngularUI.Shared.apis;
using ColloSys.DataLayer.Generic;
using ColloSys.DataLayer.Stakeholder;
using ColloSys.QueryBuilder.GenericBuilder;
using ColloSys.QueryBuilder.StakeholderBuilder;

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
            var stkhRepo = new StakeQueryBuilder();
            var stkh = stkhRepo.GetStakeByExtId(GetUsername());

            var hierarchy = Session.Get<StkhHierarchy>(id);
            var devPermission = PermissionManager.GetPermission(hierarchy,stkh.ExternalId);
            return Request.CreateResponse(HttpStatusCode.OK, devPermission); 
        }
        #endregion

        #region save
        private static void SetParents(GPermission data)
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
