#region References
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SharedDomain;

#endregion

namespace ColloSys.DataLayer.Domain
{
    public class CacsActivity : UploadableEntity, IUniqueKey
    {
        #region Property
        public override FileScheduler FileScheduler { get; set; }

        public virtual string TelecallerId { get; set; }

        public virtual string AccountNo { get; set; }

        public virtual ScbEnums.Products Products { get; set; }

        public virtual string Region { get; set; }

        public virtual DateTime CallDateTime { get; set; }

        public virtual uint CallDuration { get; set; }

        public virtual string CallDirection { get; set; }

        public virtual string CallLocation { get; set; }

        public virtual string CallResponce { get; set; }

        public virtual string ActivityCode { get; set; }

        public virtual DateTime? Ptp1Date { get; set; }

        public virtual decimal? Ptp1Amt { get; set; }

        public virtual DateTime? Ptp2Date { get; set; }

        public virtual decimal? Ptp2Amt { get; set; }

        public virtual string ExcuseCode { get; set; }

        public virtual bool MissingBasicInfo { get; set; }

        public override DateTime FileDate { get; set; }

        public override ulong FileRowNo { get; set; }

        public virtual bool ConsiderInAllocation { get; set; }
        #endregion

        #region Relationship None

        public override IList<string> GetExcludeInExcelProperties()
        {
            var memberHelper = new MemberHelper<CacsActivity>();
            return new List<string> {
                memberHelper.GetName(x => x.Id),
                memberHelper.GetName(x => x.Version),
                memberHelper.GetName(x => x.CreatedBy),
                memberHelper.GetName(x => x.CreatedOn),
                memberHelper.GetName(x => x.CreateAction),
                memberHelper.GetName(x => x.ConsiderInAllocation),
                memberHelper.GetName(x => x.MissingBasicInfo),
                memberHelper.GetName(x => x.FileScheduler)
            };
        }

        public override IList<string> GetWriteInExcelProperties(ColloSysEnums.FileAliasName? aliasName = null)
        {
            var memberHelper = new MemberHelper<CacsActivity>();
            return new List<string> {
                memberHelper.GetName(x => x.TelecallerId),
                memberHelper.GetName(x => x.AccountNo),
                memberHelper.GetName(x => x.Products)
            };
        }

        //public override void MakeEmpty(bool forceEmpty = false)
        //{
        //}
        #endregion
    }
}