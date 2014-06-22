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
            Property( x=> x.IsResponse);
            ManyToOne(x => x.ForStakeholder, map => map.NotNullable(true));
        }
    }
}
