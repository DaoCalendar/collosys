using System.Collections.Generic;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.SessionMgr;

namespace ColloSys.Shared.DbLayer
{
    public static class DataAccess
    {
        public static IEnumerable<ProductConfig> GetProductInfo()
        {
            using (var session = SessionManager.GetNewSession())
            {
                using (var trans = session.BeginTransaction())
                {
                    var data = session.QueryOver<ProductConfig>().Cacheable().List();
                    trans.Rollback();
                    return data;
                }
            }
        }
    }
}
