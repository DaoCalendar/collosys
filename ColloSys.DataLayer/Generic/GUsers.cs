#region references

using System;
using ColloSys.DataLayer.BaseEntity;

#endregion

namespace ColloSys.DataLayer.Domain
{
    public class GUsers : Entity
    {
        #region Properties
        public virtual string Username { get; set; }
        public virtual string ApplicationName { get; set; }
        public virtual string Email { get; set; }

        public virtual string Comment { get; set; }
        public virtual string Password { get; set; }

        public virtual string PasswordQuestion { get; set; }
        public virtual string PasswordAnswer { get; set; }

        public virtual bool IsApproved { get; set; }
        public virtual DateTime LastActivityDate { get; set; }

        public virtual DateTime LastLoginDate { get; set; }
        public virtual DateTime LastPasswordChangedDate { get; set; }

        public virtual bool IsOnLine { get; set; }
        public virtual bool IsLockedOut { get; set; }
        public virtual DateTime LastLockedOutDate { get; set; }

        public virtual int FailedPasswordAttemptCount { get; set; }
        public virtual int FailedPasswordAnswerAttemptCount { get; set; }

        public virtual DateTime FailedPasswordAttemptWindowStart { get; set; }
        public virtual DateTime FailedPasswordAnswerAttemptWindowStart { get; set; }

        public virtual StkhHierarchy Role { get; set; }
        #endregion

        #region Constructor
        public GUsers()
        {
            // ReSharper disable DoNotCallOverridableMethodsInConstructor
            LastPasswordChangedDate = Convert.ToDateTime("01/01/1999");
            LastActivityDate = Convert.ToDateTime("01/01/1999");
            LastLockedOutDate = Convert.ToDateTime("01/01/1999");
            FailedPasswordAnswerAttemptWindowStart = Convert.ToDateTime("01/01/1999");
            FailedPasswordAttemptWindowStart = Convert.ToDateTime("01/01/1999");
            LastLoginDate = Convert.ToDateTime("01/01/1999");
            // ReSharper restore DoNotCallOverridableMethodsInConstructor
        }
        #endregion

        #region Methods

        public virtual void AddRole(StkhHierarchy role)
        {
            role.UsersInRole.Add(this);
            Role = role;
        }

        public virtual void RemoveRole(StkhHierarchy role)
        {
            role.UsersInRole.Remove(this);
            Role = null;
        }

        #endregion

        //#region Relationship None
        //public override void MakeEmpty(bool forceEmpty = false)
        //{
        //    return;
        //}
        //#endregion
    }
}
