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
    public class StkhNotificationRepository : Repository<StkhNotification>
    {
        #region override
        public override QueryOver<StkhNotification, StkhNotification> ApplyRelations()
        {
            return QueryOver.Of<StkhNotification>()
                .Fetch(x => x.ForStakeholder).Eager;
        }
        #endregion

        #region fetch

        public IList<StkhNotification> GetNotificationsForStakeholder(Guid stkhId)
        {
            var session = SessionManager.GetCurrentSession();
            var data = session.Query<StkhNotification>()
                .Where(x => x.ForStakeholder.Id == stkhId
                    && x.NoteStatus == ColloSysEnums.NotificationStatus.Active)
                    .Fetch(x => x.ForStakeholder)
                    .Fetch(x => x.ByStakeholder)
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
                .Select(x => new { key = x.Key, value = x.Count() })
                .ToList();

            var reponse = new List<ActiveNotifications>();
            foreach (var stakeholder in stkhList)
            {
                var counter = data.SingleOrDefault(x => x.key == stakeholder.Id);
                var count = counter == null ? 0 : counter.value;

                reponse.Add(new ActiveNotifications
                {
                    StakeholderName = stakeholder.Name,
                    StakeholderId = stakeholder.Id,
                    NotifyCount = count
                });
            }

            return reponse;
        }

        #endregion

        #region create

        public Stakeholders GetLoginStakeholder(string username)
        {
            var stakeholderRepo = new StakeQueryBuilder();
            return stakeholderRepo.GetStakeByExtId(username);
        }

        public Stakeholders GetNotifyStakeholder(Stakeholders loginStakeholder)
        {
            var session = SessionManager.GetCurrentSession();

            return loginStakeholder.ReportingManager == Guid.Empty
                ? loginStakeholder
                : session.Load<Stakeholders>(loginStakeholder.ReportingManager);
        }

        #endregion
    }
}

