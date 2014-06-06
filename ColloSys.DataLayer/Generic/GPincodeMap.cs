using ColloSys.DataLayer.BaseEntity;

namespace ColloSys.DataLayer.Generic
{
    public class GPincodeMap : EntityMap<GPincode>
    {
        public GPincodeMap()
        {
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

            Bag(x => x.CLiners, map => { }, colmap => colmap.OneToMany());
            Bag(x => x.CWriteoffs, map => { }, colmap => colmap.OneToMany());
            Bag(x => x.RLiners, map => { }, colmap => colmap.OneToMany());
            Bag(x => x.RWriteoffs, map => { }, colmap => colmap.OneToMany());
            Bag(x => x.ELiners, map => { }, colmap => colmap.OneToMany());
            Bag(x => x.EWriteoffs, map => { }, colmap => colmap.OneToMany());
        }
    }
}