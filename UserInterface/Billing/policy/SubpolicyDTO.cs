using System;
using ColloSys.DataLayer.Billing;
using ColloSys.DataLayer.Enumerations;

namespace AngularUI.Billing.policy
{
    public class SubpolicyDTO
    {
        public string Name;
        public uint Priority;
        public ColloSysEnums.ApproveStatus ApproveStatus;
        public DateTime StartDate;
        public DateTime? EndDate;
        public SubpolicyActivityEnum Activity;
        public Guid PolicyId;
        public Guid SubpolicyId;
        public Guid RelationId;
        public SubpolicyTypeEnum SubpolicyType;

        public void Update(BillingRelation relation)
        {
            if (relation == null) return;
            Priority = relation.Priority;
            ApproveStatus = relation.Status;
            StartDate = relation.StartDate;
            EndDate = relation.EndDate;
            RelationId = relation.Id;
        }

        public void Update(BillingSubpolicy subpolicy)
        {
            if (subpolicy == null) return;
            Name = subpolicy.Name;
            SubpolicyId = subpolicy.Id;
        }
    }
}