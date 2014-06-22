#region references

using System;
using System.Collections.Generic;
using System.Linq;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SessionMgr;
using ColloSys.DataLayer.Stakeholder;
using ColloSys.QueryBuilder.BaseTypes;
using NHibernate.Criterion;
using NHibernate.Linq;

#endregion

namespace ColloSys.QueryBuilder.StakeholderBuilder
{
    public class ActiveNotifications
    {
        public Guid StakeholderId { get; set; }
        public string StakeholderName { get; set; }
        public int NotifyCount { get; set; }
    }

    public class StkhNotificationRepository : Repository<StkhNotification>
    {
        #region override
        public override QueryOver<StkhNotification, StkhNotification> ApplyRelations()
        {
            return QueryOver.Of<StkhNotification>()
                .Fetch(x => x.ForStakeholder).Eager;
        }
        #endregion

        #region shared

        public IList<StkhNotification> GetNotificationsForStakeholder(Guid stkhId)
        {
            var session = SessionManager.GetCurrentSession();
            var data = session.Query<StkhNotification>()
                .Where(x => x.ForStakeholder.Id == stkhId 
                    && x.NoteStatus == ColloSysEnums.NotificationStatus.Active)
                .ToList();
            return data;
        } 

        public IList<ActiveNotifications> GetNotifications(IList<Stakeholders> stkhList)
        {
            var stkhIdList = stkhList.Select(x => x.Id).ToList();
            var session = SessionManager.GetCurrentSession();
            var data = session.Query<StkhNotification>()
                .Where(x => stkhIdList.Contains(x.ForStakeholder.Id) 
                    && x.NoteStatus == ColloSysEnums.NotificationStatus.Active)
                .GroupBy(x => x.ForStakeholder.Id)
                .Select(x => new { key = x.Key, value = x.Count()})
                .ToList();

            var noticeList = new List<ActiveNotifications>();
            foreach (var stkh in data)
            {
                if(stkh.value == 0) continue;
                noticeList.Add( new ActiveNotifications
                {
                    NotifyCount = stkh.value,
                    StakeholderId =  stkh.key,
                    StakeholderName = stkhList.Where( x=> x.Id == stkh.key).Select( x=> x.Name).First()
                });
            }

            return noticeList;
        }

        private Stakeholders GetLoginStakeholder(string username)
        {
            var stakeholderRepo = new StakeQueryBuilder();
            return stakeholderRepo.GetStakeByExtId(username);
        }

        private Stakeholders GetNotifyStakeholder(Stakeholders loginStakeholder)
        {
            var session = SessionManager.GetCurrentSession();

            return loginStakeholder.ReportingManager == Guid.Empty
                ? loginStakeholder
                : session.Load<Stakeholders>(loginStakeholder.ReportingManager);
        }
        #endregion

        #region notify stakeholder activity
        private StkhNotification GenerateStakeholderAddNotification(Stakeholders addedStakeholder, Stakeholders notifyStakeholder)
        {
            var desciptionStkhAdd = "Added new stakeholder ";
            desciptionStkhAdd += addedStakeholder.Name.Trim().ToUpperInvariant();
            if (!string.IsNullOrWhiteSpace(addedStakeholder.ExternalId))
                desciptionStkhAdd += string.Format(" ({0})", addedStakeholder.ExternalId);
            desciptionStkhAdd += string.Format(" as {0}, {1}.",
                addedStakeholder.Hierarchy.Designation, addedStakeholder.Hierarchy.Hierarchy);

            var stkhAddNote2 = new StkhNotification
            {
                Description = desciptionStkhAdd,
                ForStakeholder = notifyStakeholder,
                NoteType = ColloSysEnums.NotificationType.StakeholderChange,
                NoteStatus = ColloSysEnums.NotificationStatus.Active,
                EntityId = addedStakeholder.Id
            };

            return stkhAddNote2;
        }

        public StkhNotification GenerateStkhWorkingAddNotification(StkhWorking addedStkhWorking, Stakeholders notifyStakeholder)
        {
            var desciption = "Added working details for ";
            desciption += addedStkhWorking.Stakeholder.Name.Trim().ToUpperInvariant();
            desciption += ".";

            var stkhWorkNote = new StkhNotification
            {
                Description = desciption,
                ForStakeholder = notifyStakeholder,
                NoteType = ColloSysEnums.NotificationType.StakeholderWorkingChange,
                NoteStatus = ColloSysEnums.NotificationStatus.Active,
                EntityId = addedStkhWorking.Stakeholder.Id
            };

            return stkhWorkNote;
        }

        public StkhNotification GenerateStkhPaymentAddNotification(StkhPayment addedStkhPayment, Stakeholders notifyStakeholder)
        {
            var desciption = "Added fixed payment for ";
            desciption += addedStkhPayment.Stakeholder.Name.Trim().ToUpperInvariant();
            desciption += ", Gross : " + addedStkhPayment.FixpayGross;

            var stkhPaymentNote = new StkhNotification
            {
                Description = desciption,
                ForStakeholder = notifyStakeholder,
                NoteType = ColloSysEnums.NotificationType.StakeholderPaymentChange,
                NoteStatus = ColloSysEnums.NotificationStatus.Active,
                EntityId = addedStkhPayment.Stakeholder.Id
            };

            return stkhPaymentNote;
        }

        public void NotifyNewStakeholder(Stakeholders addedStakeholder, string username)
        {
            var loginStkh = GetLoginStakeholder(username);
            var notifyStkh = GetNotifyStakeholder(loginStkh);
            var notification = GenerateStakeholderAddNotification(addedStakeholder, notifyStkh);
            Save(notification);
        }
        #endregion
    }
}
