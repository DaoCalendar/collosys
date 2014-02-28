#region References

using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;
using NHibernate;

#endregion

namespace ColloSys.DataLayer.Mapping
{
    public class BillAdhocMap : EntityMap<BillAdhoc>
    {
        public BillAdhocMap()
        {
            Table("BILL_ADHOC");

            #region Property
            Property(x => x.IsRecurring);
            Property(x => x.TotalAmount);
            Property(x => x.RemainingAmount);
            Property(x => x.StartMonth);
            Property(x => x.EndMonth);
            Property(x => x.Tenure);
            Property(x => x.IsCredit);
            Property(x => x.IsPretax);
            Property(x => x.ReasonCode, map => map.NotNullable(false));

            Property(x => x.Products);
            #endregion

            #region IApprove
            Property(p => p.Status);
            Property(p => p.Description, map => map.NotNullable(false));
            Property(p => p.ApprovedBy, map => map.NotNullable(false));
            Property(p => p.ApprovedOn);
            Property(p => p.OrigEntityId);
            Property(p => p.RowStatus);
            #endregion

            #region ManyToOne
            ManyToOne(x => x.Stakeholder, map => map.NotNullable(true));
            #endregion

            #region Bag
            Set(x => x.BillDetails, colmap => { }, map => map.OneToMany(x => { }));
            #endregion
        }
    }
}
