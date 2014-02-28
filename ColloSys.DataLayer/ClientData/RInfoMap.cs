using ColloSys.DataLayer.Enumerations;

namespace ColloSys.DataLayer.ClientData
{

    namespace ColloSys.DataLayer.ClientData
    {
        public class RInfoMap : SharedInfoMap<RInfo>
        {
            public RInfoMap() : base(ScbEnums.ScbSystems.RLS)
            {
                Set(x => x.RAllocs, colmap => { }, map => map.OneToMany(x => { }));
            }
        }
    }
}
