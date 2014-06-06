using ColloSys.DataLayer.BaseEntity;

namespace ColloSys.DataLayer.Generic
{
    public class GTaxDetailMap : EntityMap<GTaxDetail>
    {
        public GTaxDetailMap()
        {
            Property(x => x.ApplicableTo, map => map.NotNullable(true));
            Property(x => x.IndustryZone);
            Property(x => x.Country, map => map.NotNullable(true));
            Property(x => x.State, map => map.NotNullable(true));
            Property(x => x.District, map => map.NotNullable(true));
            Property(x => x.Priority, map => map.NotNullable(true));
            Property(x => x.Percentage);
            Property(x => x.TaxId, map => map.NotNullable(true));
            Property(p => p.StartDate, map => map.NotNullable(true));
            Property(p => p.EndDate);
            ManyToOne(x => x.GTaxesList, map => map.NotNullable(true));
        }
    }
}