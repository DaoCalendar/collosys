#region references

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AngularUI.Shared.apis;
using ColloSys.DataLayer.Stakeholder;
using ColloSys.QueryBuilder.StakeholderBuilder;

#endregion

namespace AngularUI.Generic.home
{
    public class HomeApiController : BaseApiController<StkhNotification>
    {
        #region get list
        [HttpGet]
        public HttpResponseMessage GetUserNotifyList()
        {
            var repoStkh = new StakeQueryBuilder();
            var stkh = repoStkh.GetStakeByExtId(GetUsername());
            var reportingStkh = repoStkh.GetReportingList(stkh.Id);
            var repoStkhLeave = new StkhLeaveRepository();
            var deledatedStkh = repoStkhLeave.GetDelegatedToMe(stkh.Id, DateTime.Today);

            var stkhList = new List<Stakeholders> { stkh };
            stkhList.AddRange(reportingStkh);
            stkhList.AddRange(deledatedStkh);

            var repoNotify = new StkhNotificationRepository();
            var list = repoNotify.GetNotifications(stkhList);
            return Request.CreateResponse(HttpStatusCode.OK, list);
        }

        [HttpGet]
        public HttpResponseMessage GetActiveNotificationForStakeholder(Guid stakeholderId)
        {
            var repoNotify = new StkhNotificationRepository();
            var list = repoNotify.GetNotificationsForStakeholder(stakeholderId);
            return Request.CreateResponse(HttpStatusCode.OK, list);
        }
        #endregion

        [HttpPost]
        public HttpResponseMessage NotifyApproval(StkhNotification notification)
        {
            var repoNotify = new StakeholderNotificationManager(GetUsername());
            repoNotify.NotifyApprove(notification);
            return Request.CreateResponse(HttpStatusCode.OK, "success");
        }

        [HttpPost]
        public HttpResponseMessage NotifyRejection(StkhNotification notification)
        {
            var repoNotify = new StakeholderNotificationManager(GetUsername());
            repoNotify.NotifyReject(notification);
            return Request.CreateResponse(HttpStatusCode.OK, "success");
        }

        [HttpPost]
        public HttpResponseMessage NotifyDissmiss(StkhNotification notification)
        {
            var repoNotify = new StakeholderNotificationManager(GetUsername());
            repoNotify.NotifyDismiss(notification);
            return Request.CreateResponse(HttpStatusCode.OK, "success");
        }
    }
}
