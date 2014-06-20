#region References

using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;

#endregion

namespace ColloSys.DataLayer.Mapping
{
    public class EWriteoffMap : EntityMap<EWriteoff>
    {
        public EWriteoffMap()
        {
            #region Relationship Mapping

            Set(x => x.Allocs, colmap => { }, map => map.OneToMany(x => { }));
            ManyToOne(x => x.FileScheduler, map => map.NotNullable(true));
            ManyToOne(x => x.GPincode, map => { });

            #endregion

            #region Property Mapping

            Property(x => x.FileDate, map =>
            {
                map.UniqueKey("EBBS_UQ_WRITEOFF");
                map.Index("EBBS_IX_WRITEOFF");
            });
            Property(x => x.FileRowNo);
            Property(y => y.AccountNo, map =>
            {
                map.UniqueKey("EBBS_UQ_WRITEOFF");
                map.Index("EBBS_IX_WRITEOFF");
            });
            Property(x => x.CustomerName, map => map.NotNullable(false));
            Property(p => p.ChargeOffDate);
            Property(x => x.Product);
            Property(x => x.ProductName);
            Property(x => x.Branch);
            Property(x => x.IsSetteled);
            Property(x => x.TotalDue);
            Property(x => x.PrincipalDue);
            Property(p => p.InterestCharge);
            //Property(x => x.AmountRepaid);
            Property(x => x.CurrentDue);
            Property(x => x.Comments, map => map.NotNullable(false));
            Property(p => p.BounceCharge);
            Property(p => p.FeeCharge);
            Property(p => p.LateCharge);
            Property(x => x.IsReferred);
            Property(x => x.Pincode);
            Property(x => x.NoAllocResons);
            Property(x => x.CustStatus, map => map.NotNullable(false));
            Property(x => x.AllocStatus);
            Property(x => x.Recoverydate_2002);
            Property(x => x.Amountrecovered_2002);

            Property(x => x.Recoverydate_2003);
            Property(x => x.Amountrecovered_2003);
            Property(x=>x.BadDebtsIncurred_2003);

            Property(x => x.Recoverydate_2004);
            Property(x => x.Amountrecovered_2004);
            Property(x => x.BadDebtsIncurred_2004);

            Property(x => x.Recoverydate_2005);
            Property(x => x.Amountrecovered_2005);
            Property(x => x.BadDebtsIncurred_2005);

            Property(x => x.Recoverydate_2006);
            Property(x => x.Amountrecovered_2006);
            Property(x => x.BadDebtsIncurred_2006);

            Property(x => x.Recoverydate_2007);
            Property(x => x.Amountrecovered_2007);
            Property(x => x.BadDebtsIncurred_2007);

            Property(x => x.Recoverydate_2008);
            Property(x => x.Amountrecovered_2008);
            Property(x => x.BadDebtsIncurred_2008);

            Property(x => x.Recoverydate_2009);
            Property(x => x.Amountrecovered_2009);

            Property(x => x.Recoverydate_2010);
            Property(x => x.Amountrecovered_2010);
            Property(x => x.BadDebtsIncurred_2010);

            Property(x => x.Recoverydate_2011);
            Property(x => x.Amountrecovered_2011);
            Property(x => x.BadDebtsIncurred_2011);

            Property(x => x.Recoverydate_2012);
            Property(x => x.Amountrecovered_2012);
            Property(x => x.BadDebtsIncurred_2012);

            Property(x => x.Recoverydate_2013);
            Property(x => x.Amountrecovered_2013);
            Property(x => x.BadDebtsIncurred_2013);

            Property(x => x.SettlementY);
            Property(x => x.DISPUTE);
            Property(x => x.DONOTFOLLOW);
            Property(x => x.SNo);
            Property(x => x.RemarksNamechange);
            Property(x => x.MANUALXHOLDINGCHGOFFREMARKS);
            Property(x => x.SELLDOWNACTS);
            #endregion

        }
    }
}