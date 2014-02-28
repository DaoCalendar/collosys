using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;

namespace ColloSys.DataLayer.Generic
{
    class GKeyValueMap : EntityMap<GKeyValue>
    {
        public GKeyValueMap()
        {
            Table("G_KeyValue");

            #region Property

            Property(x => x.Key);

            Property(x => x.Value);

            Property(x => x.ValueType);

            Property(x => x.Area);

            #endregion
        }

    }
}
