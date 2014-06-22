#region references

using System;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SessionMgr;
using ColloSys.DataLayer.Stakeholder;
using ColloSys.QueryBuilder.StakeholderBuilder;

#endregion

namespace AngularUI.Generic.home
{
    public abstract class NotificationManager
    {
        #region ctor
        protected readonly string Username;
        protected readonly StkhNotificationRepository NotificationRepository;

        protected NotificationManager(string user)
        {
            Username = user;
            NotificationRepository = new StkhNotificationRepository();
        }
        #endregion

        #region approval
        private void SetResponseFields(StkhNotification notification, Stakeholders loginStakeholder)
        {
            var session = SessionManager.GetCurrentSession();
            notification.ForStakeholder = session.Load<Stakeholders>(notification.ByStakeholder.Id);
            notification.ByStakeholder = session.Load<Stakeholders>(loginStakeholder.Id);
            notification.IsResponse = true;
            notification.ResponseBy = loginStakeholder.Name;
            notification.ResponseDateTime = DateTime.Now;
        }

        public void NotifyApprove(StkhNotification notification)
        {
            var loginStkh = NotificationRepository.GetLoginStakeholder(Username);
            SetResponseFields(notification, loginStkh);
            notification.ResponseType = "Approved";
            NotificationRepository.Save(notification);
        }

        public void NotifyReject(StkhNotification notification)
        {
            var loginStkh = NotificationRepository.GetLoginStakeholder(Username);
            SetResponseFields(notification, loginStkh);
            notification.ResponseType = "Rejected";
            NotificationRepository.Save(notification);
        }

        public void NotifyDismiss(StkhNotification notification)
        {
            notification.NoteStatus = ColloSysEnums.NotificationStatus.Archived;
            NotificationRepository.Save(notification);
        }
        #endregion
    }
}