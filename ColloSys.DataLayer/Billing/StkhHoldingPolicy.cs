using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Mapping;

namespace ColloSys.DataLayer.Billing
{
    public class StkhHoldingPolicy : Entity
    {
        public virtual HoldingPolicy HoldingPolicy { get; set; }
        public virtual Stakeholders Stakeholder { get; set; }
        public virtual ScbEnums.Products Products { get; set; }
        public virtual int StartMonth { get; set; }
    }
}