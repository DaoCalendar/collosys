using ColloSys.DataLayer.Generic;
using ColloSys.DataLayer.Stakeholder;

#region references

using System.Collections.Generic;
using System.Linq;
using ColloSys.DataLayer.Allocation;
using ColloSys.DataLayer.Enumerations;
using ColloSys.QueryBuilder.AllocationBuilder;
using ColloSys.QueryBuilder.GenericBuilder;
using ColloSys.QueryBuilder.StakeholderBuilder;

#endregion

//stakeholders calls changed

namespace ColloSys.AllocationService.DBLayer
{
    public static class DbLayer
    {
        private static readonly StakeQueryBuilder StakeQueryBuilder = new StakeQueryBuilder();
        private static readonly GPincodeBuilder GPincodeBuilder = new GPincodeBuilder();
        private static readonly ProductConfigBuilder ProductConfigBuilder = new ProductConfigBuilder();
        private static readonly AllocPolicyBuilder AllocPolicyBuilder = new AllocPolicyBuilder();
        
        public static AllocPolicy GetAllocationPolicy(ScbEnums.Products product, ScbEnums.Category category)
        {
            return AllocPolicyBuilder.OnProductAndSystem(product, category);
        }

        public static bool IsMonthWiseReset(ScbEnums.Products products)
        {
            var reset = ProductConfigBuilder.FilterBy(x => x.Product == products)
                               .Select(x => x.AllocationResetStrategy)
                               .SingleOrDefault();
            return reset == ColloSysEnums.AllocationPolicy.Monthly;
        }

        public static IEnumerable<Stakeholders> GetListOfStakeholders(ScbEnums.Products products)
        {
            return StakeQueryBuilder.OnProduct(products);
        }

        public static IList<GPincode> PincodeList()
        {
            return GPincodeBuilder.GetAll().ToList();
        }
    }
}

//public static AllocPolicy GetAllocationPolicyV2(ScbEnums.Products product, ScbEnums.Category category)
//{
//    using (var session = SessionManager.GetNewSession())
//    {
//        using (var trans = session.BeginTransaction())
//        {
//            var allocpolicy = session.CreateCriteria<AllocPolicy>("Policy")
//                                     .Add(Restrictions.Eq("Policy.Products", product))
//                                     .Add(Restrictions.Eq("Policy.Category", category))
//                                     .SetFetchMode("AllocRelations", FetchMode.Eager)
//                                     .SetFetchMode("AllocSubpolicy", FetchMode.Eager)
//                                     .SetFetchMode("Stakeholder", FetchMode.Eager)
//                                     .CreateCriteria("AllocRelations", "Relations", JoinType.LeftOuterJoin)
//                                     .CreateCriteria("AllocSubpolicy", "subpolicy", JoinType.LeftOuterJoin)
//                                     .Add(Restrictions.Eq("subpolicy.Status",
//                                                          ColloSysEnums.ApproveStatus.Approved))
//                                     .List<AllocPolicy>().First();
//            trans.Rollback();
//            return allocpolicy;
//        }
//    }
//}
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