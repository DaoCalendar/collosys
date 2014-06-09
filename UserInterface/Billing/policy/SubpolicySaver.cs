#region references

using System;
using System.Linq;
using ColloSys.DataLayer.Billing;
using ColloSys.DataLayer.Enumerations;
using NHibernate;
using NHibernate.Linq;

#endregion

namespace AngularUI.Billing.policy
{
    public class SubpolicySaver
    {
        private string Username { get; set; }
        private ISession Session { get; set; }

        public SubpolicySaver(string username, ISession session)
        {
            Username = username;
            Session = session;
        }

        public SubpolicyDTO ManageSubpolicyActivity(SubpolicyDTO subpolicy)
        {
            switch (subpolicy.Activity)
            {
                case SubpolicyActivityEnum.Activate:
                    return ActivateDraftSubpolicy(subpolicy);
                case SubpolicyActivityEnum.Expire:
                    return ExpireSubpolicy(subpolicy);
                case SubpolicyActivityEnum.Reactivate:
                    return ReactivateSubpolicy(subpolicy);
                case SubpolicyActivityEnum.Approve:
                    return ApproveSubpolicy(subpolicy);
                case SubpolicyActivityEnum.Reject:
                    return ApproveSubpolicy(subpolicy);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private SubpolicyDTO ActivateDraftSubpolicy(SubpolicyDTO subpolicy)
        {
            var subpolicyRelation = Session.Query<BillingSubpolicy>()
                .Where(x => x.Id == subpolicy.SubpolicyId)
                .Fetch(x => x.BillingRelations)
                .SingleOrDefault();

            if (!(subpolicyRelation == null || subpolicyRelation.BillingRelations == null
                || subpolicyRelation.BillingRelations.Count == 0))
            {
                throw new NullReferenceException("Draft subpolicy must have zero relations");
            }

            var relation = new BillingRelation
            {
                BillingPolicy = Session.Load<BillingPolicy>(subpolicy.PolicyId),
                BillingSubpolicy = Session.Load<BillingSubpolicy>(subpolicy.SubpolicyId),
                StartDate = subpolicy.StartDate,
                EndDate = subpolicy.EndDate,
                Priority = subpolicy.Priority,
                Status = ColloSysEnums.ApproveStatus.Submitted
            };
            using (var tx = Session.BeginTransaction())
            {
                Session.SaveOrUpdate(relation);
                tx.Commit();
            }

            subpolicy.ApproveStatus = ColloSysEnums.ApproveStatus.Submitted;
            subpolicy.SubpolicyType = SubpolicyTypeEnum.Unapproved;
            subpolicy.RelationId = relation.Id;
            return subpolicy;
        }

        private SubpolicyDTO ExpireSubpolicy(SubpolicyDTO subpolicy)
        {
            if (!subpolicy.EndDate.HasValue)
            {
                throw new InvalidProgramException("To Deactive any subpolicy start/end date are required");
            }
            var relation = Session.Query<BillingRelation>()
                .Single(x => x.Id == subpolicy.RelationId);
            Session.Evict(relation);
            relation.ResetUniqueProperties();
            relation.StartDate = subpolicy.StartDate;
            relation.EndDate = subpolicy.EndDate.Value;
            relation.Status = ColloSysEnums.ApproveStatus.Submitted;
            using (var tx = Session.BeginTransaction())
            {
                Session.SaveOrUpdate(relation);
                tx.Commit();
            }

            subpolicy.RelationId = relation.Id;
            subpolicy.Update(relation);
            subpolicy.SubpolicyType = SubpolicyTypeEnum.Unapproved;
            return subpolicy;
        }

        private SubpolicyDTO ApproveSubpolicy(SubpolicyDTO subpolicy)
        {
            var subpolicyRelation = Session.Query<BillingSubpolicy>()
                .Where(x => x.Id == subpolicy.SubpolicyId)
                .Fetch(x => x.BillingRelations)
                .SingleOrDefault();

            if (subpolicyRelation == null || subpolicyRelation.BillingRelations == null
                || subpolicyRelation.BillingRelations.Count == 0)
            {
                throw new NullReferenceException("Billing Relation cannot be null");
            }

            var relation = subpolicyRelation.BillingRelations
                .FirstOrDefault(x => x.Id == subpolicy.RelationId);
            if (relation == null)
            {
                throw new NullReferenceException("Billing Relation cannot be null");
            }

            var originRelation = subpolicyRelation.BillingRelations
                .FirstOrDefault(x => (x.EndDate == null || x.EndDate >= DateTime.Today)
                && x.Id != relation.Id);
            if (originRelation != null)
            {
                originRelation.EndDate = subpolicy.EndDate;
                originRelation.ApprovedBy = Username;
                originRelation.ApprovedOn = DateTime.Now;
            }

            relation.Status = subpolicy.Activity == SubpolicyActivityEnum.Approve
                ? ColloSysEnums.ApproveStatus.Approved
                : ColloSysEnums.ApproveStatus.Rejected;
            relation.ApprovedOn = DateTime.Now;
            relation.ApprovedBy = Username;
            using (var tx = Session.BeginTransaction())
            {
                if (originRelation != null)
                {
                    subpolicyRelation.BillingRelations.Remove(relation);
                    Session.SaveOrUpdate(subpolicyRelation);
                }
                else
                {
                    Session.SaveOrUpdate(relation);
                }
                tx.Commit();
            }

            if (subpolicy.Activity == SubpolicyActivityEnum.Approve)
            {
                subpolicy.ApproveStatus = ColloSysEnums.ApproveStatus.Approved;
                subpolicy.SubpolicyType = SubpolicyTypeEnum.Active;
            }
            return subpolicy;
        }

        private SubpolicyDTO RejectSubpolicy(SubpolicyDTO subpolicy)
        {
            var subpolicyRelation = Session.Query<BillingSubpolicy>()
                .Where(x => x.Id == subpolicy.SubpolicyId)
                .Fetch(x => x.BillingRelations)
                .SingleOrDefault();

            if (subpolicyRelation == null || subpolicyRelation.BillingRelations == null
                || subpolicyRelation.BillingRelations.Count == 0)
            {
                throw new NullReferenceException("Billing Relation cannot be null");
            }

            var relation = subpolicyRelation.BillingRelations
                .FirstOrDefault(x => x.Id == subpolicy.RelationId);
            if (relation == null)
            {
                throw new NullReferenceException("Billing Relation cannot be null");
            }

            var originRelation = subpolicyRelation.BillingRelations
                .FirstOrDefault(x => (x.EndDate == null || x.EndDate >= DateTime.Today)
                && x.Id != relation.Id);
            if (originRelation != null)
            {
                originRelation.EndDate = subpolicy.EndDate;
                originRelation.ApprovedBy = Username;
                originRelation.ApprovedOn = DateTime.Now;
            }

            relation.Status = subpolicy.Activity == SubpolicyActivityEnum.Approve
                ? ColloSysEnums.ApproveStatus.Approved
                : ColloSysEnums.ApproveStatus.Rejected;
            relation.ApprovedOn = DateTime.Now;
            relation.ApprovedBy = Username;
            using (var tx = Session.BeginTransaction())
            {
                if (originRelation != null)
                {
                    Session.SaveOrUpdate(originRelation);
                    Session.Delete(relation);
                }
                else
                {
                    Session.SaveOrUpdate(relation);
                }
                tx.Commit();
            }

            if (subpolicy.Activity == SubpolicyActivityEnum.Approve)
            {
                subpolicy.ApproveStatus = ColloSysEnums.ApproveStatus.Approved;
                subpolicy.SubpolicyType = SubpolicyTypeEnum.Active;
            }
            return subpolicy;
        }

        private SubpolicyDTO ReactivateSubpolicy(SubpolicyDTO subpolicy)
        {
            var subpolicyRelation = Session.Query<BillingSubpolicy>()
                .Where(x => x.Id == subpolicy.SubpolicyId)
                .Fetch(x => x.BillingRelations)
                .SingleOrDefault();

            if (subpolicyRelation == null || subpolicyRelation.BillingRelations == null
                || subpolicyRelation.BillingRelations.Count == 0)
            {
                throw new NullReferenceException("Draft subpolicy must have atleast one relations");
            }

            var relation = new BillingRelation
            {
                BillingPolicy = Session.Load<BillingPolicy>(subpolicy.PolicyId),
                BillingSubpolicy = Session.Load<BillingSubpolicy>(subpolicy.SubpolicyId),
                StartDate = subpolicy.StartDate,
                EndDate = subpolicy.EndDate,
                Priority = subpolicy.Priority,
                Status = ColloSysEnums.ApproveStatus.Submitted
            };
            using (var tx = Session.BeginTransaction())
            {
                Session.SaveOrUpdate(relation);
                tx.Commit();
            }

            subpolicy.ApproveStatus = ColloSysEnums.ApproveStatus.Submitted;
            subpolicy.SubpolicyType = SubpolicyTypeEnum.Unapproved;
            return subpolicy;
        }
    }
}
