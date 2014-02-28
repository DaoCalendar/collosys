#region references

using System.Collections.Generic;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;

#endregion

namespace ColloSys.AllocationService.PincodeEntry
{
    public static class DbLayer
    {
        public static List<T> GetDataLinerWriteOffData<T>(ScbEnums.Products products)
            where T : Entity, IDelinquentCustomer
        {
            using (var session = SessionManager.GetNewSession())
            {
                using (var trans = session.BeginTransaction())
                {
                    var data = session.QueryOver<T>()
                                      //.Where(x => x.AllocStatus == ColloSysEnums.AllocStatus.None)
                                      .Where(x => x.GPincode == null)
                                      .And(x => x.Product == products)
                                      .And(x=>x.Pincode>0)
                                      .List();
                    trans.Rollback();
                    return (List<T>)data;
                }
            }

        }

        public static void SaveList<T>(IEnumerable<T> data)
            where T : Entity, IDelinquentCustomer
        {
            using (var session = SessionManager.GetNewSession())
            {
                using (var trans = session.BeginTransaction())
                {
                    foreach (var obj in data)
                    {
                        session.SaveOrUpdate(obj);
                    }
                    trans.Commit();
                }
            }
        }
    }
}




//public static List<T> GetDataWriteoff<T>(ScbEnums.Products products)
//    where T:Entity, IHardDelq
//{
//    var data = SessionManager.GetNewSession()
//                             .QueryOver<T>()
//                             .Where(x => x.ConsideredForAllocation == false)
//                             .And(x => x.Product == products)
//                             .List();
//    return (List<T>) data;
//}

