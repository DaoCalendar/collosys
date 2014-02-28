#region Reference

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
    public class ELiner : UploadableEntity, ISoftDelq, IUniqueKey
    {
        #region Properties
        public virtual ColloSysEnums.DelqFlag Flag { get; set; }
        public virtual string ProductCode { get; set; }
        public virtual string AccountNo { get; set; }
        public virtual bool IsSetteled { get; set; }
        public virtual DateTime? AccountOpenDate { get; set; }
        public virtual ScbEnums.Products Product { get; set; }
        public virtual decimal TotalDue { get; set; }
        public virtual decimal CurrentDue { get; set; }
        public virtual decimal AmountRepaid { get; set; }
        public virtual DateTime? ExpirtyDate { get; set; }
        public virtual decimal OdLimit { get; set; }
        public virtual uint Cycle { get; set; }
        public virtual uint DayPastDue { get; set; }
        public virtual string CustomerName { get; set; }
        public virtual decimal InterestCharge { get; set; }
        public virtual decimal FeeCharge { get; set; }
        public virtual decimal InterestPct { get; set; }
        public virtual decimal MinimumDue { get; set; }
        public virtual uint Bucket { get; set; } //sd
        public virtual decimal BucketDue { get; set; }
        public virtual decimal Bucket1Due { get; set; }
        public virtual decimal Bucket2Due { get; set; }
        public virtual decimal Bucket3Due { get; set; }
        public virtual decimal Bucket4Due { get; set; }
        public virtual decimal Bucket5Due { get; set; }
        public virtual uint PeakBucket { get; set; }
        public virtual string DelqHistoryString { get; set; }
        public virtual ColloSysEnums.DelqAccountStatus AccountStatus { get; set; }
        public virtual string CustStatus { get; set; }
        public virtual DateTime? AllocStartDate { get; set; }
        public virtual DateTime? AllocEndDate { get; set; }
        public override DateTime FileDate { get; set; }
        public override ulong FileRowNo { get; set; }

        public virtual bool IsReferred { get; set; }  //cmi
        public virtual uint Pincode { get; set; }//cmi//add
        public virtual ColloSysEnums.AllocStatus AllocStatus { get; set; }
        public virtual ColloSysEnums.NoAllocResons? NoAllocResons { get; set; }//cmi//add
        public virtual GPincode GPincode { get; set; } //cmi//add
        public override FileScheduler FileScheduler { get; set; }
        #endregion

        #region Relationship

        public override IList<string> GetExcludeInExcelProperties()
        {
            var memberHelper = new MemberHelper<ELiner>();
            return new List<string>
                {
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
                    memberHelper.GetName(x => x.CycleDate),
                    memberHelper.GetName(x => x.CurrentBalance),
                    memberHelper.GetName(x => x.EAllocs)
                };
        }

        public override IList<string> GetWriteInExcelProperties(ColloSysEnums.FileAliasName? aliasName = null)
        {
            var memberHelper = new MemberHelper<ELiner>();
            return new List<string>
                {
                    memberHelper.GetName(x => x.Flag),
                    memberHelper.GetName(x => x.ProductCode),
                    memberHelper.GetName(x => x.AccountNo),
                    memberHelper.GetName(x => x.IsSetteled),
                    memberHelper.GetName(x => x.AccountOpenDate),
                    memberHelper.GetName(x => x.Product),
                    memberHelper.GetName(x => x.TotalDue),
                    memberHelper.GetName(x => x.CurrentDue),
                    memberHelper.GetName(x => x.AmountRepaid),
                    memberHelper.GetName(x => x.ExpirtyDate ),
                    memberHelper.GetName(x => x.OdLimit ),
                    memberHelper.GetName(x => x.Cycle ),
                    memberHelper.GetName(x => x.DayPastDue ),
                    memberHelper.GetName(x => x.CustomerName ),
                    memberHelper.GetName(x => x.InterestCharge ),
                    memberHelper.GetName(x => x.FeeCharge ),
                    memberHelper.GetName(x => x.InterestPct ),
                    memberHelper.GetName(x => x.MinimumDue ),
                    memberHelper.GetName(x => x.Bucket ),
                    memberHelper.GetName(x => x.BucketDue),
                    memberHelper.GetName(x => x.Bucket1Due ),
                    memberHelper.GetName(x => x.Bucket2Due ),
                    memberHelper.GetName(x => x.Bucket3Due ),
                    memberHelper.GetName(x => x.Bucket4Due ),
                    memberHelper.GetName(x => x.Bucket5Due ),
                    memberHelper.GetName(x => x.PeakBucket),
                    memberHelper.GetName(x => x.DelqHistoryString ),
                    memberHelper.GetName(x => x.AccountStatus ),
                    memberHelper.GetName(x => x.CustStatus ),
                    memberHelper.GetName(x => x.FileDate ),
                    memberHelper.GetName(x => x.FileRowNo ),

                };
        }

        //public override void MakeEmpty(bool forceEmpty = false)
        //{
        //    if (!NHibernateUtil.IsInitialized(EAllocs) || forceEmpty) EAllocs = null;
        //}
        public virtual Iesi.Collections.Generic.ISet<EAlloc> EAllocs { get; set; }

        #endregion

        #region Unmapped Property
        public virtual DateTime CycleDate { get; set; }
        public virtual decimal CurrentBalance { get; set; }
        #endregion
    }
}

