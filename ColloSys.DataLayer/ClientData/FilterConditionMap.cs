using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColloSys.DataLayer.BaseEntity;

namespace ColloSys.DataLayer.ClientData
{
  public  class FilterConditionMap:EntityMap<FilterCondition>
    {
      public FilterConditionMap()
      {
          ManyToOne(x=>x.FileDetail,map=>map.NotNullable(true));
          Property(x=>x.ColumnName);
          Property(x=>x.Operator);
          Property(x=>x.RelationType, map=>map.NotNullable(false));
          Property(x=>x.Value);
      }

    }
}
