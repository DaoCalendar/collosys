using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColloSys.DataLayer.BaseEntity;

namespace ColloSys.DataLayer.Generic
{
    public class GProcessMgrMap:EntityMap<GProcessMgr>
    {
        public GProcessMgrMap()
        {
            Table("G_PROCESSMGR");

            Property(x=>x.FileDate, map=>map.NotNullable(true));

            Property(x=>x.Product, map=>map.NotNullable(true));

            Property(x=>x.Category, map=>map.NotNullable(true));

            Property(x=>x.Status,map=>map.NotNullable(true));

            ManyToOne(x=>x.FileScheduler, map=> map.NotNullable(true));
        }
    }
}
