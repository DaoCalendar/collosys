#region references

using System.Collections.Generic;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SharedDomain;
using ColloSys.QueryBuilder.ClientDataBuilder;

#endregion


namespace ColloSys.AllocationService.AllocationLastCode
{
    public class DbLayer
    {
        private static readonly InfoBuilder InfoBuilder=new InfoBuilder();
        public static IEnumerable<Info> GetUnAllocatedCases(ScbEnums.Products products)
        {
            return InfoBuilder.UnAllocatedCases(products);
        }

        //public static void SaveList(IList<T> entityList)
        //    where T : Entity
        //{
        //    using (var session = SessionManager.GetNewSession())
        //    {
        //        using (var trans = session.BeginTransaction())
        //        {
        //            foreach (var entity in entityList)
        //            {
        //                session.SaveOrUpdate(entity);
        //            }
        //            trans.Commit();
        //        }
        //    }
        //}

    }
}
