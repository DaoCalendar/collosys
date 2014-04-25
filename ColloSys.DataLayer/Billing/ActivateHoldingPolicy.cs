using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;

namespace ColloSys.DataLayer.Mapping
{
    public class ActivateHoldingPolicy : Entity
    {
        public virtual HoldingPolicy HoldingPolicy { get; set; }
        public virtual Stakeholders Stakeholder { get; set; }
        public virtual ScbEnums.Products Products { get; set; }
        public virtual int StartMonth { get; set; }
    }
}