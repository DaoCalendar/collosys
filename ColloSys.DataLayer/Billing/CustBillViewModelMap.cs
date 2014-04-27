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

            #region ICICI demo
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

            #endregion
        }
    }
}
