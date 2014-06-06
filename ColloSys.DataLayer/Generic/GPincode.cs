using System.Collections.Generic;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.SharedDomain;

namespace ColloSys.DataLayer.Generic
{
    public class GPincode : Entity
    {
        public virtual IList<CLiner> CLiners { get; set; }
        public virtual IList<CWriteoff> CWriteoffs { get; set; }
        public virtual IList<CustomerInfo> Infos { get; set; }
        public virtual IList<RLiner> RLiners { get; set; }
        public virtual IList<RWriteoff> RWriteoffs { get; set; }
        public virtual IList<ELiner> ELiners { get; set; }
        public virtual IList<EWriteoff> EWriteoffs { get; set; }

        public virtual string Country { get; set; }
        public virtual uint Pincode { get; set; }
        public virtual string Area { get; set; }
        public virtual string City { get; set; }
        public virtual string District { get; set; }
        public virtual string Cluster { get; set; }
        public virtual string State { get; set; }
        public virtual string Region { get; set; }
        public virtual bool IsInUse { get; set; }
        public virtual string CityCategory { get; set; }
    }
}
