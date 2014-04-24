#region References

using System;
using System.ComponentModel.DataAnnotations;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Components;

#endregion

namespace ColloSys.DataLayer.Domain
{
    public class GTaxDetail : Entity 
    {
        #region ctor & Relations

        // class
        public virtual GTaxesList GTaxesList { get; set; }
        //public virtual BMatrix BMatrix { get; set; }

        #endregion

        #region Properties

        public virtual string ApplicableTo { get; set; }

        public virtual string IndustryZone { get; set; }

        public virtual string Country { get; set; }

        public virtual string State { get; set; }

        public virtual string District { get; set; }

        public virtual int Priority { get; set; }

        public virtual UInt64 TaxId { get; set; }

        public virtual decimal Percentage { get; set; }
        #endregion

        #region DateRange Component
        public virtual DateTime StartDate { get; set; }

        public virtual DateTime? EndDate { get; set; }
        #endregion

    }
}