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
          Property(x => x.AliasConditionName,map=>map.NotNullable(true));
          Bag(x => x.Fconditions, colmap => { }, map => map.OneToMany(x => { }));
      }

    }
}
