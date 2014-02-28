#region References
using System.ComponentModel.DataAnnotations;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Enumerations;

#endregion

namespace ColloSys.DataLayer.Domain
{
    public class ProductConfig : Entity
    {
        #region Properties
        public virtual ScbEnums.Products Product { get; set; }

        public virtual ScbEnums.ScbSystems ProductGroup { get; set; }

        public virtual ColloSysEnums.AllocationPolicy AllocationResetStrategy { get; set; }

        public virtual ColloSysEnums.BillingPolicy BillingStrategy { get; set; }

        public virtual bool HasTelecalling { get; set; }

        public virtual uint FrCutOffDaysCycle { get; set; }

        public virtual uint FrCutOffDaysMonth { get; set; }

        public virtual string CycleCodes { get; set; }

        public virtual bool HasWriteOff { get; set; }

        public virtual ScbEnums.ClientDataTables LinerTable { get; set; }

        public virtual ScbEnums.ClientDataTables? WriteoffTable { get; set; }

        public virtual ScbEnums.ClientDataTables PaymentTable { get; set; }
        #endregion

        //#region Relationship None
        //public override void MakeEmpty(bool forceEmpty = false)
        //{
        //    return;
        //}
        //#endregion
    }
}
