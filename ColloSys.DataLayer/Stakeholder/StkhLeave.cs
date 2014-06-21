using System;
using ColloSys.DataLayer.BaseEntity;

namespace ColloSys.DataLayer.Stakeholder
{
    public class StkhLeave : Entity
    {
        public virtual Stakeholders Stakeholder { get; set; }
        public virtual DateTime FromDate { get; set; }
        public virtual DateTime ToDate { get; set; }
        public virtual Stakeholders DelegatedTo { get; set; }
    }
}