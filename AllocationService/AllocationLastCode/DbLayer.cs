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
using ColloSys.DataLayer.SharedDomain;
using ColloSys.QueryBuilder.ClientDataBuilder;

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
