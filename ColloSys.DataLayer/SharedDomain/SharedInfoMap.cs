using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.ClientData;

namespace ColloSys.DataLayer.SharedDomain
{
        public abstract class SharedInfoMap : EntityMap<SharedInfo> 
        {
            protected SharedInfoMap(string className)
            {
                

            }
        }
}
