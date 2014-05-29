using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColloSys.DataLayer.BaseEntity;

namespace ColloSys.DataLayer.Billing
{
    class DHFL_InfoMap :EntityMap<DHFL_Info>
    {
        public DHFL_InfoMap()
        {
            Property(x=>x.LoanNo);
            Property(x=>x.Month);
            Property(x=>x.SanctionAmt);
            Property(x=>x.TotalDisbAmt);
            Property(x=>x.TotalPayout);
            Property(x=>x.TotalProcFee);
            Property(x=>x.DeductCap);
            Property(x=>x.DeductPF);
            
        }
    }
}
