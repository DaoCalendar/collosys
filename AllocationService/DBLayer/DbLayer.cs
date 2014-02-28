#region references

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ColloSys.AllocationService.Generic;
using ColloSys.DataLayer.Allocation;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.SqlCommand;
using NHibernate.Transform;

#endregion

namespace ColloSys.AllocationService.DBLayer
{
    public static class DbLayer
    {
        /// <summary>
        /// Get Allocation Policy for product and category basis
        /// </summary>
        /// <param name="product"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        public static AllocPolicy GetAllocationPolicy(ScbEnums.Products product, ScbEnums.Category category)
        {
            using (var session = SessionManager.GetNewSession())
            {
                using (var trans = session.BeginTransaction())
                {
                    AllocPolicy policy = null;
                    AllocRelation relation = null;
                    AllocSubpolicy subpolicy = null;
                    AllocCondition condition = null;
                    Stakeholders stakeholder = null;

                    var allocPolicy = session.QueryOver(() => policy)
                                             .Fetch(x => x.AllocRelations).Eager
                                             .Fetch(x => x.AllocRelations.First().AllocSubpolicy).Eager
                                             .Fetch(x => x.AllocRelations.First().AllocSubpolicy.Conditions).Eager
                                             .Fetch(x => x.AllocRelations.First().AllocSubpolicy.Stakeholder).Eager
                                             .JoinAlias(() => policy.AllocRelations, () => relation, JoinType.LeftOuterJoin)
                                             .JoinAlias(() => relation.AllocSubpolicy, () => subpolicy, JoinType.LeftOuterJoin)
                                             .JoinAlias(() => subpolicy.Conditions, () => condition, JoinType.LeftOuterJoin)
                                             .JoinAlias(() => subpolicy.Stakeholder, () => stakeholder, JoinType.LeftOuterJoin)
                                             .Where(() => policy.Products == product && policy.Category == category)
                                             .And(() => relation.Status == ColloSysEnums.ApproveStatus.Approved)
                                             .And(() => relation.StartDate <= Util.GetTodayDate() &&
                                                        (relation.EndDate == null ||
                                                         relation.EndDate.Value >= Util.GetTodayDate()))
                                             .SingleOrDefault();

                    trans.Rollback();
                    return allocPolicy;
                }
            }
        }

        public static bool IsMonthWiseReset(ScbEnums.Products products)
        {
            using (var session = SessionManager.GetNewSession())
            {
                using (var trans = session.BeginTransaction())
                {
                    var reset = session.Query<ProductConfig>()
                                       .Where(x => x.Product == products)
                                       .Select(x => x.AllocationResetStrategy)
                                       .Cacheable()
                                       .SingleOrDefault();
                    trans.Rollback();
                    return reset == ColloSysEnums.AllocationPolicy.Monthly;
                }
            }
        }


        public static IList<Stakeholders> GetListOfStakeholders(ScbEnums.Products products)
        {
            Stakeholders stakeholders = null;
            StkhWorking workings = null;
            StkhHierarchy hierarchy = null;
            using (var session = SessionManager.GetNewSession())
            {
                using (var trans = session.BeginTransaction())
                {
                    var data = session.QueryOver(() => stakeholders)
                                      .Fetch(x => x.StkhWorkings).Eager
                                      .JoinAlias(() => stakeholders.StkhWorkings, () => workings, JoinType.LeftOuterJoin)
                                      .JoinAlias(() => stakeholders.Hierarchy, () => hierarchy,
                                                 JoinType.LeftOuterJoin)
                                      .Where(() => workings.Products == products)
                                      .And(() => hierarchy.IsInAllocation)
                                      .And(()=>hierarchy.IsInField)
                                      .And(() => stakeholders.JoiningDate < Util.GetTodayDate())
                                      .And(() => stakeholders.LeavingDate == null ||
                                                 stakeholders.LeavingDate > Util.GetTodayDate())
                                      .And(() => stakeholders.Status == ColloSysEnums.ApproveStatus.Approved)
                                      .TransformUsing(Transformers.DistinctRootEntity)
                                      .List();
                    trans.Rollback();
                    return data;
                }
            }
        }

        public static IList<GPincode> PincodeList()
        {
            using (var session = SessionManager.GetNewSession())
            {
                using (var trans = session.BeginTransaction())
                {
                    var data = session.Query<GPincode>()
                                         .ToList();
                    trans.Rollback();
                    return data;
                }
            }
        }

        public static void SaveList<T>(IEnumerable<T> entityList)
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

        public static void SaveObjectList(IList entityList)
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


        public static AllocPolicy GetAllocationPolicyV2(ScbEnums.Products product, ScbEnums.Category category)
        {
            using (var session = SessionManager.GetNewSession())
            {
                using (var trans = session.BeginTransaction())
                {
                    var allocpolicy = session.CreateCriteria<AllocPolicy>("Policy")
                                             .Add(Restrictions.Eq("Policy.Products", product))
                                             .Add(Restrictions.Eq("Policy.Category", category))
                                             .SetFetchMode("AllocRelations", FetchMode.Eager)
                                             .SetFetchMode("AllocSubpolicy", FetchMode.Eager)
                                             .SetFetchMode("Stakeholder", FetchMode.Eager)
                                             .CreateCriteria("AllocRelations", "Relations", JoinType.LeftOuterJoin)
                                             .CreateCriteria("AllocSubpolicy", "subpolicy", JoinType.LeftOuterJoin)
                                             .Add(Restrictions.Eq("subpolicy.Status",
                                                                  ColloSysEnums.ApproveStatus.Approved))
                                             .List<AllocPolicy>().First();
                    trans.Rollback();
                    return allocpolicy;
                }
            }
        }
    }
}
/*
public static bool IsAcNoExistInDatabaseRInfo(decimal AccountNo)
        {
            var session = SessionManager.GetCurrentSession();
            var isExistInDatabase = session.Query<RInfo>()
                                           .Where(x => x.AccountNo == AccountNo)
                                           .Select(x => x.AccountNo).SingleOrDefault();
            return isExistInDatabase != 0.0M;
        }
        public static bool IsAcNoExistInDatabaseEInfo(decimal AccountNo)
        {
            var session = SessionManager.GetCurrentSession();
            var isExistInDatabase = session.Query<EInfo>()
                                           .Where(x => x.AccountNo == AccountNo)
                                           .Select(x => x.AccountNo).SingleOrDefault();
            return isExistInDatabase != 0.0M;
        }
        public static bool IsAcNoExistInDatabaseCInfo(decimal AccountNo)
        {
            var session = SessionManager.GetCurrentSession();
            var isExistInDatabase = session.Query<CInfo>()
                                           .Where(x => x.AccountNo == AccountNo)
                                           .Select(x => x.AccountNo).SingleOrDefault();
            return isExistInDatabase != 0.0M;
        }
 * 
 *  public static RInfo GetRInfo(decimal accountNo)
        {
            if (accountNo == default(decimal))
                return null;
            var rinfo = SessionManager.GetCurrentSession()
                                      .Query<RInfo>()
                                      .Where(x => x.AccountNo == accountNo)
                                      .Select(x => x).SingleOrDefault();
            return rinfo;

        }

        public static EInfo GetEInfo(decimal accountNo)
        {
            if (accountNo == default(decimal))
                return null;
            var einfo = SessionManager.GetCurrentSession()
                                      .Query<EInfo>()
                                      .Where(x => x.AccountNo == accountNo)
                                      .Select(x => x).SingleOrDefault();
            return einfo;
        }

        public static CInfo GetCInfo(decimal accountNo)
        {
            if (accountNo == default(decimal))
                return null;
            var cinfo = SessionManager.GetCurrentSession()
                                      .Query<CInfo>()
                                      .Where(x => x.AccountNo == accountNo)
                                      .Select(x => x).SingleOrDefault();
            return cinfo;
        }

*/

//var policies = _session.QueryOver(() => policy)
//                        .JoinAlias(() => policy.AllocRelations, () => relation)
//                        //.Fetch(x=>x.AllocRelations).Eager
//                        .JoinAlias(() => relation.AllocSubpolicy, () => subpolicy)
//                        //.JoinAlias(() => subpolicy.Conditions.F, () => condition)
//                        .Where(() => policy.Products == product && policy.Category == category)
//                        .And(() => policy.Status == EnumHelper.ApproveStatus.Approved)
//                        .And(() => relation.StartDate <= DateTime.Now && relation.EndDate >= DateTime.Now)
//                        .SingleOrDefault();
//         QueryOver<A>() 
//.JoinAlias(x => x.B, () => bAlias) 
//.Where(() => bAlias.Surname == "Smith") 
//.Fetch(x => x.B).Eager 
//.JoinQueryOver(() => bAlias.C)


/*
        public static bool DoesAcNoExistInDatabseInfo<T>(string accountNo)
            where T : SharedInfo
        {
            using (var session = SessionManager.GetNewSession())
            {
                using (var trans = session.BeginTransaction())
                {
                    var isExistInDatabase = session.Query<T>()
                                                   .Where(x => x.AccountNo != null &&  x.AccountNo == accountNo )
                                                   .Select(x => x.AccountNo)
                                                   .Count();
                    trans.Rollback();
                    return isExistInDatabase > 0;

                }
            }
        }
*/

/*
        public static T GetInfo<T>(string accountNo) where T : SharedInfo
        {
            if (accountNo == string.Empty) return null;

            using (var session = SessionManager.GetNewSession())
            {
                using (var trans = session.BeginTransaction())
                {
                    var cinfo = session.Query<T>()
                                              .Where(x => x.AccountNo != null && x.AccountNo == accountNo)
                                              .Select(x => x)
                                              .SingleOrDefault();
                    trans.Rollback();
                    return cinfo;
                }
            }
        }
*/