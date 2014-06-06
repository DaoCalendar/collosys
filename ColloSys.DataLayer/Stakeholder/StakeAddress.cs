using ColloSys.DataLayer.BaseEntity;

namespace ColloSys.DataLayer.Stakeholder
{
    public class StakeAddress : Entity
    {
        public virtual Stakeholders Stakeholder { get; set; }
        public virtual string Line1 { get; set; }
        public virtual string Line2 { get; set; }
        public virtual string Line3 { get; set; }
        public virtual int Pincode { get; set; }
        public virtual string Country { get; set; }
        public virtual string StateCity { get; set; }
        public virtual string LandlineNo { get; set; }
    }
}