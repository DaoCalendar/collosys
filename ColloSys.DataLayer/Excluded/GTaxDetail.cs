#region References

using System;
using System.ComponentModel.DataAnnotations;
using ColloSys.DataLayer.Components;

#endregion

namespace ColloSys.DataLayer.Domain
{
    public class GTaxDetail : Entity,IDateRangeComponent 
    {
        #region ctor & Relations

        // class
        public virtual GTaxesList GTaxesList { get; set; }
        public virtual BMatrix BMatrix { get; set; }

        #endregion

        #region Properties

        [Required]
        public virtual string ApplicableTo { get; set; }

        public virtual string IndustryZone { get; set; }

        [Required]
        public virtual string Country { get; set; }

        [Required]
        public virtual string State { get; set; }

        [Required]
        public virtual string District { get; set; }

        [Required]
        public virtual int Priority { get; set; }

        [Required]
        public virtual UInt64 TaxId { get; set; }

        public virtual string Percentage { get; set; }
        #endregion

        #region DateRange Component
        [Required]
        [DataType(DataType.Date)]
        public virtual DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public virtual DateTime? EndDate { get; set; }
        #endregion

    }
}