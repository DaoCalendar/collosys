using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.SessionMgr;
using ColloSys.QueryBuilder.BaseTypes;
using ColloSys.QueryBuilder.TransAttributes;
using NHibernate.Linq;

namespace ColloSys.QueryBuilder.GenericBuilder
{
    public class GTaxDetailsBuilder:Repository<GTaxDetail>
    {
        [Transaction]
        public IEnumerable<GTaxDetail> GetAllWithRef()
        {
            return SessionManager.GetCurrentSession().QueryOver<GTaxDetail>()
                                 .Fetch(x => x.GTaxesList).Eager.List();
        }
    }
}
