using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Billing;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;

namespace ColloSys.DataLayer.Mapping
{
    class CustBillViewModelMap : EntityMap<CustBillViewModel>
    {
        public CustBillViewModelMap()
        {
            #region SCB
            Property(x => x.AccountNo);
            Property(x => x.GlobalCustId);
            Property(x => x.Flag);
            Property(x => x.Product);
            Property(x => x.IsInRecovery);
            Property(x => x.ChargeofDate);
            Property(x => x.Cycle);
            Property(x => x.Bucket);
            Property(x => x.MobWriteoff);
            Property(x => x.MobWriteoff);
            Property(x => x.Vintage);
            Property(x => x.CityCategory);
            Property(x => x.City);
            Property(x => x.IsXHoldAccount);
            Property(x => x.AllocationStartDate);
            Property(x => x.AllocationEndDate);
            Property(x => x.TotalDueOnAllocation);
            Property(x => x.TotalAmountRecovered);
            Property(x => x.ResolutionPercentage);
            Property(x => x.ConditionSatisfy, map => map.Length(4001));

            ManyToOne(x => x.BillDetail);
            ManyToOne(x => x.GPincode);
            ManyToOne(x => x.Stakeholders);
            #endregion

            #region demo DHFL
            Property(x => x.LanNo);
            Property(x => x.SanctionAmt);
            Property(x=>x.Month);
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
            Property(x => x.SanAmt);
            Property(x => x.DisbursementDt);
            Property(x => x.DisbursementAmt);
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
            Property(x => x.OCCUPCATEGORY);
            Property(x => x.REFERRALTYPE);
            Property(x => x.REFERRALNAME);
            Property(x => x.REFERRALCODE);
            Property(x => x.SOURCENAME);
            Property(x => x.SchemeGroupName);
            Property(x => x.M_SCHNAME);
            Property(x => x.M_SCHNAMEPremium);
            #endregion
        }
    }
}
