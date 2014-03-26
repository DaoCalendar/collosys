using System.Collections.Generic;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SharedDomain;

namespace ColloSys.DataLayer.Allocation
{
    public class AllocSubpolicy : Entity, IPolicyStatusComponent
    {
        public virtual Stakeholders Stakeholder { get; set; }

        public virtual IList<AllocRelation> AllocRelations { get; set; }
        public virtual IList<Allocations> Allocs { get; set; }
        public virtual IList<AllocCondition> Conditions { get; set; }

        public virtual string Name { get; set; }
        public virtual ColloSysEnums.AllocationType AllocateType { get; set; }
        public virtual string ReasonNotAllocate { get; set; }
        public virtual uint NoAllocMonth { get; set; }
        public virtual ScbEnums.Products Products { get; set; }
        public virtual ScbEnums.Category Category { get; set; }

        public virtual bool IsActive { get; set; }
        public virtual bool IsInUse { get; set; }
    }
}