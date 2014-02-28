#region References

using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;

#endregion

namespace ColloSys.DataLayer.Mapping
{
    public class BillStatusMap : EntityMap<BillStatus>
    {
        public BillStatusMap()
        {
            Table("BILL_STATUS");

            #region Property
            Property(x => x.Products);
            Property(x => x.BillMonth);
            Property(x => x.BillCycle);
            Property(x => x.Status);

            #endregion
        }
    }
}

