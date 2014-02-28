#region References
using System;
using System.ComponentModel.DataAnnotations;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Enumerations;

#endregion

namespace ColloSys.DataLayer.Domain
{
    public class StkhTax : Entity, IApproverComponent
    {
        //Relation
        public virtual Stakeholders Stakeholder { get; set; }

        #region Property
        public virtual string TaxType { get; set; }

        public virtual decimal? AmountRange { get; set; }

        public virtual decimal TaxPercentage { get; set; }
        #endregion

        #region DateRange
        
        [DataType(DataType.Date)]
        public virtual DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public virtual DateTime? EndDate { get; set; }
        #endregion

        #region Approver
        [EnumDataType(typeof(ColloSysEnums.ApproveStatus))]
        public virtual ColloSysEnums.ApproveStatus Status { get; set; }
        public virtual string Description { get; set; }
        public virtual string ApprovedBy { get; set; }
        public virtual DateTime? ApprovedOn { get; set; }
        #endregion
    }
}
