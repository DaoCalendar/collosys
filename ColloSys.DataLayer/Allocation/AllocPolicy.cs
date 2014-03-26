using System;
using System.Collections.Generic;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SharedDomain;

namespace ColloSys.DataLayer.Allocation
{
    public class AllocPolicy : Entity, IApproverComponent
    {
        public virtual IList<AllocRelation> AllocRelations { get; set; }
        public virtual IList<Allocations> Allocs { get; set; }

        public virtual string Name { get; set; }
        public virtual ScbEnums.Products Products { get; set; }
        public virtual ScbEnums.Category Category { get; set; }

        public virtual ColloSysEnums.ApproveStatus Status { get; set; }
        public virtual string Description { get; set; }
        public virtual string ApprovedBy { get; set; }
        public virtual DateTime? ApprovedOn { get; set; }
        public virtual Guid OrigEntityId { get; set; }
        public virtual RowStatus RowStatus { get; set; }
    }
}