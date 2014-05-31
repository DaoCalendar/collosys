using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColloSys.DataLayer.BaseEntity;

namespace ColloSys.DataLayer.Billing
{
    public class DHFL_Info : Entity
    {
        public virtual uint ApplNo { get; set; }
        public virtual string LoanNo { get; set; }
        public virtual decimal SanctionAmt { get; set; }
        public virtual UInt32 Month { get; set; }
        public virtual decimal TotalDisbAmt { get; set; }
        public virtual decimal TotalProcFee { get; set; }
        public virtual decimal TotalPayout { get; set; }
        public virtual decimal DeductCap { get; set; }
        public virtual decimal DeductPF { get; set; }
    }
}
