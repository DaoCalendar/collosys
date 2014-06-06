using ColloSys.DataLayer.BaseEntity;

namespace ColloSys.DataLayer.Generic
{
    public class GUsersMap : EntityMap<GUsers>
    {
        public GUsersMap()
        {
            Property(x => x.Username, map => map.UniqueKey("GUSERS_USERNAME"));
            Property(x => x.ApplicationName);
            Property(x => x.Comment, map => map.NotNullable(false));
            Property(x => x.Email);
            Property(x => x.FailedPasswordAnswerAttemptCount);
            Property(x => x.FailedPasswordAnswerAttemptWindowStart);
            Property(x => x.FailedPasswordAttemptCount);
            Property(x => x.FailedPasswordAttemptWindowStart);
            Property(x => x.IsApproved);
            Property(x => x.IsLockedOut);
            Property(x => x.IsOnLine);
            Property(x => x.LastActivityDate);
            Property(x => x.LastLockedOutDate);
            Property(x => x.LastLoginDate);
            Property(x => x.LastPasswordChangedDate);
            Property(x => x.Password);
            Property(x => x.PasswordAnswer);
            Property(x => x.PasswordQuestion);

            ManyToOne(x => x.Role, map => map.NotNullable(true));
        }
    }
}
