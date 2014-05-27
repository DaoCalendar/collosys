using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.ClientData;
using NHibernate.Hql.Ast.ANTLR;

namespace ColloSys.DataLayer.Billing
{
    class DHFL_CustInfoMap:EntityMap<DHFL_CustInfo>
    {
        public DHFL_CustInfoMap()
        {
            #region demo DHFL

            Property(x => x.LanNo);
            Property(x => x.SanctionAmt);
            Property(x => x.Month);
            Property(x => x.DisbAmt);
            Property(x => x.ProcessingFees);
            Property(x => x.TotalDisbAmt);
            Property(x => x.TotalProcFee);
            Property(x => x.Payout);
            Property(x => x.TotalPayout);
            Property(x => x.DeductCap);
            Property(x => x.DeductPf);
            Property(x => x.FinalPayout);
            Property(x => x.BranchName);
            Property(x => x.Branchcat);
            Property(x => x.ApplNo);
            Property(x => x.Loancode);
            Property(x => x.SalesRefNo);
            Property(x => x.Name);
            Property(x => x.SanctionDt);
            //Property(x => x.SanAmt);
            Property(x => x.DisbursementDt);
            //Property(x => x.DisbursementAmt);
            Property(x => x.FeeDue);
            Property(x => x.FeeWaived);
            Property(x => x.FeeReceived);
            Property(x => x.MemberName);
            Property(x => x.DesigName);
            Property(x => x.Orignateby);
            Property(x => x.Orignateby2);
            Property(x => x.Orignateby3);
            Property(x => x.Orignateby4);
            Property(x => x.Orignateby5);
            Property(x => x.Occupcategory);
            Property(x => x.Referraltype);
            Property(x => x.Referralname);
            Property(x => x.Referralcode);
            Property(x => x.Sourcename);
            Property(x => x.SchemeGroupName);
            Property(x => x.MSchname);
            Property(x => x.MSchnamePremium);
            Property(x=>x.DisbNo);
            Property(x=>x.Subvention);
            Property(x=>x.Product);
            Property(x=>x.Corporate);
            #endregion
        }

    }
}
