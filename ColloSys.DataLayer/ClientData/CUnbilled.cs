#region references

using System;
using System.Collections.Generic;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SharedDomain;

#endregion

namespace ColloSys.DataLayer.Domain
{
    public class CUnbilled : UploadableEntity, IUniqueKey
    {
        #region Relationship
        public virtual string AccountNo { get; set; }
        public virtual string CustomerName { get; set; }
        public virtual UInt32 OriginalTenure { get; set; }
        public virtual UInt32 RemainingTenure { get; set; }
        public virtual decimal InterestPct { get; set; }
        public virtual decimal LoanAmount { get; set; }
        public virtual decimal BilledAmount { get; set; }
        public virtual decimal UnbilledAmount { get; set; }
        public virtual decimal BilledInterest { get; set; }
        public virtual DateTime StartDate { get; set; }
        public virtual DateTime? EndDate { get; set; }

        public override DateTime FileDate { get; set; }
        public override ulong FileRowNo { get; set; }
        public override FileScheduler FileScheduler { get; set; }
        #endregion

        #region Relationship None

        public override IList<string> GetExcludeInExcelProperties()
        {
            var memberHelper = new MemberHelper<CUnbilled>();
            return new List<string> {
                memberHelper.GetName(x => x.Id),
                memberHelper.GetName(x => x.Version),
                memberHelper.GetName(x => x.CreatedBy),
                memberHelper.GetName(x => x.CreatedOn),
                memberHelper.GetName(x => x.CreateAction),
                memberHelper.GetName(x => x.FileScheduler),
            };
        }

        public override IList<string> GetWriteInExcelProperties(ColloSysEnums.FileAliasName? aliasName = null)
        {
            var memberHelper = new MemberHelper<CUnbilled>();
            return new List<string>
                {
                    memberHelper.GetName(x => x.AccountNo),
                    memberHelper.GetName(x => x.CustomerName),
                    memberHelper.GetName(x => x.OriginalTenure),
                    memberHelper.GetName(x => x.RemainingTenure ),
                    memberHelper.GetName(x => x.InterestPct),
                    memberHelper.GetName(x => x.LoanAmount),
                    memberHelper.GetName(x => x.BilledAmount),
                    memberHelper.GetName(x => x.UnbilledAmount),
                    memberHelper.GetName(x => x.BilledInterest),
                    memberHelper.GetName(x => x.StartDate),
                    memberHelper.GetName(x => x.EndDate),
                    memberHelper.GetName(x => x.FileDate),
                    memberHelper.GetName(x => x.FileRowNo),
                  
                 };
         }

        //public override void MakeEmpty(bool forceEmpty = false)
        //{
        //}
        #endregion
    }
}
