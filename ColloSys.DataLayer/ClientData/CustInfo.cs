using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Enumerations;

namespace ColloSys.DataLayer.ClientData
{
    public class CustInfo : Entity
    {
        public virtual string LanNo { get; set; }
        public virtual string Zone { get; set; }
        public virtual string Region { get; set; }
        public virtual string Location { get; set; }
        public virtual string CustName { get; set; }
        public virtual uint SanctionAmt { get; set; }
        public virtual DateTime StartDate { get; set; }
        public virtual DateTime SanctionDate { get; set; }
        public virtual DateTime AgreementDate { get; set; }
        public virtual string CustCat { get; set; }
        public virtual decimal IRR { get; set; }
        public virtual uint Tenure { get; set; }
        public virtual string RepaymentMode { get; set; }
        public virtual uint AssetCode { get; set; }
        public virtual ColloSysEnums.DelqFlag AssetType { get; set; }
        public virtual string Scheme { get; set; }
        public virtual string DisbMemoNo { get; set; }
        public virtual DateTime DisbMemoDate { get; set; }
        public virtual uint ProcessingFees { get; set; }
        public virtual uint NetDisb { get; set; }
        public virtual uint DisbAmt { get; set; }
        public virtual string DisbMode { get; set; }
        public virtual string DisbStatus { get; set; }
        public virtual uint EmpIdCredit { get; set; }
        public virtual string EmpIdOps { get; set; }
        public virtual string LoanSource { get; set; }
        public virtual uint DMACode { get; set; }
        public virtual ColloSysEnums.CityCategory CityCat { get; set; }
        public virtual string LoanType { get; set; }
        public virtual DateTime MemoApprovalDate { get; set; }
    }
}
