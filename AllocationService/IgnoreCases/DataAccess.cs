#region references

using System.Collections.Generic;
using System.Linq;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.SharedDomain;
using ColloSys.QueryBuilder.ClientDataBuilder;

#endregion


namespace ColloSys.AllocationService.IgnoreCases
{
    internal static class DataAccess
    {
        private static readonly InfoBuilder InfoBuilder=new InfoBuilder();

        public static IEnumerable<Info> GetInfoData(ScbEnums.Products products)
        {
            return InfoBuilder.IgnoreAllocated(products);
        }

        //TODO: check here for query layer
        public static TLinerWriteOff CheckInInfo<TLinerWriteOff>(TLinerWriteOff linerWriteOff)
            //where TInfo : SharedInfo
            where TLinerWriteOff : Entity, IDelinquentCustomer
        {
            using (var session = SessionManager.GetNewSession())
            {
                using (var trans = session.BeginTransaction())
                {
                    
                    var linerWriteoffold = session.QueryOver<TLinerWriteOff>()
                                          .Where(x => x.AccountNo == linerWriteOff.AccountNo)
                                          .And(x=>x.AllocStatus!=ColloSysEnums.AllocStatus.None)
                                          //.Fetch(x => x.Allocs).Eager
                                          //TODO: harish : mahendra - i have removed allocs, we need fetch alloc on runtime
                                          .OrderBy(x => x.CreatedOn).Desc
                                          .List()
                                          .FirstOrDefault();
                    trans.Rollback();
                    return linerWriteoffold;
                    //var data = session.QueryOver<TLinerWriteOff>()
                    //                  .Fetch(x => x.Allocs).Eager
                    //                  .Where(x => x.AccountNo == linerWriteOff.AccountNo)
                    //                  .And(x => x.IsAllocated)
                    //                  .And(x => x.Allocs.StartDate < Util.GetTodayDate())
                    //                  .And(x => x.Allocs.EndDate > Util.GetTodayDate())
                    //                  .OrderBy(x => x.FileDate).Desc
                    //                  .Take(1)
                    //                  .SingleOrDefault();
                    //trans.Rollback();
                    //return data;
                }
            }
        }
    }
}
