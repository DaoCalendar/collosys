#region references

using System;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Stakeholder;

#endregion

namespace AngularUI.Generic.home
{
    public class StakeholderNotificationManager : NotificationManager
    {
        public StakeholderNotificationManager(string user) : base(user) { }

        #region new stakeholder
        private StkhNotification GenerateStakeholderAddNotification(Stakeholders addedStakeholder,
                Stakeholders notifyStakeholder, Stakeholders loginStakeholder)
        {
            var desciptionStkhAdd = "Added new stakeholder ";
            desciptionStkhAdd += addedStakeholder.Name.Trim().ToUpperInvariant();
            if (!string.IsNullOrWhiteSpace(addedStakeholder.ExternalId))
                desciptionStkhAdd += string.Format(" ({0})", addedStakeholder.ExternalId);
            desciptionStkhAdd += string.Format(" as {0}, {1}.",
                addedStakeholder.Hierarchy.Designation, addedStakeholder.Hierarchy.Hierarchy);

            var stkhAddNote2 = new StkhNotification
            {
                ByStakeholder = loginStakeholder,
                Description = desciptionStkhAdd,
                EntityId = addedStakeholder.Id,
                ForStakeholder = notifyStakeholder,
                NoteType = ColloSysEnums.NotificationType.StakeholderChange,
                NoteStatus = ColloSysEnums.NotificationStatus.Active,
                IsResponse = false,
                RequestBy = loginStakeholder.Name,
                RequestDateTime = DateTime.Now
            };

            return stkhAddNote2;
        }

        public void NotifyNewStakeholder(Stakeholders addedStakeholder)
        {
            var loginStkh = NotificationRepository.GetLoginStakeholder(Username);
            var notifyStkh = NotificationRepository.GetNotifyStakeholder(loginStkh);
            var notification = GenerateStakeholderAddNotification(addedStakeholder, notifyStkh, loginStkh);
            NotificationRepository.Save(notification);
        }
        #endregion
    }
}