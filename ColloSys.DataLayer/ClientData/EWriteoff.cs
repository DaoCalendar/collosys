#region References

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
    public class EWriteoff : UploadableEntity, IDelinquentCustomer, IUniqueKey
    {

        #region Properties
        public virtual string AccountNo { get; set; }
        public virtual DateTime ChargeOffDate { get; set; }
        public virtual string CustomerName { get; set; }
        public virtual decimal PrincipalDue { get; set; }
        public virtual decimal InterestCharge { get; set; }
        public virtual decimal FeeCharge { get; set; }
        public virtual decimal BounceCharge { get; set; }
        public virtual decimal LateCharge { get; set; }
        public virtual decimal TotalDue { get; set; }
        public virtual string ProductName { get; set; }
        public virtual ScbEnums.Products Product { get; set; }
        public virtual string Branch { get; set; }
        public virtual decimal CurrentDue { get; set; }
        public virtual bool IsSetteled { get; set; }
        public virtual string CustStatus { get; set; }
        public virtual DateTime? AllocStartDate { get; set; }
        public virtual DateTime? AllocEndDate { get; set; }
        public virtual string Comments { get; set; }
        public override DateTime FileDate { get; set; }
        public override ulong FileRowNo { get; set; }

        //public virtual decimal AmountRepaid { get; set; }
        public virtual bool IsReferred { get; set; }
        public virtual uint Pincode { get; set; }
        public virtual ColloSysEnums.AllocStatus AllocStatus { get; set; }
        public virtual ColloSysEnums.NoAllocResons? NoAllocResons { get; set; }
        public virtual GPincode GPincode { get; set; }
        public override FileScheduler FileScheduler { get; set; }
        #endregion

        #region Relationship

        public override IList<string> GetExcludeInExcelProperties()
        {
            var memberHelper = new MemberHelper<EWriteoff>();
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
                memberHelper.GetName(x => x.EAllocs)
            };
        }

        public override IList<string> GetWriteInExcelProperties(ColloSysEnums.FileAliasName? aliasName = null)
        {
            var memberHelper = new MemberHelper<EWriteoff>();
            return new List<string>
                {
                    memberHelper.GetName(x => x.AccountNo),
                    memberHelper.GetName(x => x.ChargeOffDate),
                    memberHelper.GetName(x => x.CustomerName),
                    memberHelper.GetName(x => x.PrincipalDue),
                    memberHelper.GetName(x => x.InterestCharge),
                    memberHelper.GetName(x => x.FeeCharge ),
                    memberHelper.GetName(x => x.BounceCharge ),
                    memberHelper.GetName(x => x.LateCharge ),
                    memberHelper.GetName(x => x.TotalDue ),
                    memberHelper.GetName(x => x.ProductName ),
                    memberHelper.GetName(x => x.Product ),
                    memberHelper.GetName(x => x.Branch ),
                    memberHelper.GetName(x => x.CurrentDue ),
                    memberHelper.GetName(x => x.IsSetteled ),
                    memberHelper.GetName(x => x.CustStatus ),
                    memberHelper.GetName(x => x.Comments ),
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

    }
}


