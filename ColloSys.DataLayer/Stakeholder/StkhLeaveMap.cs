using ColloSys.DataLayer.BaseEntity;

namespace ColloSys.DataLayer.Stakeholder
{
    public class StkhLeaveMap : EntityMap<StkhLeave>
    {
        public StkhLeaveMap()
        {
            Property(x => x.FromDate);
            Property(x => x.ToDate);

            ManyToOne(x => x.Stakeholder, map => map.NotNullable(true));
            ManyToOne(x => x.DelegatedTo, map => { map.NotNullable(true); map.Column("DelegatedTo"); });
        }
    }
}