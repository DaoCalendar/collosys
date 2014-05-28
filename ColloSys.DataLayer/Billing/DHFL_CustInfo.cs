using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;

namespace ColloSys.DataLayer.ClientData
{
    public  class DHFL_CustInfo :UploadableEntity
    {
        #region Demo DHFL
        public virtual string LanNo { get; set; }
        public virtual ulong SanctionAmt { get; set; }
        public virtual DateTime Month { get; set; }
        public virtual ulong DisbAmt { get; set; }
        public virtual ulong? ProcessingFees { get; set; }
        public virtual ulong TotalDisbAmt { get; set; }
        public virtual ulong TotalProcFee { get; set; }
        public virtual ulong Payout { get; set; }
        public virtual ulong TotalPayout { get; set; }
        public virtual ulong DeductCap { get; set; }
        public virtual ulong DeductPf { get; set; }
        public virtual ulong FinalPayout { get; set; }

        public virtual string SubProduct { get; set; }

        public virtual ulong TotalPf { get; set; }

        public virtual string CustomerType { get; set; }

        #region input file columns

        public virtual string BranchName { get; set; }
        public virtual string Branchcat { get; set; }
        public virtual uint ApplNo { get; set; }
        public virtual uint Loancode { get; set; }
        public virtual uint SalesRefNo { get; set; }
        public virtual string Name { get; set; }
        public virtual DateTime SanctionDt { get; set; }
        //public virtual ulong SanAmt { get; set; }
        public virtual DateTime DisbursementDt { get; set; }
        //public virtual ulong DisbursementAmt { get; set; }
        public virtual uint FeeDue { get; set; }
        public virtual uint FeeWaived { get; set; }
        public virtual uint FeeReceived { get; set; }
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
        public virtual string MSchname { get; set; }
        public virtual string MSchnamePremium { get; set; }
        public virtual string DisbNo { get; set; }
        public virtual string Subvention { get; set; }
        public virtual string Corporate { get; set; }
        public virtual string Product { get; set; }
        public virtual string AgentId { get; set; }

        #endregion

        #endregion

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
    }
}
