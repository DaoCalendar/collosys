using System;
using System.Linq;
using ColloSys.DataLayer.Billing;
using ColloSys.DataLayer.Enumerations;
using NHibernate;
using NHibernate.Linq;

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

        public void ManageSubpolicyActivity(SubpolicyDTO subpolicy)
        {
            switch (subpolicy.Activity)
            {
                case SubpolicyActivityEnum.Activate:
                    if (subpolicy.SubpolicyTypeEnum == SubpolicyTypeEnum.Draft)
                    {
                        ActivateDraftSubpolicy(subpolicy);
                    }
                    break;
                case SubpolicyActivityEnum.Expire:
                    DeactivateSubpolicy(subpolicy);
                    break;
                case SubpolicyActivityEnum.Reactivate:
                    ReactivateSubpolicy(subpolicy);
                    break;
                case SubpolicyActivityEnum.Approve:
                    ApproveSubpolicy(subpolicy);
                    break;
                case SubpolicyActivityEnum.Reject:
                    ApproveSubpolicy(subpolicy);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ActivateDraftSubpolicy(SubpolicyDTO subpolicy)
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

            if (subpolicy.UpdatedStartDate == null)
            {
                throw new NullReferenceException("Draft subpolicy must have start date specified");
            }

            var relation = new BillingRelation
            {
                BillingPolicy = Session.Load<BillingPolicy>(subpolicy.PolicyId),
                BillingSubpolicy = Session.Load<BillingSubpolicy>(subpolicy.SubpolicyId),
                StartDate = subpolicy.UpdatedStartDate.Value,
                EndDate = subpolicy.UpdatedEndDate,
                Priority = subpolicy.Priority,
                Status = ColloSysEnums.ApproveStatus.Submitted
            };
            using (var tx = Session.BeginTransaction())
            {
                Session.SaveOrUpdate(relation);
                tx.Commit();
            }
        }

        private void DeactivateSubpolicy(SubpolicyDTO subpolicy)
        {
            if (subpolicy.UpdatedStartDate == null || subpolicy.UpdatedEndDate == null)
            {
                throw new InvalidProgramException("To Deactive any subpolicy start/end date are required");
            }
            var relation = Session.Query<BillingRelation>()
                .Single(x => x.Id == subpolicy.RelationId);
            Session.Evict(relation);
            relation.ResetUniqueProperties();
            relation.StartDate = subpolicy.UpdatedStartDate.Value;
            relation.EndDate = subpolicy.UpdatedEndDate.Value;
            relation.Status = ColloSysEnums.ApproveStatus.Submitted;
            using (var tx = Session.BeginTransaction())
            {
                Session.SaveOrUpdate(relation);
                tx.Commit();
            }

            subpolicy.RelationId = relation.Id;
            subpolicy.Update(relation);
        }

        private void ApproveSubpolicy(SubpolicyDTO subpolicy)
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

            relation.Status = subpolicy.ApproveStatus;
            relation.ApprovedOn = DateTime.Now;
            relation.ApprovedBy = Username;
            using (var tx = Session.BeginTransaction())
            {
                Session.SaveOrUpdate(relation);
                tx.Commit();
            }
        }

        public BillingRelation CreateRelation()
        {
            return new BillingRelation
            {

            };
        }

        public BillingPolicy CreatePolicy()
        {
            return new BillingPolicy
            {

            };
        }

        private void ReactivateSubpolicy(SubpolicyDTO subpolicy)
        {
            //var subpolicy =
        }
    }
}

//draft
//submitted => only start date
//approve/reject
//deactiveate => enddate, if start==end delete
