using System.Collections.Generic;
using System.Web.Http;
using ColloSys.DataLayer.Allocation;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.UserInterface.AuthNAuth.Models;
using ColloSys.UserInterface.Shared.Attributes;

namespace UserInterfaceAngular.app
{
    public class NotificationApiController : ApiController
    {
        // GET api/<controller>
        [HttpGet]
        [HttpTransaction]
        public IEnumerable<NotificationModel> GetActivities()
        {
            var notificationList = new List<NotificationModel>();
            var session = SessionManager.GetCurrentSession();
            var allocCount = (uint)session.QueryOver<AllocPolicy>().Where(x => x.Status == ColloSysEnums.ApproveStatus.Submitted).List().Count;
            var billCount = (uint)session.QueryOver<BillingPolicy>().Where(x => x.Status == ColloSysEnums.ApproveStatus.Submitted).List().Count;

            //if (allocCount > 0)
            //{
            var allocmodel = new NotificationModel()
                {
                    Count = allocCount,
                    Activity = "Allocation Changes",
                    Url = "/Home/Index"
                };

            notificationList.Add(allocmodel);


            //}

            //if (billCount > 0)
            //{
            var billingmodel = new NotificationModel()
            {
                Count = billCount,
                Activity = "Billing Changes",
                Url = "/Home/Index"
            };
            notificationList.Add(billingmodel);
            //}

            return notificationList;

        }
    }
}