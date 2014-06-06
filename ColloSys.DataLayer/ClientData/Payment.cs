#region references

using System;
using System.Collections.Generic;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SharedDomain;

#endregion

namespace ColloSys.DataLayer.ClientData
{
    public class Payment : UploadableEntity, IUniqueKey
    {
        #region Properties
        public virtual string AccountNo { get; set; }
        public virtual int TransCode { get; set; }
        public virtual DateTime TransDate { get; set; }
        public virtual string TransDesc { get; set; }
        public virtual decimal TransAmount { get; set; }
        public virtual bool IsDebit { get; set; }
        public override DateTime FileDate { get; set; }
        public override ulong FileRowNo { get; set; }
        public virtual ScbEnums.Products Products { get; set; }
        public virtual ColloSysEnums.BillStatus BillStatus { get; set; }
        public virtual DateTime? BillDate { get; set; }
        public virtual bool IsExcluded { get; set; }
        public virtual string ExcludeReason { get; set; }

        public virtual decimal DebitAmount { get; set; }
        public virtual decimal? CreditAmount { get; set; }

        public virtual ColloSysEnums.ApproveStatus Status { get; set; }
        public virtual string Description { get; set; }
        public virtual string ApprovedBy { get; set; }
        public virtual DateTime? ApprovedOn { get; set; }

        public override FileScheduler FileScheduler { get; set; }
        #endregion

        #region Relationship None

        protected IList<string> GetExcludeInExcelPaymentProperties()
        {
            var memberHelper = new MemberHelper<Payment>();
            return new List<string> {
                memberHelper.GetName(x => x.Id),
                memberHelper.GetName(x => x.Version),
                memberHelper.GetName(x => x.CreatedBy),
                memberHelper.GetName(x => x.CreatedOn),
                memberHelper.GetName(x => x.CreateAction),
                memberHelper.GetName(x => x.BillDate),
                memberHelper.GetName(x=>x.Products),
                memberHelper.GetName(x => x.BillStatus),
                memberHelper.GetName(x => x.Status),
                memberHelper.GetName(x => x.Description),
                memberHelper.GetName(x => x.FileScheduler),
                memberHelper.GetName(x => x.ApprovedBy),
                memberHelper.GetName(x => x.ApprovedOn)
            };
        }

        public override IList<string> GetExcludeInExcelProperties()
        {
            var memberHelper = new MemberHelper<Payment>();
            var list = GetExcludeInExcelPaymentProperties();
            list.Add(memberHelper.GetName(x => x.DebitAmount));
            return list;
        }

        public override IList<string> GetWriteInExcelProperties(ColloSysEnums.FileAliasName? aliasName = null)
        {
            var memberHelper = new MemberHelper<Payment>();
            return new List<string>
                {
                memberHelper.GetName(x => x.AccountNo ),
                memberHelper.GetName(x => x.TransCode ),
                memberHelper.GetName(x => x.TransDate ),
                memberHelper.GetName(x => x.TransDesc ),
                memberHelper.GetName(x => x.TransAmount ),
                memberHelper.GetName(x => x.IsDebit ),
                memberHelper.GetName(x => x.FileDate ),
                memberHelper.GetName(x => x.FileRowNo ),
                memberHelper.GetName(x => x.IsExcluded ),
                memberHelper.GetName(x => x.ExcludeReason ),
                memberHelper.GetName(x=>x.Products),
               
                };
        }

        //public override void MakeEmpty(bool forceEmpty = false)
        //{
        //}
        #endregion
    }
}
