using System.Collections.Generic;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;

namespace ColloSys.AllocationService.CacsToLineWriteoff
{
    public class CacsData
    {
        public CacsData()
        {
            LinerList=new List<Entity>();
            WriteoffList=new List<Entity>();
            CacsList=new List<Entity>();

        }
        public IList<Entity> LinerList { get; set; }
        public IList<Entity> WriteoffList { get; set; }
        public IList<Entity> CacsList { get; set; }
        public FileScheduler FileSchedular { get; set; }
    }
}
