using System.Collections.Generic;
using ColloSys.DataLayer.BaseEntity;

namespace ColloSys.DataLayer.Generic
{
    public class GTaxesList : Entity
    {
        public virtual IList<GTaxDetail> GTaxDetails { get; set; }

        public virtual string TaxName { get; set; }
        public virtual string TaxType { get; set; }
        public virtual string ApplicableTo { get; set; }
        public virtual string IndustryZone { get; set; }
        public virtual string ApplyOn { get; set; }
        public virtual string TotSource { get; set; }
        public virtual string Description { get; set; }
    }
}