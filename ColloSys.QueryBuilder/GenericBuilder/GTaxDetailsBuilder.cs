using System.Collections.Generic;
using ColloSys.DataLayer.Generic;
using ColloSys.DataLayer.SessionMgr;
using ColloSys.QueryBuilder.BaseTypes;
using ColloSys.QueryBuilder.TransAttributes;

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
