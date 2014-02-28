using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;

namespace ColloSys.AllocationService.AllocationLastCode
{
    public class DbLayer
    {
        public static List<T> GetUnAllocatedCases<T>(ScbEnums.Products products)
            where T : Entity, IDelinquentCustomer
        {
            using (var session = SessionManager.GetNewSession())
            {
                using (var trans=session.BeginTransaction())
                {
                    var data = session.QueryOver<T>()
                                      .Where(x => x.Product == products)
                                      .And(x=>x.AllocEndDate==null)
                                      .List();
                    trans.Rollback();
                    return (List<T>) data;
                }
            }
        }

        public static void SaveList<T>(IList<T> entityList)
            where T : Entity
        {
            using (var session = SessionManager.GetNewSession())
            {
                using (var trans = session.BeginTransaction())
                {
                    foreach (var entity in entityList)
                    {
                        session.SaveOrUpdate(entity);
                    }
                    trans.Commit();
                }
            }
        }

    }
}
