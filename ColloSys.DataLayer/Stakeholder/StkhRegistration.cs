using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;

namespace ColloSys.DataLayer.Stakeholder
{
    public class StkhRegistration : Entity
    {
        public virtual Stakeholders Stakeholder { get; set; }
        public virtual string RegistrationNo { get; set; }
        public virtual string PanNo { get; set; }
        public virtual string TanNo { get; set; }
        public virtual string ServiceTaxno { get; set; }
    }
}
