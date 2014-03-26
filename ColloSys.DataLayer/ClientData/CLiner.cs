#region references

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ColloSys.DataLayer.Allocation;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SharedDomain;
using Iesi.Collections.Generic;
using NHibernate;

#endregion

namespace ColloSys.DataLayer.Domain
{
    public class CLiner : UploadableEntity, ISoftDelq, IUniqueKey
    {
        #region Properties
        public virtual ColloSysEnums.DelqFlag Flag { get; set; }
        public virtual string GlobalCustId { get; set; }
        public virtual string AccountNo { get; set; }
        public virtual string CustomerName { get; set; }
        public virtual uint Cycle { get; set; }
        public virtual decimal CreditLimit { get; set; }
        public virtual decimal UnbilledDue { get; set; }
        public virtual decimal CurrentBalance { get; set; }
        public virtual decimal TotalDue { get; set; }
        public virtual uint PeakBucket { get; set; }
        public virtual ColloSysEnums.DelqAccountStatus AccountStatus { get; set; }
        public virtual uint Bucket { get; set; }
        public virtual decimal BucketAmount { get; set; }
        public virtual string Block { get; set; }
        public virtual string AltBlock { get; set; }
        public virtual decimal LastPayAmount { get; set; }
        public virtual DateTime? LastPayDate { get; set; }
        public virtual decimal CurrentDue { get; set; }
        public virtual decimal Bucket0Due { get; set; }
        public virtual decimal Bucket1Due { get; set; }
        public virtual decimal Bucket2Due { get; set; }
        public virtual decimal Bucket3Due { get; set; }
        public virtual decimal Bucket4Due { get; set; }
        public virtual decimal Bucket5Due { get; set; }
        public virtual decimal Bucket6Due { get; set; }
        public virtual decimal Bucket7Due { get; set; }
        public virtual decimal OutStandingBalance { get; set; }
        public virtual string Location { get; set; }
        public virtual string DelqHistoryString { get; set; }
        public virtual decimal CustTotalDue { get; set; }
        public virtual string CustStatus { get; set; }
        public virtual DateTime? AllocStartDate { get; set; }
        public virtual DateTime? AllocEndDate { get; set; }
        public override DateTime FileDate { get; set; }
        public override ulong FileRowNo { get; set; }

        public virtual bool IsReferred { get; set; }
        public virtual uint Pincode { get; set; }
        public virtual ScbEnums.Products Product { get; set; }
        public virtual GPincode GPincode { get; set; }
        public virtual ColloSysEnums.AllocStatus AllocStatus { get; set; }
        public virtual ColloSysEnums.NoAllocResons? NoAllocResons { get; set; }
        public override FileScheduler FileScheduler { get; set; }
        #endregion

        #region Relationship

        public override IList<string> GetExcludeInExcelProperties()
        {
            var memberHelper = new MemberHelper<CLiner>();
            return new List<string> {
                memberHelper.GetName(x => x.Id),
                memberHelper.GetName(x => x.Version),
                memberHelper.GetName(x => x.CreatedBy),
                memberHelper.GetName(x => x.CreatedOn),
                memberHelper.GetName(x => x.CreateAction),
                memberHelper.GetName(x => x.IsReferred),
                memberHelper.GetName(x => x.Pincode),
                memberHelper.GetName(x => x.AllocStatus),
                memberHelper.GetName(x => x.NoAllocResons),
                memberHelper.GetName(x => x.FileScheduler),
                memberHelper.GetName(x => x.GPincode),
                memberHelper.GetName(x => x.Allocs),
                memberHelper.GetName(x => x.Product)
            };
        }

        public override IList<string> GetWriteInExcelProperties(ColloSysEnums.FileAliasName? aliasName = null)
        {
            var memberHelper = new MemberHelper<CLiner>();
            return new List<string>
                {
                    memberHelper.GetName(x => x.Flag),
                     memberHelper.GetName(x => x.GlobalCustId ),
                     memberHelper.GetName(x => x.AccountNo),
                     memberHelper.GetName(x => x.CustomerName),
                     memberHelper.GetName(x => x.Cycle),
                     memberHelper.GetName(x => x.CreditLimit),
                     memberHelper.GetName(x => x.UnbilledDue),
                     memberHelper.GetName(x => x.CurrentBalance),
                     memberHelper.GetName(x => x.TotalDue),
                     memberHelper.GetName(x => x.PeakBucket),
                     memberHelper.GetName(x => x.AccountStatus),
                     memberHelper.GetName(x => x.Bucket),
                     memberHelper.GetName(x => x.BucketAmount),
                     memberHelper.GetName(x => x.Block),
                     memberHelper.GetName(x => x.AltBlock),
                     memberHelper.GetName(x => x.LastPayAmount),
                     memberHelper.GetName(x => x.LastPayDate),
                     memberHelper.GetName(x => x.CurrentDue),
                     memberHelper.GetName(x => x.Bucket0Due),
                     memberHelper.GetName(x => x.Bucket1Due),
                     memberHelper.GetName(x => x.Bucket2Due),
                     memberHelper.GetName(x => x.Bucket3Due),
                     memberHelper.GetName(x => x.Bucket4Due),
                     memberHelper.GetName(x => x.Bucket5Due),
                     memberHelper.GetName(x => x.Bucket6Due),
                     memberHelper.GetName(x => x.Bucket7Due),
                     memberHelper.GetName(x => x.OutStandingBalance),
                     memberHelper.GetName(x => x.Location ),
                     memberHelper.GetName(x => x.DelqHistoryString ),
                     memberHelper.GetName(x => x.CustTotalDue),
                     memberHelper.GetName(x => x.CustStatus ),
                     memberHelper.GetName(x => x.FileDate ),
                     memberHelper.GetName(x => x.FileRowNo )

                };
        }

        public virtual Iesi.Collections.Generic.ISet<Allocations> Allocs { get; set; }

        //public override void MakeEmpty(bool forceEmpty = false)
        //{
        //    if (!NHibernateUtil.IsInitialized(CAllocs) || forceEmpty) CAllocs = null;
        //}

        #endregion
    }
}