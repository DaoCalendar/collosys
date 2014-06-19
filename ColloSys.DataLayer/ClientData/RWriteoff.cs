using ColloSys.DataLayer.Generic;

#region references

using System;
using System.Collections.Generic;
using ColloSys.DataLayer.Allocation;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SharedDomain;

#endregion

// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable DoNotCallOverridableMethodsInConstructor
// ReSharper disable ClassWithVirtualMembersNeverInherited.Global
// ReSharper disable CheckNamespace
namespace ColloSys.DataLayer.Domain
{
    public class RWriteoff : UploadableEntity, IDelinquentCustomer, IUniqueKey
    {
        #region Properties
        public virtual string AccountNo { get; set; }
        public virtual string CustomerName { get; set; }
        public virtual DateTime? DisbursementDate { get; set; }
        public virtual DateTime? FinalInstDate { get; set; }
        public virtual uint Cycle { get; set; }
        public virtual string Branch { get; set; }
        public virtual DateTime? ChargeOffDate { get; set; }
        public virtual string ProductName { get; set; }
        public virtual ScbEnums.Products Product { get; set; }
        public virtual decimal BounceCharge { get; set; }
        public virtual decimal LateCharge { get; set; }
        public virtual decimal PrincipalDue { get; set; }
        public virtual decimal InterestCharge { get; set; }
        public virtual decimal FeeCharge { get; set; }
        public virtual decimal TotalDue { get; set; }
        public virtual decimal Recovery { get; set; }
        public virtual decimal CurrentDue { get; set; }
        public virtual bool IsSetteled { get; set; }
        public virtual string Comment { get; set; }
        public virtual string CustStatus { get; set; }
        public virtual DateTime? AllocStartDate { get; set; }
        public virtual DateTime? AllocEndDate { get; set; }
        public override DateTime FileDate { get; set; }
        public override ulong FileRowNo { get; set; }

        public virtual bool IsReferred { get; set; }
        public virtual uint Pincode { get; set; }
        public virtual ColloSysEnums.AllocStatus AllocStatus { get; set; }
        public virtual ColloSysEnums.NoAllocResons? NoAllocResons { get; set; }
        public virtual GPincode GPincode { get; set; }
        public override FileScheduler FileScheduler { get; set; }
        #endregion

        #region "RWriteoff_Properties"
        // tempTable=> TEMP_E_LINER_AUTO
        public virtual string DiscProv { get; set; }
        public virtual string AmtrecvdDATE_2005 { get; set; }
        public virtual string AmtrecvdMSA81183_2005 { get; set; }
        public virtual string BDincurredMSA81131_2005 { get; set; }
        public virtual string AmtrecvdDATE_2006 { get; set; } //  S/FWITHDEBITPOSTCOFF
        public virtual string AmtrecvdMSA81183_2006 { get; set; }
        public virtual string BDincurredMSA81131_2006 { get; set; }
        public virtual string AmtrecvdDATE_2007 { get; set; }
        public virtual string AmtrecvdMSA81183_2007 { get; set; }
        public virtual string BDincurredMSA81131_2007 { get; set; }
        public virtual string AmtrecvdDATE_2008 { get; set; }
        public virtual string AmtrecvdMSA81183_2008 { get; set; }
        public virtual string BDincurredMSA81131_2008 { get; set; }
        public virtual string AmtrecvdDATE_2009 { get; set; }
        public virtual string AmtrecvdMSA81183_2009 { get; set; }
        public virtual string AmtrecvdDATE_2010 { get; set; }
        public virtual string AmtrecvdMSA81183_2010 { get; set; }
        public virtual string BDincurredMSA81131_2010 { get; set; }
        public virtual string AmtrecvdDATE_2011 { get; set; }
        public virtual string AmtrecvdMSA81183_2011 { get; set; }
        public virtual string BDincurredMSA81131_2011 { get; set; }
        public virtual string AmtrecvdDATE_2012 { get; set; }
        public virtual string AmtrecvdMSA81183_2012 { get; set; }
        public virtual string BDincurredMSA81131_2012 { get; set; }
        public virtual string AmtrecvdDATE_2013 { get; set; }
        public virtual string AmtrecvdMSA81183_2013 { get; set; }
        public virtual string BDincurredMSA81131_2013 { get; set; }
        public virtual string SFWITHDEBITPOSTCOFF { get; set; }
        public virtual string Settlement { get; set; }
        public virtual string REMARKS_43 { get; set; }
        public virtual string ECS { get; set; }
        public virtual string PDC { get; set; }
        public virtual string APPBRANCHCODE { get; set; }
        public virtual string Dispute { get; set; }
        public virtual string DonotFollow { get; set; }
        public virtual string COReason { get; set; }// C/OReason
        public virtual string Remarks_48 { get; set; }

        // TempTable=> TEMP_R_WRITEOFF_PL_AEB
        public virtual string RECOVERYASPERAMEX { get; set; }
        public virtual string BDincurredMSA81131_2002 { get; set; }
        public virtual string RECOVERDT_2008 { get; set; }
        public virtual string RECOVERDT_2002 { get; set; }
        public virtual string Amtrecvd710901_2008 { get; set; }
        public virtual string AmountrecovdMSA81183_2003 { get; set; }
        public virtual string BDincurred710401_2008 { get; set; }
        public virtual string BaddebtsincurredMSA81131_2003 { get; set; }
        public virtual string RECOVERDT_2009 { get; set; }
        public virtual string RECOVERDT_2003 { get; set; }
        public virtual string Amtrecvd710901_2009 { get; set; }
        public virtual string AmtrecvdMSA81183_2004 { get; set; }
        public virtual string RECOVERDT_2010 { get; set; }
        public virtual string BDincurredMSA81131_2004 { get; set; }
        public virtual string Amtrecvd710901_2010 { get; set; }
        public virtual string RECOVERDT_2004 { get; set; }
        public virtual string BDincurred710401_2010 { get; set; }
        public virtual string RECOVERDT_2011 { get; set; }
        public virtual string Amtrecvd710901_2011 { get; set; }
        public virtual string RECOVERDT_2005 { get; set; }
        public virtual string BDincurred710401_2011 { get; set; }
        public virtual string RECOVERDT_2012 { get; set; }
        public virtual string Amtrecvd710901_2012 { get; set; }
        public virtual string RECOVERDT_2006 { get; set; }
        public virtual string BDincurred710401_2012 { get; set; }
        public virtual string Amtrecvd710901_2007 { get; set; }
        public virtual string RECOVERDT_2013 { get; set; }
        public virtual string BDincurred710401_2007 { get; set; }
        public virtual string Amtrecvd710901_2013 { get; set; }
        public virtual string RECOVERDT_2007 { get; set; }
        public virtual string BDincurred710401_2013 { get; set; }
        public virtual string SFWITH81131 { get; set; }
        public virtual string ManualChgoffRemarks { get; set; }
        public virtual string ECSFLAG { get; set; }
        public virtual string PDCFLAG { get; set; }
        public virtual string APPBR_38 { get; set; }
        public virtual string Remarks_41 { get; set; }
        public virtual string CHGOFFREASONCODE { get; set; }

        // TempTable=>TEMP_R_WRITEOFF_PL_SCB
        public virtual string AmtrecovdMSA81183_2002 { get; set; }

        public virtual string Amtrecvd710901_2000 { get; set; }
        public virtual string BDincurred710401_2000 { get; set; }
        public virtual string RECOVERDT_2001 { get; set; }
        public virtual string Amtrecvd710901_2001 { get; set; }
        public virtual string BDincurred710401_2001 { get; set; }
        public virtual string BTDRECONP0S { get; set; }
        public virtual string APPBR_61 { get; set; }
        public virtual string Remarks_64 { get; set; }
        public virtual string CHGOFFREASONASPER402SCREEN { get; set; }
        public virtual string SELLDOWNACTS { get; set; }
        public virtual string Count { get; set; }

        // TempTable=>TEMP_R_WRITEOFF_PL_GB

        //public virtual string PRODUCT { get; set; }
        public virtual string AmttakenfromSCGB { get; set; }
        public virtual string LTCHGWAIVED_723 { get; set; }
        public virtual string AMTUTLPROV567 { get; set; }
        public virtual string AMTUTLINTTSUS797 { get; set; }
        public virtual string ANYOTHERDUES_851799 { get; set; }
        public virtual string AmountrecovdMSA_81183_2002 { get; set; }
        public virtual string BaddebtsincurredMSA_81131_2002 { get; set; }
        public virtual string AmountrecovdMSA_81183_2003 { get; set; }
        public virtual string BaddebtsincurredMSA_81131_2003 { get; set; }
        public virtual string AmountrecovdMSA_81183_2004 { get; set; }
        public virtual string BaddebtsincurredMSA_81131_2004 { get; set; }
        public virtual string Amountrecovd710961_2005 { get; set; }
        public virtual string Baddebtsincurred710461_2005 { get; set; }
        public virtual string Amountrecovd710961_2006 { get; set; }
        public virtual string Baddebtsincurred710461_2006 { get; set; }
        public virtual string Amountrecovd710961_2007 { get; set; }
        public virtual string Baddebtsincurred710461_2007 { get; set; }
        public virtual string Amountrecovd710961_2008 { get; set; }
        public virtual string Baddebtsincurred710461_2008 { get; set; }
        public virtual string Amountrecovd710961_2009 { get; set; }
        public virtual string Amountrecovd710961_2010 { get; set; }
        public virtual string Baddebtsincurred710461_2010 { get; set; }
        public virtual string Amountrecovd710961_2011 { get; set; }
        public virtual string Baddebtsincurred710461_2011 { get; set; }
        public virtual string Amountrecovd710961_2012 { get; set; }
        public virtual string Baddebtsincurred710461_2012 { get; set; }
        public virtual string Amountrecovd710961_2013 { get; set; }
        public virtual string Baddebtsincurred710461_2013 { get; set; }
        public virtual string RECDT_52 { get; set; }
        public virtual string AMOUNTRECD { get; set; }
        public virtual string BTDRECOVONPOS { get; set; }
       // public virtual string SETTLEMENT { get; set; }
        public virtual string MANUALCHGOFF_REMARKS { get; set; }
        public virtual string APPBR_63 { get; set; }
      //  public virtual string DISPUTE { get; set; }
        //public virtual string DONOTFOLLOW { get; set; }
        public virtual string REMARKS_66 { get; set; }
        public virtual string PLSELLDOWN { get; set; }

        // TempTable=>TEMP_R_WRITEOFF_PL_LORDS
        public virtual string LORDSLoan { get; set; }//LORDSLoan#
        public virtual string PRODCODE { get; set; }
        public virtual string BANKNBFC { get; set; }
        public virtual string BTDRECONPS0 { get; set; }
        public virtual string REMARKS_26 { get; set; }
        public virtual string APPBR { get; set; }
        public virtual string BRANCHNAME { get; set; }
        public virtual string Remarks { get; set; }
        public virtual string CHGOFFREASON_ASPERSCREEN402 { get; set; }

        // tempTable=>TEMP_R_WRITEOFF_AUTO_AEB

        public virtual string ANYOTHERDUES_799 { get; set; }
        public virtual string RECOVERDT { get; set; }
        public virtual string Amtrecvd710901 { get; set; }
        public virtual string BDincurred710401 { get; set; }
        public virtual string RECOVERDT_18 { get; set; }
        public virtual string Amtrecvd710901_19 { get; set; }
        public virtual string RECOVERDT_20 { get; set; }
        public virtual string Amtrecvd710901_21 { get; set; }
        public virtual string BDincurred710401_22 { get; set; }
        public virtual string RECOVERDT_23 { get; set; }
        public virtual string Amtrecvd710901_24 { get; set; }
        public virtual string BDincurred710401_25 { get; set; }
        public virtual string RECOVERDT_26 { get; set; }
        public virtual string Amtrecvd710901_27 { get; set; }
        public virtual string BDincurred710401_28 { get; set; }
        public virtual string RECOVERDT_29 { get; set; }
        public virtual string Amtrecvd710901_30 { get; set; }
        public virtual string BDincurred710401_31 { get; set; }
        public virtual string RECOVERDT_32 { get; set; }
        public virtual string Amtrecvd710901_33 { get; set; }
        public virtual string APPBR_43 { get; set; }
        public virtual string Remarks_46 { get; set; }


        //TempTable=> TEMP_R_WRITEOFF_AUTO_GB

        public virtual string ANYOTHERDUES_799851 { get; set; }//ANYOTHERDUES_&799,851
        public virtual string AmountrecovdMSA_81183 { get; set; }
        public virtual string BaddebtsincurredMSA_81131 { get; set; }
        public virtual string AmountrecovdMSA_81183_19 { get; set; }
        public virtual string BaddebtsincurredMSA_81131_20 { get; set; }
        public virtual string RECOVERDT_21 { get; set; }
        public virtual string AmountrecovdMSA_81183_22 { get; set; }
        public virtual string BaddebtsincurredMSA_81131_23 { get; set; }
        public virtual string RECOVERDT_24 { get; set; }
        public virtual string Amountrecovd710961 { get; set; }
        public virtual string Baddebtsincurred710461 { get; set; }
        public virtual string RECOVERDT_27 { get; set; }
        public virtual string Amountrecovd710961_28 { get; set; }
        public virtual string Baddebtsincurred710461_29 { get; set; }
        public virtual string RECOVERDT_30 { get; set; }
        public virtual string Amountrecovd710961_31 { get; set; }
        public virtual string Baddebtsincurred710461_32 { get; set; }
        public virtual string RECOVERDT_33 { get; set; }
        public virtual string Amountrecovd710961_34 { get; set; }
        public virtual string Baddebtsincurred710461_35 { get; set; }
        public virtual string RECOVERDT_36 { get; set; }
        public virtual string Amountrecovd710961_37 { get; set; }
        public virtual string RECOVERDT_38 { get; set; }
        public virtual string Amountrecovd710961_39 { get; set; }
        public virtual string Baddebtsincurred710461_40 { get; set; }
        public virtual string RECOVERDT_41 { get; set; }
        public virtual string Amountrecovd710961_42 { get; set; }
        public virtual string Baddebtsincurred710461_43 { get; set; }
        public virtual string RECOVERDT_44 { get; set; }
        public virtual string Amountrecovd710961_45 { get; set; }
        public virtual string Baddebtsincurred710461_46 { get; set; }
        public virtual string RECOVERMONTH { get; set; }
        public virtual string AMTRECOVERED { get; set; }
        public virtual string RECOVERDT_49 { get; set; }
        public virtual string Amountrecovd710961_50 { get; set; }
        public virtual string Baddebtsincurred710461_51 { get; set; }
        public virtual string RECOVERDT_52 { get; set; }
        public virtual string Amountrecovd710961_53 { get; set; }
        public virtual string Baddebtsincurred710461_54 { get; set; }
        public virtual string CARSALEMANUALCHGOFF_REMARKS { get; set; }
        public virtual string APPBR_62 { get; set; }
        public virtual string REMARKS_65 { get; set; }
        public virtual string CHARGEOFFREASONCODE { get; set; }

        //TempTable=> TEMP_R_WRITEOFF_AUTO_SCB

        public virtual string AmtrecovdMSA_81183 { get; set; }
        public virtual string BDincurredMSA_81131 { get; set; }
        public virtual string AmtrecvdMSA_81183 { get; set; }
        public virtual string BDincurredMSA_81131_20 { get; set; }
        public virtual string AmtrecvdMSA_81183_22 { get; set; }
        public virtual string BDincurredMSA_81131_23 { get; set; }
        public virtual string AmtrecvdMSA_81183_25 { get; set; }
        public virtual string Amtrecvd710961 { get; set; }
        public virtual string BDincurred710461 { get; set; }
        public virtual string Amtrecvd710961_31 { get; set; }
        public virtual string BDincurred710461_32 { get; set; }
        public virtual string Amtrecvd710961_34 { get; set; }
        public virtual string BDincurred710461_35 { get; set; }
        public virtual string Amtrecvd710961_37 { get; set; }
        public virtual string BDincurred710461_38 { get; set; }
        public virtual string RECOVERDT_39 { get; set; }
        public virtual string Amtrecvd710961_40 { get; set; }
        public virtual string Amtrecvd710961_42 { get; set; }
        public virtual string BDincurred710461_43 { get; set; }
        public virtual string Amtrecvd710961_45 { get; set; }
        public virtual string BDincurred710461_46 { get; set; }
        public virtual string RECOVERDT_47 { get; set; }
        public virtual string Amtrecvd710961_48 { get; set; }
        public virtual string BDincurred710461_49 { get; set; }
        public virtual string RECOVERDT_50 { get; set; }
        public virtual string Amtrecvd710961_51 { get; set; }
        public virtual string BDincurred710461_52 { get; set; }
        public virtual string RECOVERDT_53 { get; set; }
        public virtual string Amtrecvd710961_54 { get; set; }
        public virtual string BDincurred710461_55 { get; set; }
        public virtual string SettlementRemarks { get; set; }
        public virtual string CarSaleAndManualChgoffRemarks { get; set; }
        public virtual string APPBRANCH { get; set; }
        public virtual string Remarks_67 { get; set; }
        public virtual string BuyBackACTS { get; set; }

        #endregion

        #region Relationship

        public override IList<string> GetExcludeInExcelProperties()
        {
            var memberHelper = new MemberHelper<RWriteoff>();
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
            var memberHelper = new MemberHelper<RWriteoff>();
            return new List<string>
                {
                    memberHelper.GetName(x => x.AccountNo),
                    memberHelper.GetName(x => x.CustomerName ),
                    memberHelper.GetName(x => x.DisbursementDate ),
                    memberHelper.GetName(x => x.FinalInstDate ),
                    memberHelper.GetName(x => x.Cycle ),
                    memberHelper.GetName(x => x.Branch ),
                    memberHelper.GetName(x => x.ChargeOffDate ),
                    memberHelper.GetName(x => x.ProductName ),
                    memberHelper.GetName(x => x.Product ),
                    memberHelper.GetName(x => x.BounceCharge ),
                    memberHelper.GetName(x => x.LateCharge ),
                    memberHelper.GetName(x => x.PrincipalDue ),
                    memberHelper.GetName(x => x.InterestCharge ),
                    memberHelper.GetName(x => x.FeeCharge ),
                    memberHelper.GetName(x => x.TotalDue ),
                    memberHelper.GetName(x => x.Recovery ),
                    memberHelper.GetName(x => x.CurrentDue ),
                    memberHelper.GetName(x => x.IsSetteled ),
                    memberHelper.GetName(x => x.Comment ),
                    memberHelper.GetName(x => x.CustStatus ),
                    memberHelper.GetName(x => x.FileDate ),
                    memberHelper.GetName(x => x.FileRowNo  )


                };
        }

        //public override void MakeEmpty(bool forceEmpty = false)
        //{
        //    if (!NHibernateUtil.IsInitialized(RAllocs) || forceEmpty) RAllocs = null;
        //}
        public virtual Iesi.Collections.Generic.ISet<Allocations> Allocs { get; set; }

        #endregion
    }
}

// ReSharper restore CheckNamespace
// ReSharper restore ClassWithVirtualMembersNeverInherited.Global
// ReSharper restore DoNotCallOverridableMethodsInConstructor
// ReSharper restore UnusedAutoPropertyAccessor.Global
// ReSharper restore MemberCanBeProtected.Global

//public virtual decimal LoanAmount { get; set; }

//public virtual UInt32 Tenure { get; set; }

//public virtual decimal? Emi { get; set; }

//public virtual string DelqStatus { get; set; }

//public virtual DateTime DelqDate { get; set; }

//public virtual decimal DelqAmount { get; set; }

//public virtual uint? Pincode { get; set; }

//public virtual bool DoAllocate { get; set; }

//public virtual decimal InterestPct { get; set; }

