using ColloSys.DataLayer.Enumerations;

namespace ColloSys.DataLayer.ClientData
{
    public class EInfoMap : SharedInfoMap<EInfo>
    {
        public EInfoMap() : base(ScbEnums.ScbSystems.EBBS)
        {
            Set(x => x.EAllocs, colmap => { }, map => map.OneToMany(x => { }));
        }
    }
}