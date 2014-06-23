using System;

namespace ColloSys.QueryBuilder.StakeholderBuilder
{
    public class ActiveNotifications
    {
        public Guid StakeholderId { get; set; }
        public string UserRole { get; set; }
        public string StakeholderName { get; set; }
        public int NotifyCount { get; set; }
    }
}