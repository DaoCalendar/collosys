#region References
using System;
using System.ComponentModel.DataAnnotations;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Enumerations;

#endregion

namespace ColloSys.DataLayer.Domain
{
    public class BillDetail : Entity
    {
        //relationships
        public virtual Stakeholders Stakeholder { get; set; }
        public virtual BillingPolicy BillingPolicy { get; set; }
        public virtual BillingSubpolicy BillingSubpolicy { get; set; }
        public virtual BillAdhoc BillAdhoc { get; set; }
        #region Property

        public virtual UInt32 BillMonth { get; set; }

        public virtual UInt32 BillCycle { get; set; }

        public virtual decimal Amount { get; set; }
      
        public virtual ScbEnums.Products Products { get; set; }

        public virtual ColloSysEnums.PaymentSource PaymentSource { get; set; }
        #endregion


        //public virtual string ElementType { get; set; }

        //public virtual UInt64 ElementId { get; set; }

        //public virtual string ParamName { get; set; }

        //public virtual string ParamValue { get; set; }

        //#region Relationship None
        //public override void MakeEmpty(bool forceEmpty = false)
        //{
        //    return;
        //}
        //#endregion
    }
}