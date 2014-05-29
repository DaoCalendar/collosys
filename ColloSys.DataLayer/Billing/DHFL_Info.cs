using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColloSys.DataLayer.BaseEntity;

namespace ColloSys.DataLayer.Billing
{
    class DHFL_Info:Entity
    {
        public virtual uint LoanNo { get; set; }
        public virtual ulong SanctionAmt { get; set; }
        public virtual DateTime Month { get; set; }
        public virtual ulong TotalDisbAmt { get; set; }
        public virtual ulong? TotalProcFee { get; set; }
        public virtual ulong? TotalPayout { get; set; }
        public virtual ulong? DeductCap { get; set; }
        public virtual ulong? DeductPF { get; set; }
    }
}
