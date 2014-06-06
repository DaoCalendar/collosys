using ColloSys.DataLayer.BaseEntity;

namespace ColloSys.DataLayer.Generic
{
    class GKeyValueMap : EntityMap<GKeyValue>
    {
        public GKeyValueMap()
        {
            Property(x => x.Area);
            Property(x => x.ParamName);
            Property(x => x.Value);
            Property(x => x.ValueType);
            Property(x => x.ApprovedBy, map => map.NotNullable(false));
            Property(x => x.Status);
            Property(x => x.ApprovedOn);
        }
    }
}
