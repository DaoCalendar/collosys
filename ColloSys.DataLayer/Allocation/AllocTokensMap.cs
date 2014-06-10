using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColloSys.DataLayer.BaseEntity;

namespace ColloSys.DataLayer.Allocation
{
  public class AllocTokensMap:EntityMap<AllocTokens>
  {
      public AllocTokensMap()
        {
            Property(x => x.Type);
            Property(x => x.Text);
            Property(x => x.Value);
            Property(x => x.DataType);
            Property(x=>x.GroupType);
            Property(x => x.GroupId);
            Property(x => x.Priority);

            ManyToOne(x => x.AllocSubpolicy, map => map.NotNullable(false));
        }
    }
}
