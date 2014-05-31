using ColloSys.DataLayer.BaseEntity;

namespace ColloSys.DataLayer.Billing
{
    public class DHFL_Info:Entity
    {
        public virtual string ApplNo { get; set; }
        public virtual decimal SanctionAmt { get; set; }
        public virtual decimal UpdateMonth { get; set; }
        public virtual decimal TotalDisbAmt { get; set; }
        public virtual decimal TotalProcFee { get; set; }
        public virtual decimal TotalPayout { get; set; }
        public virtual decimal DeductCap { get; set; }
        public virtual decimal DeductPf { get; set; }
    }
}
