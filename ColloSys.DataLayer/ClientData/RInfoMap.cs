using ColloSys.DataLayer.Enumerations;

namespace ColloSys.DataLayer.ClientData
{

    namespace ColloSys.DataLayer.ClientData
    {
        public class RInfoMap : InfoMap<RInfo>
        {
            public RInfoMap() : base(ScbEnums.ScbSystems.RLS)
            {
                Set(x => x.RAllocs, colmap => { }, map => map.OneToMany(x => { }));
            }
        }
    }
}
