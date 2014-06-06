using System.Collections.Generic;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Enumerations;

namespace ColloSys.DataLayer.Allocation
{
    public class AllocPolicy : Entity
    {
        public virtual IList<AllocRelation> AllocRelations { get; set; }
        public virtual IList<Allocations> Allocs { get; set; }

        public virtual string Name { get; set; }
        public virtual ScbEnums.Products Products { get; set; }
    }
}