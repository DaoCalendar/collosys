#region References

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ColloSys.DataLayer.BaseEntity;

#endregion

namespace ColloSys.DataLayer.Domain
{
    public class GTaxesList : Entity
    {
        #region ctor & Relations

        public GTaxesList()
        {
            GTaxDetails = new List<GTaxDetail>();
        }

        //list
        public virtual IList<GTaxDetail> GTaxDetails { get; set; }

        #endregion

        #region Properties

        public virtual string TaxName { get; set; }

        public virtual string TaxType { get; set; }

        public virtual string ApplicableTo { get; set; }

        public virtual string IndustryZone { get; set; }

        public virtual string ApplyOn { get; set; }

        public virtual string TotSource { get; set; }

        public virtual string Description { get; set; }

        #endregion
    }
}