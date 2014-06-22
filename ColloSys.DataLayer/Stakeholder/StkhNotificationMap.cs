using ColloSys.DataLayer.BaseEntity;

namespace ColloSys.DataLayer.Stakeholder
{
    public class StkhNotificationMap : EntityMap<StkhNotification>
    {
        public StkhNotificationMap()
        {
            Property(x => x.EntityId);
            Property(x => x.NoteType);
            Property(x => x.NoteStatus);
            Property(x => x.Description);
            Property(x => x.ParamsJson, map => map.NotNullable(false));
            Property(x => x.IsResponse);
            Property(x => x.ResponseType, map => map.NotNullable(false));
            Property(x => x.ResponseBy, map => map.NotNullable(false));
            Property(x => x.RequestBy);
            Property(x => x.RequestDateTime);
            Property(x => x.ResponseDateTime);
            ManyToOne(x => x.ForStakeholder, map => { map.NotNullable(true); map.Column("ForStakeholder"); });
            ManyToOne(x => x.ByStakeholder, map => { map.NotNullable(true); map.Column("ByStakeholder"); });
        }
    }
}
