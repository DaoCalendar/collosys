using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;

namespace ColloSys.DataLayer.Billing
{
    public class BillTokens : Entity
    {
        public virtual string Type { get; set; }
        public virtual string Text { get; set; }
        public virtual string Value { get; set; }
        public virtual string DataType { get; set; }
        public virtual string GroupId { get; set; }
        public virtual int Priority { get; set; }
        public virtual BillingSubpolicy BillingSubpolicy { get; set; }
    }
}
