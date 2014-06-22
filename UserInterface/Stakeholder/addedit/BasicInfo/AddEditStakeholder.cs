#region references

using System;
using System.Data.SqlTypes;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Generic;
using ColloSys.DataLayer.SessionMgr;
using ColloSys.DataLayer.Stakeholder;
using ColloSys.QueryBuilder.StakeholderBuilder;
using ColloSys.Shared.Encryption;

#endregion

namespace AngularUI.Stakeholder.addedit.BasicInfo
{
    public static class AddEditStakeholder
    {
        public static void SetStakeholderObj(Stakeholders data)
        {
            data.ApprovalStatus = ColloSysEnums.ApproveStatus.Submitted;
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

        public static GUsers CreateUser(Stakeholders stakeholder)
        {
            var user = new GUsers
            {
                Role = stakeholder.Hierarchy,
                ApplicationName = "ColloSys",
                Email = string.IsNullOrWhiteSpace(stakeholder.EmailId)
                    ? "invalid@sc.com"
                    : stakeholder.EmailId,
                FailedPasswordAnswerAttemptCount = 0,
                FailedPasswordAnswerAttemptWindowStart = SqlDateTime.MinValue.Value,
                FailedPasswordAttemptCount = 0,
                FailedPasswordAttemptWindowStart = SqlDateTime.MinValue.Value,
                IsApproved = true,
                IsLockedOut = false,
                LastActivityDate = SqlDateTime.MinValue.Value,
                LastLockedOutDate = SqlDateTime.MinValue.Value,
                Password = PasswordUtility.EncryptText("collosys"),
                PasswordAnswer = "20010101",
                PasswordQuestion = "What is your joining date?",
                Username = stakeholder.ExternalId,
                LastLoginDate = DateTime.Now,
                LastPasswordChangedDate = DateTime.Now
            };

            return user;
        }
    }
}