using ColloSys.DataLayer.BaseEntity;

namespace ColloSys.DataLayer.Generic
{
    public class GTaxesListMap : EntityMap<GTaxesList>
    {
        public GTaxesListMap()
        {
            Property(x => x.TaxName);
            Property(x => x.TaxType);
            Property(x => x.ApplicableTo);
            Property(x => x.IndustryZone);
            Property(x => x.ApplyOn);
            Property(x => x.TotSource);
            Property(x => x.Description);

            Bag(x => x.GTaxDetails, colmap => { }, map => map.OneToMany(x => { }));
        }
    }
}