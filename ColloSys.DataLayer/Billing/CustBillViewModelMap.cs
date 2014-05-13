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
            //#region SCB
            //Property(x => x.AccountNo);
            //Property(x => x.GlobalCustId);
            //Property(x => x.Flag);
            //Property(x => x.Product);
            //Property(x => x.IsInRecovery);
            //Property(x => x.ChargeofDate);
            //Property(x => x.Cycle);
            //Property(x => x.Bucket);
            //Property(x => x.MobWriteoff);
            //Property(x => x.MobWriteoff);
            //Property(x => x.Vintage);
            //Property(x => x.CityCategory);
            //Property(x => x.City);
            //Property(x => x.IsXHoldAccount);
            //Property(x => x.AllocationStartDate);
            //Property(x => x.AllocationEndDate);
            //Property(x => x.TotalDueOnAllocation);
            //Property(x => x.TotalAmountRecovered);
            //Property(x => x.ResolutionPercentage);
            //Property(x => x.ConditionSatisfy, map => map.Length(4001));

            //ManyToOne(x => x.BillDetail);
            //ManyToOne(x => x.GPincode);
            //ManyToOne(x => x.Stakeholders);
            //#endregion

            #region SCB
            Property(x => x.AccountNo);
            Property(x => x.GlobalCustId, map => map.NotNullable(false));
            Property(x => x.Flag);
            Property(x => x.Product);
            Property(x => x.IsInRecovery, map => map.NotNullable(false));
            Property(x => x.ChargeofDate);
            Property(x => x.Cycle, map => map.NotNullable(false));
            Property(x => x.Bucket, map => map.NotNullable(false));
            Property(x => x.MobWriteoff, map => map.NotNullable(false));

            Property(x => x.Vintage, map => map.NotNullable(false));
            Property(x => x.CityCategory, map => map.NotNullable(false));
            Property(x => x.City, map => map.NotNullable(false));
            Property(x => x.IsXHoldAccount, map => map.NotNullable(false));
            Property(x => x.AllocationStartDate, map => map.NotNullable(false));
            Property(x => x.AllocationEndDate, map => map.NotNullable(false));
            Property(x => x.TotalDueOnAllocation, map => map.NotNullable(false));
            Property(x => x.TotalAmountRecovered, map => map.NotNullable(false));
            Property(x => x.ResolutionPercentage, map => map.NotNullable(false));
            Property(x => x.ConditionSatisfy, map => map.Length(4001));

            ManyToOne(x => x.BillDetail,map=>map.NotNullable(false));
            ManyToOne(x => x.GPincode,map=>map.NotNullable(false));
            ManyToOne(x => x.Stakeholders,map=>map.NotNullable(false));
            #endregion

            #region demo DHFL

            //Property(x => x.LanNo);
            //Property(x => x.SanctionAmt);
            //Property(x=>x.Month);
            //Property(x => x.DisbAmt);
            //Property(x => x.ProcessingFees);
            //Property(x => x.TotalDisbAmt);
            //Property(x => x.TotalProcFee);
            //Property(x => x.Payout);
            //Property(x => x.TotalPayout);
            //Property(x => x.DeductCap);
            //Property(x => x.DeductPf);
            //Property(x => x.FinalPayout);
            //Property(x => x.BranchName);
            //Property(x => x.Branchcat);
            //Property(x => x.ApplNo);
            //Property(x => x.Loancode);
            //Property(x => x.SalesRefNo);
            //Property(x => x.Name);
            //Property(x => x.SanctionDt);
            ////Property(x => x.SanAmt);
            //Property(x => x.DisbursementDt);
            ////Property(x => x.DisbursementAmt);
            //Property(x => x.FeeDue);
            //Property(x => x.FeeWaived);
            //Property(x => x.FeeReceived);
            //Property(x => x.MemberName);
            //Property(x => x.DesigName);
            //Property(x => x.Orignateby);
            //Property(x => x.Orignateby2);
            //Property(x => x.Orignateby3);
            //Property(x => x.Orignateby4);
            //Property(x => x.Orignateby5);
            //Property(x => x.Occupcategory);
            //Property(x => x.Referraltype);
            //Property(x => x.Referralname);
            //Property(x => x.Referralcode);
            //Property(x => x.Sourcename);
            //Property(x => x.SchemeGroupName);
            //Property(x => x.MSchname);
            //Property(x => x.MSchnamePremium);
            #endregion


            #region demo DHFL

            Property(x => x.LanNo);
            Property(x => x.SanctionAmt,map=>map.NotNullable(false));
            Property(x => x.Month, map => map.NotNullable(false));
            Property(x => x.DisbAmt, map => map.NotNullable(false));
            Property(x => x.ProcessingFees, map => map.NotNullable(false));
            Property(x => x.TotalDisbAmt, map => map.NotNullable(false));
            Property(x => x.TotalProcFee, map => map.NotNullable(false));
            Property(x => x.Payout, map => map.NotNullable(false));
            Property(x => x.TotalPayout, map => map.NotNullable(false));
            Property(x => x.DeductCap, map => map.NotNullable(false));
            Property(x => x.DeductPf, map => map.NotNullable(false));
            Property(x => x.FinalPayout, map => map.NotNullable(false));
            Property(x=>x.SubProduct,map=>map.NotNullable(false));
            Property(x => x.TotalPf);
            Property(x => x.CustomerType,map=>map.NotNullable(false));
            Property(x => x.BranchName, map => map.NotNullable(false));
            Property(x => x.Branchcat, map => map.NotNullable(false));
            Property(x => x.ApplNo, map => map.NotNullable(false));
            Property(x => x.Loancode, map => map.NotNullable(false));
            Property(x => x.SalesRefNo, map => map.NotNullable(false));
            Property(x => x.Name, map => map.NotNullable(false));
            Property(x => x.SanctionDt, map => map.NotNullable(false));
            //Property(x => x.SanAmt);
            Property(x => x.DisbursementDt, map => map.NotNullable(false));
            //Property(x => x.DisbursementAmt);
            Property(x => x.FeeDue, map => map.NotNullable(false));
            Property(x => x.FeeWaived, map => map.NotNullable(false));
            Property(x => x.FeeReceived, map => map.NotNullable(false));
            Property(x => x.MemberName, map => map.NotNullable(false));
            Property(x => x.DesigName, map => map.NotNullable(false));
            Property(x => x.Orignateby, map => map.NotNullable(false));
            Property(x => x.Orignateby2, map => map.NotNullable(false));
            Property(x => x.Orignateby3, map => map.NotNullable(false));
            Property(x => x.Orignateby4, map => map.NotNullable(false));
            Property(x => x.Orignateby5, map => map.NotNullable(false));
            Property(x => x.Occupcategory, map => map.NotNullable(false));
            Property(x => x.Referraltype, map => map.NotNullable(false));
            Property(x => x.Referralname,map=>map.NotNullable(false));
            Property(x => x.Referralcode, map => map.NotNullable(false));
            Property(x => x.Sourcename, map => map.NotNullable(false));
            Property(x => x.SchemeGroupName, map => map.NotNullable(false));
            Property(x => x.MSchname, map => map.NotNullable(false));
            Property(x => x.MSchnamePremium,map=>map.NotNullable(false));
            #endregion
        }
    }
}
