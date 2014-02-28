#region References
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Enumerations;
using Iesi.Collections.Generic;
using NHibernate;

#endregion 

namespace ColloSys.DataLayer.Domain
{
    public class StkhPayment : Entity, IApproverComponent //IDateRangeComponent
    {
        #region Properties
        public virtual Stakeholders Stakeholder { get; set; }
        public virtual BillingPolicy CollectionBillingPolicy { get; set; }
        public virtual BillingPolicy RecoveryBillingPolicy { get; set; }

        public virtual ScbEnums.Products Products { get; set; }
        
        public virtual string BankAccNo { get; set; }

        public virtual string BankAccName { get; set; }

        public virtual string BankIfscCode { get; set; }
        
        public virtual decimal MobileElig { get; set; }
        
        public virtual decimal TravelElig { get; set; }

       // public virtual string VariableLiner { get; set; }

        //public virtual string VariableWriteoff { get; set; }
        
        public virtual decimal FixpayBasic { get; set; }
        
        public virtual decimal FixpayHra { get; set; }
        
        public virtual decimal FixpayOther { get; set; }
        
        public virtual decimal FixpayTotal { get; set; }

        public virtual decimal ServiceCharge { get; set; }

        public virtual DateTime? StartDate { get; set; }

        public virtual DateTime? EndDate { get; set; }

        #endregion

        #region Approver
        public virtual ColloSysEnums.ApproveStatus Status { get; set; }
        public virtual string Description { get; set; }
        public virtual string ApprovedBy { get; set; }
        public virtual DateTime? ApprovedOn { get; set; }
        public virtual Guid OrigEntityId { get; set; }
        public virtual RowStatus RowStatus { get; set; }

        #endregion

        #region Relationship None
        public virtual IList<StkhWorking> StkhWorkings { get; set; }

        //public override void MakeEmpty(bool forceEmpty = false)
        //{
        //    if (!NHibernateUtil.IsInitialized(StkhWorkings) || forceEmpty) StkhWorkings = null;
        //}

        #endregion
    }
}
