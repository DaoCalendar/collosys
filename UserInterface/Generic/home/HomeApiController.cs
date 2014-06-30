#region references

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AngularUI.Shared.apis;
using ColloSys.DataLayer.Enumerations;
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
            foreach (var user in list)
            {
                if (user.StakeholderId == stkh.Id)
                {
                    user.UserRole = "Self";
                    continue;
                }
                if (reportingStkh.Any(x => x.Id == user.StakeholderId))
                {
                    user.UserRole = "Reportee";
                    continue;
                }
                user.UserRole = "Delegate";
            }
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
        public HttpResponseMessage NotifyApproval(RceiveId notificationId)
        {
            var repo2Notify = new StkhNotificationRepository();
            var notification = repo2Notify.Get(notificationId.Id);
            if (!notification.IsResponse)
            {
                var repoNotify = new StakeholderNotificationManager(GetUsername());
                repoNotify.NotifyApprove(notification);
            }

            return Request.CreateResponse(HttpStatusCode.OK, "success");
        }

        [HttpPost]
        public HttpResponseMessage NotifyRejection(RceiveId notificationId)
        {
            var repo2Notify = new StkhNotificationRepository();
            var notification = repo2Notify.Get(notificationId.Id);
            if (!notification.IsResponse)
            {
                var repoNotify = new StakeholderNotificationManager(GetUsername());
                repoNotify.NotifyReject(notification);
            }
            return Request.CreateResponse(HttpStatusCode.OK, "success");
        }

        [HttpPost]
        public HttpResponseMessage NotifyDissmiss(RceiveId notificationId)
        {
            var repo2Notify = new StkhNotificationRepository();
            var notification = repo2Notify.Get(notificationId.Id);
            if (notification.NoteStatus != ColloSysEnums.NotificationStatus.Archived)
            {
                var repoNotify = new StakeholderNotificationManager(GetUsername());
                repoNotify.NotifyDismiss(notification);
            }
            return Request.CreateResponse(HttpStatusCode.OK, "success");
        }
    }
    public class RceiveId
    {
        public Guid Id { get; set; }
    }
}
