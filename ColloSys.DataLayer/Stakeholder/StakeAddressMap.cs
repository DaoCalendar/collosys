using ColloSys.DataLayer.BaseEntity;

namespace ColloSys.DataLayer.Stakeholder
{
    public class StakeAddressMap : EntityMap<StakeAddress>
    {
        public StakeAddressMap()
        {
            Property(x => x.Line1);
            Property(x => x.Line2, map => map.NotNullable(false));
            Property(x => x.Line3, map => map.NotNullable(false));
            Property(x => x.LandlineNo, map => map.NotNullable(false));
            Property(x => x.Pincode);
            Property(x => x.Country);
            Property(x => x.StateCity, map => map.NotNullable(false));
            ManyToOne(x => x.Stakeholder, map => map.NotNullable(true));
        }
    }
}