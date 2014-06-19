#region references

using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;

#endregion

namespace ColloSys.DataLayer.Mapping
{
    public class RWriteoffMap : EntityMap<RWriteoff>
    {
        public RWriteoffMap()
        {
            #region Relationship Mapping

            Set(x => x.Allocs, colmap => { }, map => map.OneToMany(x => { }));
            ManyToOne(x => x.FileScheduler, map => map.NotNullable(true));
            ManyToOne(x => x.GPincode, map => { });

            #endregion

            #region Property Mapping

            Property(x => x.FileDate, map =>
            {
                map.UniqueKey("RLS_UQ_WRITEOFF");
                map.Index("RLS_IX_WRITEOFF");
            });

            Property(x => x.FileRowNo);
            Property(y => y.AccountNo, map =>
            {
                map.UniqueKey("RLS_UQ_WRITEOFF");
                map.Index("RLS_IX_WRITEOFF");
            });

            Property(x => x.CustomerName, map => map.NotNullable(false));
            Property(x => x.Product);
            Property(x => x.ProductName, map => map.NotNullable(false));
            Property(x => x.Branch, map => map.NotNullable(false));
            Property(x => x.ChargeOffDate);
            Property(x => x.TotalDue);
            Property(x => x.PrincipalDue);
            Property(x => x.Recovery);
            Property(x => x.CurrentDue);
            Property(x => x.IsSetteled);
            Property(x => x.Comment, map => map.NotNullable(false));
            Property(p => p.BounceCharge);
            Property(p => p.FeeCharge);
            Property(p => p.InterestCharge);
            Property(p => p.LateCharge);
            Property(x => x.Cycle);
            Property(x => x.DisbursementDate);
            Property(x => x.FinalInstDate);
            Property(x => x.IsReferred);
            Property(x => x.Pincode);
            Property(x => x.NoAllocResons);
            Property(x => x.CustStatus, map => map.NotNullable(false));
            Property(x => x.AllocStatus);

            #endregion

            Property(x => x.DiscProv);
            Property(x => x.AmtrecvdDATE_2005);
            Property(x => x.AmtrecvdMSA81183_2005);
            Property(x => x.BDincurredMSA81131_2005);
            Property(x => x.AmtrecvdDATE_2006);
            Property(x => x.AmtrecvdMSA81183_2006);
            Property(x => x.BDincurredMSA81131_2006);
            Property(x => x.AmtrecvdDATE_2007);
            Property(x => x.AmtrecvdMSA81183_2007);
            Property(x => x.BDincurredMSA81131_2007);
            Property(x => x.AmtrecvdDATE_2008);
            Property(x => x.AmtrecvdMSA81183_2008);
            Property(x => x.BDincurredMSA81131_2008);
            Property(x => x.AmtrecvdDATE_2009);
            Property(x => x.AmtrecvdMSA81183_2009);
            Property(x => x.AmtrecvdDATE_2010);
            Property(x => x.AmtrecvdMSA81183_2010);
            Property(x => x.BDincurredMSA81131_2010);
            Property(x => x.AmtrecvdDATE_2011);
            Property(x => x.AmtrecvdMSA81183_2011);
            Property(x => x.BDincurredMSA81131_2011);
            Property(x => x.AmtrecvdDATE_2012);
            Property(x => x.AmtrecvdMSA81183_2012);
            Property(x => x.BDincurredMSA81131_2012);
            Property(x => x.AmtrecvdDATE_2013);
            Property(x => x.AmtrecvdMSA81183_2013);
            Property(x => x.BDincurredMSA81131_2013);
            Property(x => x.SFWITHDEBITPOSTCOFF);
            Property(x => x.Settlement);
            Property(x => x.REMARKS_43);
            Property(x => x.ECS);
            Property(x => x.PDC);
            Property(x => x.APPBRANCHCODE);
            Property(x => x.Dispute);
            Property(x => x.DonotFollow);
            Property(x => x.COReason);
            Property(x => x.Remarks_48);
            Property(x => x.RECOVERYASPERAMEX);
            Property(x => x.BDincurredMSA81131_2002);
            Property(x => x.RECOVERDT_2008);
            Property(x => x.RECOVERDT_2002);
            Property(x => x.Amtrecvd710901_2008);
            Property(x => x.AmountrecovdMSA81183_2003);
            Property(x => x.BDincurred710401_2008);
            Property(x => x.BaddebtsincurredMSA81131_2003);
            Property(x => x.RECOVERDT_2009);
            Property(x => x.RECOVERDT_2003);
            Property(x => x.Amtrecvd710901_2009);
            Property(x => x.AmtrecvdMSA81183_2004);
            Property(x => x.RECOVERDT_2010);
            Property(x => x.BDincurredMSA81131_2004);
            Property(x => x.Amtrecvd710901_2010);
            Property(x => x.RECOVERDT_2004);
            Property(x => x.BDincurred710401_2010);
            Property(x => x.RECOVERDT_2011);
            Property(x => x.Amtrecvd710901_2011);
            Property(x => x.RECOVERDT_2005);
            Property(x => x.BDincurred710401_2011);
            Property(x => x.RECOVERDT_2012);
            Property(x => x.Amtrecvd710901_2012);
            Property(x => x.RECOVERDT_2006);
            Property(x => x.BDincurred710401_2012);
            Property(x => x.Amtrecvd710901_2007);
            Property(x => x.RECOVERDT_2013);
            Property(x => x.BDincurred710401_2007);
            Property(x => x.Amtrecvd710901_2013);
            Property(x => x.RECOVERDT_2007);
            Property(x => x.BDincurred710401_2013);
            Property(x => x.SFWITH81131);
            Property(x => x.ManualChgoffRemarks);
            Property(x => x.ECSFLAG);
            Property(x => x.PDCFLAG);
            Property(x => x.APPBR_38);
            Property(x => x.Remarks_41);
            Property(x => x.CHGOFFREASONCODE);
            Property(x => x.AmtrecovdMSA81183_2002);
            Property(x => x.Amtrecvd710901_2000);
            Property(x => x.BDincurred710401_2000);
            Property(x => x.RECOVERDT_2001);
            Property(x => x.Amtrecvd710901_2001);
            Property(x => x.BDincurred710401_2001);
            Property(x => x.BTDRECONP0S);
            Property(x => x.APPBR_61);
            Property(x => x.Remarks_64);
            Property(x => x.CHGOFFREASONASPER402SCREEN);
            Property(x => x.SELLDOWNACTS);
            Property(x => x.Count);
            Property(x => x.PRODUCT);
            Property(x => x.AmttakenfromSCGB);
            Property(x => x.LTCHGWAIVED_723);
            Property(x => x.AMTUTLPROV567);
            Property(x => x.AMTUTLINTTSUS797);
            Property(x => x.ANYOTHERDUES_851799);
            Property(x => x.AmountrecovdMSA_81183_2002);
            Property(x => x.BaddebtsincurredMSA_81131_2002);
            Property(x => x.AmountrecovdMSA_81183_2003);
            Property(x => x.BaddebtsincurredMSA_81131_2003);
            Property(x => x.AmountrecovdMSA_81183_2004);
            Property(x => x.BaddebtsincurredMSA_81131_2004);
            Property(x => x.Amountrecovd710961_2005);
            Property(x => x.Baddebtsincurred710461_2005);
            Property(x => x.Amountrecovd710961_2006);
            Property(x => x.Baddebtsincurred710461_2006);
            Property(x => x.Amountrecovd710961_2007);
            Property(x => x.Baddebtsincurred710461_2007);
            Property(x => x.Amountrecovd710961_2008);
            Property(x => x.Baddebtsincurred710461_2008);
            Property(x => x.Amountrecovd710961_2009);
            Property(x => x.Amountrecovd710961_2010);
            Property(x => x.Baddebtsincurred710461_2010);
            Property(x => x.Amountrecovd710961_2011);
            Property(x => x.Baddebtsincurred710461_2011);
            Property(x => x.Amountrecovd710961_2012);
            Property(x => x.Baddebtsincurred710461_2012);
            Property(x => x.Amountrecovd710961_2013);
            Property(x => x.Baddebtsincurred710461_2013);
            Property(x => x.RECDT_52);
            Property(x => x.AMOUNTRECD);
            Property(x => x.BTDRECOVONPOS);
            Property(x => x.SETTLEMENT);
            Property(x => x.MANUALCHGOFF_REMARKS);
            Property(x => x.APPBR_63);
            Property(x => x.DISPUTE);
            Property(x => x.DONOTFOLLOW);
            Property(x => x.REMARKS_66);
            Property(x => x.PLSELLDOWN);
            Property(x => x.LORDSLoan);
            Property(x => x.PRODCODE);
            Property(x => x.BANKNBFC);
            Property(x => x.BTDRECONPS0);
            Property(x => x.REMARKS_26);
            Property(x => x.APPBR);
            Property(x => x.BRANCHNAME);
            Property(x => x.Remarks);
            Property(x => x.CHGOFFREASON_ASPERSCREEN402);
            Property(x => x.ANYOTHERDUES_799);
            Property(x => x.RECOVERDT);
            Property(x => x.Amtrecvd710901);
            Property(x => x.BDincurred710401);
            Property(x => x.RECOVERDT_18);
            Property(x => x.Amtrecvd710901_19);
            Property(x => x.RECOVERDT_20);
            Property(x => x.Amtrecvd710901_21);
            Property(x => x.BDincurred710401_22);
            Property(x => x.RECOVERDT_23);
            Property(x => x.Amtrecvd710901_24);
            Property(x => x.BDincurred710401_25);
            Property(x => x.RECOVERDT_26);
            Property(x => x.Amtrecvd710901_27);
            Property(x => x.BDincurred710401_28);
            Property(x => x.RECOVERDT_29);
            Property(x => x.Amtrecvd710901_30);
            Property(x => x.BDincurred710401_31);
            Property(x => x.RECOVERDT_32);
            Property(x => x.Amtrecvd710901_33);
            Property(x => x.APPBR_43);
            Property(x => x.Remarks_46);
            Property(x => x.ANYOTHERDUES_799851);
            Property(x => x.AmountrecovdMSA_81183);
            Property(x => x.BaddebtsincurredMSA_81131);
            Property(x => x.AmountrecovdMSA_81183_19);
            Property(x => x.BaddebtsincurredMSA_81131_20);
            Property(x => x.RECOVERDT_21);
            Property(x => x.AmountrecovdMSA_81183_22);
            Property(x => x.BaddebtsincurredMSA_81131_23);
            Property(x => x.RECOVERDT_24);
            Property(x => x.Amountrecovd710961);
            Property(x => x.Baddebtsincurred710461);
            Property(x => x.RECOVERDT_27);
            Property(x => x.Amountrecovd710961_28);
            Property(x => x.Baddebtsincurred710461_29);
            Property(x => x.RECOVERDT_30);
            Property(x => x.Amountrecovd710961_31);
            Property(x => x.Baddebtsincurred710461_32);
            Property(x => x.RECOVERDT_33);
            Property(x => x.Amountrecovd710961_34);
            Property(x => x.Baddebtsincurred710461_35);
            Property(x => x.RECOVERDT_36);
            Property(x => x.Amountrecovd710961_37);
            Property(x => x.RECOVERDT_38);
            Property(x => x.Amountrecovd710961_39);
            Property(x => x.Baddebtsincurred710461_40);
            Property(x => x.RECOVERDT_41);
            Property(x => x.Amountrecovd710961_42);
            Property(x => x.Baddebtsincurred710461_43);
            Property(x => x.RECOVERDT_44);
            Property(x => x.Amountrecovd710961_45);
            Property(x => x.Baddebtsincurred710461_46);
            Property(x => x.RECOVERMONTH);
            Property(x => x.AMTRECOVERED);
            Property(x => x.RECOVERDT_49);
            Property(x => x.Amountrecovd710961_50);
            Property(x => x.Baddebtsincurred710461_51);
            Property(x => x.RECOVERDT_52);
            Property(x => x.Amountrecovd710961_53);
            Property(x => x.Baddebtsincurred710461_54);
            Property(x => x.CARSALEMANUALCHGOFF_REMARKS);
            Property(x => x.APPBR_62);
            Property(x => x.REMARKS_65);
            Property(x => x.CHARGEOFFREASONCODE);
            Property(x => x.AmtrecovdMSA_81183);
            Property(x => x.BDincurredMSA_81131);
            Property(x => x.AmtrecvdMSA_81183);
            Property(x => x.BDincurredMSA_81131_20);
            Property(x => x.AmtrecvdMSA_81183_22);
            Property(x => x.BDincurredMSA_81131_23);
            Property(x => x.AmtrecvdMSA_81183_25);
            Property(x => x.Amtrecvd710961);
            Property(x => x.BDincurred710461);
            Property(x => x.Amtrecvd710961_31);
            Property(x => x.BDincurred710461_32);
            Property(x => x.Amtrecvd710961_34);
            Property(x => x.BDincurred710461_35);
            Property(x => x.Amtrecvd710961_37);
            Property(x => x.BDincurred710461_38);
            Property(x => x.RECOVERDT_39);
            Property(x => x.Amtrecvd710961_40);
            Property(x => x.Amtrecvd710961_42);
            Property(x => x.BDincurred710461_43);
            Property(x => x.Amtrecvd710961_45);
            Property(x => x.BDincurred710461_46);
            Property(x => x.RECOVERDT_47);
            Property(x => x.Amtrecvd710961_48);
            Property(x => x.BDincurred710461_49);
            Property(x => x.RECOVERDT_50);
            Property(x => x.Amtrecvd710961_51);
            Property(x => x.BDincurred710461_52);
            Property(x => x.RECOVERDT_53);
            Property(x => x.Amtrecvd710961_54);
            Property(x => x.BDincurred710461_55);
            Property(x => x.SettlementRemarks);
            Property(x => x.CarSaleAndManualChgoffRemarks);
            Property(x => x.APPBRANCH);
            Property(x => x.Remarks_67);
            Property(x => x.BuyBackACTS);
        }
    }
}



