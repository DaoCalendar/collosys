using System;
using System.Collections.Generic;
using ColloSys.DataLayer.Billing;
using ColloSys.DataLayer.Enumerations;
using NHibernate;

namespace AngularUI.Billing.policy
{
    public class SubpolicyList
    {
        public List<BillingSubpolicy> IsInUseSubpolices = new List<BillingSubpolicy>();
        public List<BillingSubpolicy> NotInUseSubpolices = new List<BillingSubpolicy>();
    }

    public class SubpolicySaveParams
    {
        public string Activity { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public BillingSubpolicy Subpolicy { get; set; }
        public BillingPolicy Policy { get; set; }
    }

    public class SubpolicyRelationManager
    {
        private SubpolicySaveParams Params { get; set; }
        private string Username { get; set; }
        private ISession Session { get; set; }

        public SubpolicyRelationManager(string username, ISession session, SubpolicySaveParams param)
        {
            Username = username;
            Session = session;
            Params = param;
        }

        public void ApproveSubpolicy(ColloSysEnums.ApproveStatus status)
        {
            if (Params.Subpolicy.BillingRelations == null || Params.Subpolicy.BillingRelations.Count != 0)
            {
                throw new NullReferenceException("Billing Relation cannot be null");
            }
            var relation = Params.Subpolicy.BillingRelations[0];
            relation.Status = status;
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

        public void DeactivateSubpolicy()
        {

        }

        public void ActivateSubpolicy()
        {

        }

        public void ReactivateSubpolicy()
        {
            
        }
    }
}