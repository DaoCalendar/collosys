using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AngularUI.Shared.apis;
using ColloSys.DataLayer.Stakeholder;
using ColloSys.QueryBuilder.StakeholderBuilder;

namespace AngularUI.Generic.home
{
    public class HomeApiController : BaseApiController<StkhNotification>
    {
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
    }
}
