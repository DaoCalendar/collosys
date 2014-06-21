#region references

using System;
using ColloSys.DataLayer.Enumerations;

#endregion

namespace ColloSys.DataLayer.Components
{
    public interface IApproverComponent
    {
        ColloSysEnums.ApproveStatus Status { get; set; }

        string Description { get; set; }

        string ApprovedBy { get; set; }

        DateTime? ApprovedOn { get; set; }

        Guid OrigEntityId { get; set; }

        RowStatus RowStatus { get; set; }
    }

    public interface IApprovalStatus
    {
        ColloSysEnums.ApproveStatus ApprovalStatus { get; set; }
        string ApprovedBy { get; set; }
        DateTime? ApprovedOn { get; set; }
    }

    public enum RowStatus
    {
        NotApplicable,
        Insert,
        Update,
        Delete
    }
}