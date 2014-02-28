#region references

using System.Collections.Generic;
using System.Linq;
using ColloSys.AllocationService.Generic;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.SharedDomain;

#endregion


namespace ColloSys.AllocationService.IgnoreCases
{
    internal static class DataAccess
    {
        public static IList<TLinerWriteOff> GetLinerWriteOffData<TLinerWriteOff>(ScbEnums.Products products)
            where TLinerWriteOff : Entity, IFileUploadable, IDelinquentCustomer
        {
            using (var session = SessionManager.GetNewSession())
            {
                using (var trans = session.BeginTransaction())
                {
                    var data = session.QueryOver<TLinerWriteOff>()
                                      .Where(x => x.FileDate == Util.GetTodayDate())
                                      .And(x => x.Product == products)
                                      .And(x => x.AllocStatus == ColloSysEnums.AllocStatus.None)
                                      .List();
                    trans.Rollback();
                    return data;
                }
            }
        }

        public static IList<TInfo> GetInfoData<TInfo>(ScbEnums.Products products)
           where TInfo : Entity, IDelinquentCustomer
        {
            using (var session = SessionManager.GetNewSession())
            {
                using (var trans = session.BeginTransaction())
                {
                    var data = session.QueryOver<TInfo>()
                                      .Where(x => x.Product == products)
                                      .And(x=>x.AllocEndDate!=null)
                                      .And(x=>x.AllocEndDate<Util.GetTodayDate())
                                      .List();
                    trans.Rollback();
                    return data;
                }
            }
        }

        public static TLinerWriteOff CheckInInfo<TLinerWriteOff>(TLinerWriteOff linerWriteOff)
            //where TInfo : SharedInfo
            where TLinerWriteOff : Entity, IDelinquentCustomer
        {
            using (var session = SessionManager.GetNewSession())
            {
                using (var trans = session.BeginTransaction())
                {
                    //var data = session.QueryOver<TInfo>()
                    //                  .Where(x => x.AccountNo == linerWriteOff.AccountNo)
                    //                  .And(x => x.AllocStartDate < Util.GetTodayDate())
                    //                  .And(x => x.AllocEndDate > Util.GetTodayDate())
                    //                  .SingleOrDefault();
                    //if (data == null)
                    //{
                    //    trans.Rollback();
                    //    return null;
                    //}
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
