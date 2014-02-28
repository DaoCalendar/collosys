#region references

using System;
using System.IO;
using System.Web;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;

#endregion

namespace ColloSys.DataLayer.Services.Shared
{
    public class ApprovalService<T> : IApprovalServices<T>
        where T : Entity, IApproverComponent
    {
        #region approve

        public void ApproveEntity(T entity)
        {
            // get user name
            var username = string.Empty;
            if ((HttpContext.Current != null) && (HttpContext.Current.User.Identity.IsAuthenticated))
            {
                username = HttpContext.Current.User.Identity.Name;
            }
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new InvalidProgramException("Username must be authenticated while approving the entity.");
            }

            // set general values
            entity.Status = ColloSysEnums.ApproveStatus.Approved;
            entity.ApprovedBy = username;
            entity.ApprovedOn = DateTime.Now;

            // if row is insert/delete then continue
            if (entity.RowStatus == RowStatus.Insert
                || entity.RowStatus == RowStatus.NotApplicable
                || entity.RowStatus == RowStatus.Delete)
            {
                return;
            }

            // swap ids - orig & id
            if (entity.OrigEntityId == Guid.Empty)
            {
                throw new InvalidDataException("for update row, the entity must have its orig id.");
            }
            var oldid = entity.Id;
            entity.Id = entity.OrigEntityId;
            entity.OrigEntityId = oldid;

            // get last version from db
            var origEntity = GetOrigObject(entity.Id);
            if (origEntity == null)
            {
                throw new InvalidDataException("could not fetch original entity for approval");
            }
            entity.Version = origEntity.Version;
        }

        private static T GetOrigObject(Guid id)
        {
            var session = SessionManager.GetCurrentSession();
            var orig = session.Get<T>(id);
            if (orig != null) session.Evict(orig);
            return orig;
        }

        #endregion

        #region reject

        public void RejectEntity(T entity)
        {
            // get user name
            var username = string.Empty;
            if ((HttpContext.Current != null) && (HttpContext.Current.User.Identity.IsAuthenticated))
            {
                username = HttpContext.Current.User.Identity.Name;
            }
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new InvalidProgramException("Username must be authenticated while rejecting the entity.");
            }

            // rejection reason is mandatory
            if (string.IsNullOrWhiteSpace(entity.Description))
            {
                throw new InvalidDataException("description is mandatory for rejection.");
            }

            // set general values
            entity.Status = ColloSysEnums.ApproveStatus.Rejected;
            entity.ApprovedBy = username;
            entity.ApprovedOn = DateTime.Now;
        }

        #endregion

        #region submit

        public void SubmitEntityForApproval(T entity, string approvedByUser, bool delete = false)
        {
            // user name of the approver is must
            if (string.IsNullOrWhiteSpace(approvedByUser))
            {
                throw new InvalidProgramException("Username cannot be empty while approving the entity.");
            }

            // set general values
            entity.Status = ColloSysEnums.ApproveStatus.Submitted;
            entity.ApprovedBy = approvedByUser;
            entity.ApprovedOn = null;
            entity.Description = string.Empty;

            //if this is new entity, then nothing to change during submit
            if (entity.Id == Guid.Empty)
            {
                entity.RowStatus = RowStatus.Insert;
                return;
            }
            entity.RowStatus = delete ? RowStatus.Delete : RowStatus.Update;

            // create new entity when submitted
            entity.OrigEntityId = entity.Id;
            entity.ResetUniqueProperties();
        }

        #endregion
    }
}
