#region references

using System;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SessionMgr;
using ColloSys.DataLayer.Stakeholder;
using ColloSys.QueryBuilder.StakeholderBuilder;

#endregion

namespace AngularUI.Stakeholder.addedit.BasicInfo
{
    public static class AddEditStakeholder
    {
        public static void SetStakeholderObj(Stakeholders data)
        {
            data.Status = ColloSysEnums.ApproveStatus.Submitted;
            //add stakeholder reference in address
            foreach (var address in data.StkhAddress)
            {
                address.Stakeholder = data;
            }

            //add stakeholder reference in registration
            foreach (var registration in data.StkhRegistrations)
            {
                registration.Stakeholder = data;
            }
        }

        private static Stakeholders GetLoginStakeholder(string username)
        {
            var stakeholderRepo = new StakeQueryBuilder();
            return stakeholderRepo.GetStakeByExtId(username);
        }

        private static Stakeholders GetNotifyStakeholder(Stakeholders loginStakeholder)
        {
            var session = SessionManager.GetCurrentSession();

            return loginStakeholder.ReportingManager == Guid.Empty
                ? loginStakeholder
                : session.Load<Stakeholders>(loginStakeholder.ReportingManager);
        }

        private static StkhNotification GenerateStakeholderAddNotification(Stakeholders addedStakeholder, Stakeholders notifyStakeholder)
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

        public static StkhNotification GenerateStkhWorkingAddNotification(StkhWorking addedStkhWorking, Stakeholders notifyStakeholder)
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

        public static StkhNotification GenerateStkhPaymentAddNotification(StkhPayment addedStkhPayment, Stakeholders notifyStakeholder)
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

        public static void NotifyNewStakeholder(Stakeholders addedStakeholder, string username)
        {
            var loginStkh = GetLoginStakeholder(username);
            var notifyStkh = GetNotifyStakeholder(loginStkh);
            var notification = GenerateStakeholderAddNotification(addedStakeholder, notifyStkh);
            var repo = new StkhNotificationRepository();
            repo.Save(notification);
        }
    }
}