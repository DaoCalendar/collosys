#region references

using System;
using System.Collections.Generic;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;

#endregion

namespace ColloSys.DataLayer.ClientData
{
    public class DHFL_Liner : UploadableEntity
    {
        public DHFL_Liner()
        {
            ExcludeReason = string.Empty;
        }

        #region Demo DHFL
      
        public virtual decimal TotalDisbAmt { get; set; }
        public virtual decimal TotalProcFee { get; set; }
        public virtual decimal Payout { get; set; }
        public virtual decimal TotalPayout { get; set; }
        public virtual decimal DeductCap { get; set; }
        public virtual decimal DeductPf { get; set; }
        public virtual decimal FinalPayout { get; set; }
        
        #endregion

        #region input file columns

        public virtual string BranchName { get; set; }
        public virtual string BranchCat { get; set; }
        public virtual uint ApplNo { get; set; }
        public virtual string Loancode { get; set; }
        public virtual uint SalesRefNo { get; set; }
        public virtual string Name { get; set; }
        public virtual DateTime SanctionDt { get; set; }
        public virtual decimal SanAmt { get; set; }
        public virtual DateTime DisbursementDt { get; set; }
        public virtual decimal DisbursementAmt { get; set; }
        public virtual decimal FeeDue { get; set; }
        public virtual decimal FeeWaived { get; set; }
        public virtual decimal FeeReceived { get; set; }
        public virtual string MemberName { get; set; }
        public virtual string DesigName { get; set; }
        public virtual string Orignateby { get; set; }
        public virtual string Orignateby2 { get; set; }
        public virtual string Orignateby3 { get; set; }
        public virtual string Orignateby4 { get; set; }
        public virtual string Orignateby5 { get; set; }
        public virtual string Occupcategory { get; set; }
        public virtual string Referraltype { get; set; }
        public virtual string Referralname { get; set; }
        public virtual string Referralcode { get; set; }
        public virtual string Sourcename { get; set; }
        public virtual string SchemeGroupName { get; set; }
        public virtual string M_Schname { get; set; }
        public virtual string Premium { get; set; }
        public virtual string DisbNo { get; set; }
        public virtual string Subvention { get; set; }
        public virtual string Corporate { get; set; }
        public virtual ScbEnums.Products Product { get; set; }
        public virtual string OrignateByFinal { get; set; }

        public virtual string AgentId { get; set; }
        public virtual UInt32 BillMonth { get; set; }
        public virtual ColloSysEnums.BillStatus BillStatus { get; set; }

        #endregion

        #region file upload
        public virtual BillDetail BillDetail { get; set; }
        public override FileScheduler FileScheduler { get; set; }
        public override DateTime FileDate { get; set; }
        public override ulong FileRowNo { get; set; }

        public override IList<string> GetExcludeInExcelProperties()
        {
            throw new NotImplementedException();
        }

        public override IList<string> GetWriteInExcelProperties(ColloSysEnums.FileAliasName? aliasName = null)
        {
            throw new NotImplementedException();
        }
        #endregion

        public virtual bool IsExcluded { get; set; }
        public virtual string ExcludeReason { get; set; }
    }
}
