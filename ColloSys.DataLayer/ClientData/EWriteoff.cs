#region References

using System;
using System.Collections.Generic;
using ColloSys.DataLayer.Allocation;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Generic;
using ColloSys.DataLayer.SharedDomain;

#endregion

namespace ColloSys.DataLayer.Domain
{
    public class EWriteoff : UploadableEntity, IDelinquentCustomer, IUniqueKey
    {
       
        public virtual decimal BounceCharge { get; set; }
        public virtual decimal FeeCharge { get; set; }
         public virtual string CustStatus { get; set; }
        public virtual bool IsSetteled { get; set; }
        public virtual decimal LateCharge { get; set; }
        public override DateTime FileDate { get; set; }
        public override ulong FileRowNo { get; set; }

        #region Properties
        public virtual string AccountNo { get; set; }
        public virtual DateTime ChargeOffDate { get; set; }
        public virtual string CustomerName { get; set; }
        public virtual decimal PrincipalDue { get; set; }
        public virtual decimal InterestCharge { get; set; }
        
        public virtual decimal TotalDue { get; set; }
        public virtual string ProductName { get; set; }
        public virtual ScbEnums.Products Product { get; set; }
        public virtual string Branch { get; set; }
        public virtual decimal FinalAmountDue { get; set; }
        
       
        public virtual DateTime? AllocStartDate { get; set; }
        public virtual DateTime? AllocEndDate { get; set; }
        public virtual string Remarks { get; set; }
        public virtual string Recoverydate_2002 { get; set; }
        public virtual string Amountrecovered_2002 { get; set; }

        public virtual string Recoverydate_2003 { get; set; }
        public virtual string Amountrecovered_2003 { get; set; }
        public virtual string BadDebtsIncurred_2003 { get; set; }

        public virtual string Recoverydate_2004 { get; set; }
        public virtual string Amountrecovered_2004 { get; set; }
        public virtual string BadDebtsIncurred_2004 { get; set; }

        public virtual string Recoverydate_2005 { get; set; }
        public virtual string Amountrecovered_2005 { get; set; }
        public virtual string BadDebtsIncurred_2005 { get; set; }

        public virtual string Recoverydate_2006 { get; set; }
        public virtual string Amountrecovered_2006 { get; set; }
        public virtual string BadDebtsIncurred_2006 { get; set; }

        public virtual string Recoverydate_2007 { get; set; }
        public virtual string Amountrecovered_2007 { get; set; }
        public virtual string BadDebtsIncurred_2007 { get; set; }

        public virtual string Recoverydate_2008 { get; set; }
        public virtual string Amountrecovered_2008 { get; set; }
        public virtual string BadDebtsIncurred_2008 { get; set; }

        public virtual string Recoverydate_2009 { get; set; }
        public virtual string Amountrecovered_2009 { get; set; }

        public virtual string Recoverydate_2010 { get; set; }
        public virtual string Amountrecovered_2010 { get; set; }
        public virtual string BadDebtsIncurred_2010 { get; set; }

        public virtual string Recoverydate_2011 { get; set; }
        public virtual string Amountrecovered_2011 { get; set; }
        public virtual string BadDebtsIncurred_2011 { get; set; }

        public virtual string Recoverydate_2012 { get; set; }
        public virtual string Amountrecovered_2012 { get; set; }
        public virtual string BadDebtsIncurred_2012 { get; set; }

        public virtual string Recoverydate_2013 { get; set; }
        public virtual string Amountrecovered_2013 { get; set; }
        public virtual string BadDebtsIncurred_2013 { get; set; }

        public virtual string SettlementY { get; set; }
        public virtual string DISPUTE { get; set; }
        public virtual string DONOTFOLLOW { get; set; }
        public virtual string SNo { get; set; }
        public virtual string RemarksNamechange { get; set; }
        public virtual string MANUALXHOLDINGCHGOFFREMARKS { get; set; }
        public virtual string SELLDOWNACTS { get; set; }
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
                memberHelper.GetName(x => x.Allocs)
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
                    memberHelper.GetName(x => x.FinalAmountDue ),
                    memberHelper.GetName(x => x.IsSetteled ),
                    memberHelper.GetName(x => x.CustStatus ),
                    memberHelper.GetName(x => x.Remarks ),
                    memberHelper.GetName(x => x.FileDate ),
                    memberHelper.GetName(x => x.FileRowNo ),


                };
        }

        //public override void MakeEmpty(bool forceEmpty = false)
        //{
        //    if (!NHibernateUtil.IsInitialized(EAllocs) || forceEmpty) EAllocs = null;
        //}

        public virtual IList<Allocations> Allocs { get; set; }

        #endregion
    }
}


