using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AngularUI.Billing.policy;
using ColloSys.DataLayer.Allocation;
using ColloSys.DataLayer.Enumerations;
using NHibernate;
using NHibernate.Linq;

namespace AngularUI.Allocation.policy
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
            var subpolicyRelation = Session.Query<AllocSubpolicy>()
                .Where(x => x.Id == subpolicy.SubpolicyId)
                .Fetch(x => x.AllocRelations)
                .SingleOrDefault();

            if (!(subpolicyRelation == null || subpolicyRelation.AllocRelations == null
                || subpolicyRelation.AllocRelations.Count == 0))
            {
                throw new NullReferenceException("Draft subpolicy must have zero relations");
            }

            var relation = new AllocRelation
            {
                AllocPolicy = Session.Load<AllocPolicy>(subpolicy.PolicyId),
                AllocSubpolicy = Session.Load<AllocSubpolicy>(subpolicy.SubpolicyId),
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
            var relation = Session.Query<AllocRelation>()
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
            var subpolicyRelation = Session.Query<AllocSubpolicy>()
                .Where(x => x.Id == subpolicy.SubpolicyId)
                .Fetch(x => x.AllocRelations)
                .SingleOrDefault();

            if (subpolicyRelation == null || subpolicyRelation.AllocRelations == null
                || subpolicyRelation.AllocRelations.Count == 0)
            {
                throw new NullReferenceException("Billing Relation cannot be null");
            }

            var relation = subpolicyRelation.AllocRelations
                .FirstOrDefault(x => x.Id == subpolicy.RelationId);
            if (relation == null)
            {
                throw new NullReferenceException("Billing Relation cannot be null");
            }

            var originRelation = subpolicyRelation.AllocRelations
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
                    subpolicyRelation.AllocRelations.Remove(relation);
                    Session.SaveOrUpdate(subpolicyRelation);
                }
                else
                {
                    if (subpolicy.Activity == SubpolicyActivityEnum.Approve)
                        Session.SaveOrUpdate(relation);
                    else
                        //TODO: check with sonu (i dont think db entry is getting deleted)
                        subpolicyRelation.AllocRelations.Remove(relation);
                    // Session.Delete(relation);
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
            var subpolicyRelation = Session.Query<AllocSubpolicy>()
                .Where(x => x.Id == subpolicy.SubpolicyId)
                .Fetch(x => x.AllocRelations)
                .SingleOrDefault();

            if (subpolicyRelation == null || subpolicyRelation.AllocRelations == null
                || subpolicyRelation.AllocRelations.Count == 0)
            {
                throw new NullReferenceException("Draft subpolicy must have atleast one relations");
            }

            var relation = new AllocRelation
            {
                AllocPolicy = Session.Load<AllocPolicy>(subpolicy.PolicyId),
                AllocSubpolicy = Session.Load<AllocSubpolicy>(subpolicy.SubpolicyId),
                StartDate = subpolicy.StartDate,
                EndDate = subpolicy.EndDate,
                Priority = subpolicy.Priority,
                Status = ColloSysEnums.ApproveStatus.Submitted,
            };

            using (var tx = Session.BeginTransaction())
            {
                //TODO: check with sonu
                subpolicyRelation.AllocRelations.Clear();
                subpolicyRelation.AllocRelations.Add(relation);
                Session.SaveOrUpdate(subpolicyRelation);
                tx.Commit();
            }
            subpolicy.ApproveStatus = ColloSysEnums.ApproveStatus.Submitted;
            subpolicy.SubpolicyType = SubpolicyTypeEnum.Unapproved;
            subpolicy.RelationId = relation.Id;
            return subpolicy;
        }
    }
}