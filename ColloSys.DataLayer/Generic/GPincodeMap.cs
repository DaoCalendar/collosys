#region references

using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;

#endregion

namespace ColloSys.DataLayer.Mapping
{
    public class GPincodeMap : EntityMap<GPincode>
    {
        public GPincodeMap()
        {
            Table("G_PINCODES");

            #region Property

            Property(x => x.Country);
            NaturalId(x => x.Property(y => y.Pincode, map => map.Index("IX_GPINCODE")));
            Property(x => x.Area);
            Property(x => x.City);
            Property(x => x.District);
            Property(x => x.Cluster);
            Property(x => x.State);
            Property(x => x.Region);
            Property(x => x.IsInUse);
            Property(x => x.CityCategory);

            #endregion

           // ManyToOne(x => x.StakeAddress, map => map.NotNullable(false));
        }
    }
}