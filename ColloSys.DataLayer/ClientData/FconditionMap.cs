using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColloSys.DataLayer.BaseEntity;
using NHibernate.Criterion;

namespace ColloSys.DataLayer.ClientData
{
   public class FconditionMap:EntityMap<Fcondition>
    {
       public FconditionMap()
       {
           Property(x => x.ColumnName);
           Property(x => x.Operator);
           Property(x => x.RelationType, map => map.NotNullable(false));
           Property(x => x.Value);
           ManyToOne(x => x.FilterCondition, map => map.NotNullable(true));
       }

    }
}
