using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColloSys.DataLayer.BaseEntity;

namespace ColloSys.DataLayer.Generic
{
    public class PerformanceManagementMap : EntityMap<PerformanceManagement>
    {
        public PerformanceManagementMap()
        {
            Property(x => x.Products);
            Bag(x => x.PerformanceParamses, colmap => { }, map => map.OneToMany(x => { }));
        }
    }
}
