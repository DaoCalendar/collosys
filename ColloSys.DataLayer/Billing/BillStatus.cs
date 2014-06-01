#region References
using System;
using System.ComponentModel.DataAnnotations;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Enumerations;

#endregion

namespace ColloSys.DataLayer.Domain
{
    public class BillStatus : Entity
    {

        public virtual Stakeholders Stakeholder { get; set; }

        #region Property
        public virtual UInt32 BillMonth { get; set; }

        public virtual UInt32 OriginMonth { get; set; }

        public virtual UInt32 BillCycle { get; set; }

        public virtual ColloSysEnums.BillingStatus Status { get; set; }

        public virtual ScbEnums.Products Products { get; set; }

        public virtual string ExternalId { get; set; }
        #endregion

        //#region Relationship None
        //public override void MakeEmpty(bool forceEmpty = false)
        //{
        //    return;
        //}
        //#endregion
    }
}