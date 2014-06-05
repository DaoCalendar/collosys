using System;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using NHibernate;

namespace AngularUI.Billing.newpolicy
{
    public class SubpolicyRelationManager
    {
        public string Activity { get; set; }
        public BillingSubpolicy Subpolicy { get; set; }
        public BillingPolicy Policy { get; set; }
        public BillingRelation Relation { get; set; }
        public string Username { get; set; }
        public ISession Session { get; set; }

        public void ApproveSubpolicy()
        {
            Relation.Status = ColloSysEnums.ApproveStatus.Approved;
            Relation.ApprovedOn = DateTime.Now;
            Relation.ApprovedBy = Username;
        }

        public void RejectSubpolicy()
        {
            Relation.Status = ColloSysEnums.ApproveStatus.Rejected;
            Relation.ApprovedOn = DateTime.Now;
            Relation.ApprovedBy = Username;
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

        public void DeactivateSubpolicy()
        {
            
        }

        public void ActivateSubpolicy()
        {
            
        }
    }
}