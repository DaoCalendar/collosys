using System.Collections.Generic;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Domain;
using System.Linq;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.SharedDomain;
using NHibernate.Linq;

namespace ColloSys.AllocationService.CacsToLineWriteoff
{
    public static class DataAccess
    {
        public static IList<FileScheduler> ReadyToMoveFiles()
        {
            using (var session = SessionManager.GetNewSession())
            {
                using (var trans=session.BeginTransaction())
                {
                    var data = session.QueryOver<FileScheduler>()
                              .Where(x => x.ScbSystems == ScbEnums.ScbSystems.CACS)
                              .And(x => x.Category == ScbEnums.Category.Activity)
                              .And(x => x.AllocBillDone == false)
                              .Fetch(x => x.FileDetail).Eager
                              .List();
                    trans.Rollback();
                    return data;
                }
            }
        }

        public static IList<CacsActivity> GetDataFromCacs(FileScheduler fileScheduler)
        {
            using (var session = SessionManager.GetNewSession())
            {
                using (var trans = session.BeginTransaction())
                {
                    var data = session.QueryOver<CacsActivity>()
                             .Where(x => x.FileScheduler.Id == fileScheduler.Id)
                             .And(x=>x.ConsiderInAllocation)
                             //TODO: check excuse code and activity code from cacs
                             //.And( x=> x.ExcuseCode == "R")
                             //.And(x => x.ActivityCode.EndsWith("70"))
                             .List();
                    trans.Rollback();
                    return data;
                }
            }
           
        }
        
        public static IList<T> GetInfoData<T>(IEnumerable<CacsActivity> cacsActivities)
            where T : Info
        {
            var accNoList = cacsActivities.Select(x => x.AccountNo).ToList();
            using (var session=SessionManager.GetNewSession())
            {
                using (var trans=session.BeginTransaction())
                {
                    var data = session.Query<T>()
                                      .Where(x => accNoList.Contains(x.AccountNo) && x.AllocStatus==ColloSysEnums.AllocStatus.AllocateToTelecalling)
                                     // .And(x=>)
                                      .ToList();
                    trans.Rollback();
                    return data;
                }
            }
        }

        public static void SaveInfoDataWithFileSchedular<TInfo>(IEnumerable<TInfo> sharedInfos, FileScheduler fileScheduler)
            where TInfo:Info
        {
            using (var session=SessionManager.GetNewSession())
            {
                using (var trans=session.BeginTransaction())
                {
                    foreach (var sharedInfo in sharedInfos)
                    {
                        session.SaveOrUpdate(sharedInfo);
                    }
                    session.SaveOrUpdate(fileScheduler);
                    trans.Commit();
                }
            }
        }

        public static T GetAccountData<T>(CacsActivity cacsActivity)
            where T : Entity, IFileUploadable, IDelinquentCustomer
        {
            using (var session = SessionManager.GetNewSession())
            {
                using (var trans = session.BeginTransaction())
                {
                    var data = session.QueryOver<T>()
                              .Where(x => x.AccountNo == cacsActivity.AccountNo)
                              .OrderBy(x => x.FileDate).Desc.SingleOrDefault();
                    trans.Rollback();
                    return data;
                }
            }
        }


        public static void SaveCacsData(CacsData data)
        {
            using (var session = SessionManager.GetNewSession())
            {
                using (var trans = session.BeginTransaction())
                {
                    foreach (var entity in data.CacsList)
                    {
                        session.SaveOrUpdate(entity);
                    }
                    foreach (var entity in data.LinerList)
                    {
                        session.SaveOrUpdate(entity);
                    }
                    foreach (var entity in data.WriteoffList)
                    {
                        session.SaveOrUpdate(entity);
                    }
                    data.FileSchedular.AllocBillDone = true;
                    session.SaveOrUpdate(data.FileSchedular);
                    trans.Commit();
                }
            }
        }
    }
}

//public static void SaveOrUpdate(object entity)
//{
//    using (var session = SessionManager.GetNewSession())
//    {
//        using (var trans = session.BeginTransaction())
//        {
//            session.SaveOrUpdate(entity);
//            trans.Commit();
//        }
//    }
//}

//private static void SaveList<T>(IEnumerable<T> entities) where T: Entity
//{
//    using (var session=SessionManager.GetNewSession())
//    {
//        using (var trans=session.BeginTransaction())
//        {
//            foreach (var entity in entities)
//            {
//                session.SaveOrUpdate(entity);
//            }
//            trans.Commit();
//        }
//    }
//}