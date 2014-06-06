#region references

using System.Collections.Generic;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SessionMgr;

#endregion


namespace ColloSys.AllocationService.FRCases
{
    public class DataAccess
    {
        public static IList<CacsActivity> GetDataFromCacs(FileScheduler fileScheduler,ScbEnums.Products products)
        {
            using (var session = SessionManager.GetNewSession())
            {
                using (var trans = session.BeginTransaction())
                {
                    var data = session.QueryOver<CacsActivity>()
                             .Where(x => x.FileScheduler.Id == fileScheduler.Id)
                             .And(x => x.ConsiderInAllocation)
                             .And(x=>x.Products==products)
                        //TODO: check excuse code and activity code from cacs
                        //.And( x=> x.ExcuseCode == "R")
                        //.And(x => x.ActivityCode.EndsWith("70"))
                             .List();
                    trans.Rollback();
                    return data;
                }
            }

        }
    }
}

//public static IList<CacsActivity> GetDataFromCacs(ScbEnums.Products products)
//{
//    using (var session = SessionManager.GetNewSession())
//    {
//        using (var trans = session.BeginTransaction())
//        {
//            var data = session.QueryOver<CacsActivity>()
//                     .Where(x => x.Products == products)
//                     .And(x => x.ConsiderInAllocation)
//                     .And(x=>x.FileDate==Util.GetTodayDate())
//                     .List();
//            trans.Rollback();
//            return data;
//        }
//    }
//}

//public static IList<FileScheduler> ReadyToMoveFiles()
//{
//    using (var session = SessionManager.GetNewSession())
//    {
//        using (var trans = session.BeginTransaction())
//        {
//            var data = session.QueryOver<FileScheduler>()
//                      .Where(x => x.ScbSystems == ScbEnums.ScbSystems.CACS)
//                      .And(x => x.Category == ScbEnums.Category.Activity)
//                      .And(x => x.AllocBillDone == false)
//                      .Fetch(x => x.FileDetail).Eager
//                      .List();
//            trans.Rollback();
//            return data;
//        }
//    }
//}