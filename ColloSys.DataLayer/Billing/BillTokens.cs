using ColloSys.DataLayer.BaseEntity;

namespace ColloSys.DataLayer.Billing
{
    public class BillTokens : Entity
    {
        public virtual string Type { get; set; }
        public virtual string Text { get; set; }
        public virtual string Value { get; set; }
        public virtual string DataType { get; set; }
        public virtual string GroupType { get; set; }
        public virtual int GroupId { get; set; }
        public virtual int Priority { get; set; }
        public virtual BillingSubpolicy BillingSubpolicy { get; set; }
    }
}
