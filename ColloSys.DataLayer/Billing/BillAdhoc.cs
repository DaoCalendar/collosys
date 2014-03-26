#region references
using System;
using System.ComponentModel.DataAnnotations;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Enumerations;
using Iesi.Collections.Generic;

#endregion

namespace ColloSys.DataLayer.Domain
{
    public class BillAdhoc : Entity, IApproverComponent
    {
        public virtual ISet<BillDetail> BillDetails { get; set; }

        public virtual Stakeholders Stakeholder { get; set; }

        public virtual  ScbEnums.Products Products{ get; set; }

        public virtual decimal TotalAmount { get; set; }

        public virtual decimal RemainingAmount { get; set; }

        public virtual bool IsRecurring { get; set; }

        public virtual bool IsCredit { get; set; }

        public virtual string IsPretax { get; set; }

        public virtual string ReasonCode { get; set; }

        public virtual uint StartMonth { get; set; }

        public virtual uint EndMonth { get; set; }

        public virtual uint Tenure { get; set; }

        #region IApprove
        public virtual ColloSysEnums.ApproveStatus Status { get; set; }
        public virtual string Description { get; set; }
        public virtual string ApprovedBy { get; set; }
        public virtual DateTime? ApprovedOn { get; set; }
        public virtual Guid OrigEntityId { get; set; }
        public virtual RowStatus RowStatus { get; set; }
        #endregion
    }
}
