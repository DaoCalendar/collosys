using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.ClientData;

namespace ColloSys.DataLayer.Mapping
{
    public class CustInfoMap : EntityMap<CustInfo>
    {
        public CustInfoMap()
        {
            Property(x => x.LanNo);
            Property(x => x.Zone);
            Property(x => x.Region);
            Property(x => x.Location);
            Property(x => x.CustName);
            Property(x => x.SanctionAmt);
            Property(x => x.StartDate);
            Property(x => x.SanctionDate);
            Property(x => x.AgreementDate);
            Property(x => x.CustCat);
            Property(x => x.IRR);
            Property(x => x.Tenure);
            Property(x => x.RepaymentMode);
            Property(x => x.AssetCode);
            Property(x => x.AssetType);
            Property(x => x.Scheme);
            Property(x => x.DisbMemoNo);
            Property(x => x.DisbMemoDate);
            Property(x => x.ProcessingFees);
            Property(x => x.NetDisb);
            Property(x => x.DisbAmt);
            Property(x => x.DisbMode);
            Property(x => x.DisbStatus);
            Property(x => x.EmpIdCredit);
            Property(x => x.EmpIdOps);
            Property(x => x.LoanSource);
            Property(x => x.DMACode);
            Property(x => x.CityCat);
            Property(x => x.LoanType);
            Property(x => x.MemoApprovalDate);
        }
    }
}
