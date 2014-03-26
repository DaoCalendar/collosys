#region References
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Enumerations;
using NHibernate;

#endregion

namespace ColloSys.DataLayer.Domain
{
    public class BillingPolicy : Entity, IApproverComponent//, IDateRangeComponent
    {
        #region relationships

        public virtual Iesi.Collections.Generic.ISet<BillingRelation> BillingRelations { get; set; }
        public virtual IList<StkhPayment> CollectionStkhPayments { get; set; }
        public virtual IList<StkhPayment> RecoveryStkhPayments { get; set; }

        public virtual Iesi.Collections.Generic.ISet<StkhPayment> StkhPayments { get; set; }
        public virtual Iesi.Collections.Generic.ISet<BillDetail> BillDetails { get; set; }


        #endregion

        #region Property
        public virtual string Name { get; set; }

        public virtual ScbEnums.Products Products { get; set; }

        public virtual ScbEnums.Category Category { get; set; }

        #endregion

        #region Approve
        public virtual ColloSysEnums.ApproveStatus Status { get; set; }
        public virtual string Description { get; set; }
        public virtual string ApprovedBy { get; set; }
        public virtual DateTime? ApprovedOn { get; set; }
        public virtual Guid OrigEntityId { get; set; }
        public virtual RowStatus RowStatus { get; set; }

        #endregion
    }
}