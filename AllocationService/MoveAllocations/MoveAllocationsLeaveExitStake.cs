#region references

using System;
using System.Collections.Generic;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SharedDomain;
using ColloSys.QueryBuilder.AllocationBuilder;
using ColloSys.QueryBuilder.ClientDataBuilder;
using ColloSys.QueryBuilder.StakeholderBuilder;
using System.Linq;

#endregion


//stakeholders calls replaced
namespace ColloSys.AllocationService.MoveAllocations
{
    public static class MoveAllocationsLeaveExitStake
    {
        // private static ISession _session = SessionManager.GetCurrentSession();

        private static readonly StakeQueryBuilder StakeQueryBuilder=new StakeQueryBuilder(); 
        private static readonly AllocBuilder AllocBuilder=new AllocBuilder();
        private static readonly InfoBuilder InfoBuilder=new InfoBuilder();

        private static IEnumerable<Stakeholders> GetStakeholders(ScbEnums.Products products)
        {
            return StakeQueryBuilder.ExitedOnProduct(products);
        }

        private static IEnumerable<Stakeholders> GetStakeholdersOnLeave(ScbEnums.Products products)
        {
           return StakeQueryBuilder.OnLeaveOnProduct(products);
        }

        private static void SetAllocations(Stakeholders stakeholder, ScbEnums.Products products)
        {
            if (products == ScbEnums.Products.UNKNOWN)
                return;

            var listOfCAllocations = SelectAllocations(stakeholder).ToList();
            InfoBuilder.Save(listOfCAllocations);
        }

        private static IEnumerable<Info> SelectAllocations(Stakeholders stakeholder)
        {
            var listOfAllocatons = AllocBuilder.AllocationsForStakeholder(stakeholder).ToList();

            (listOfAllocatons).ForEach(x =>
            {
                x.AllocEndDate = DateTime.Now.AddDays(-1);
            });

            return listOfAllocatons;
        }
        
        public static void Init()
        {
            foreach (ScbEnums.Products products in Enum.GetValues(typeof(ScbEnums.Products)))
            {
                if (products == ScbEnums.Products.UNKNOWN)
                    continue;
                //for exit
                var listOfStakeholders = GetStakeholders(products);

                foreach (var stakeholder in listOfStakeholders)
                {
                    SetAllocations(stakeholder, products);
                }

                //for leave 
                var listOfStakeholdersOnLeave = GetStakeholdersOnLeave(products);
                foreach (var stakeholderse in listOfStakeholdersOnLeave)
                {
                    SetAllocations(stakeholderse, products);
                }
            }

        }
    }
}

//var systemOnProduct = Util.GetSystemOnProduct(products);
//switch (systemOnProduct)
//{
//    case ScbEnums.ScbSystems.CCMS:

//        break;
//    case ScbEnums.ScbSystems.EBBS:
//        var listOfEAllocations = SelectEAllocations(stakeholder);
//        DbLayer.SaveList(listOfEAllocations);
//        break;
//    case ScbEnums.ScbSystems.RLS:
//        var listOfRAllocations = SelectRAllocations(stakeholder);
//        DbLayer.SaveList(listOfRAllocations);
//        break;
//    default:
//        throw new ArgumentOutOfRangeException();
//}
//private static IEnumerable<EInfo> SelectEAllocations(Stakeholders stakeholder)
//{
//    var _session = SessionManager.GetCurrentSession();

//    var listOfAllocatons = _session.QueryOver<EAlloc>()
//                                   .Fetch(x => x.EInfo).Eager
//                                   .Fetch(x => x.Stakeholder).Eager
//                                   .Where(x => x.Stakeholder != null)
//                                   .And(x => x.Stakeholder.Id == stakeholder.Id)
//                                   .Select(x => x.EInfo)
//                                   .List<EInfo>();

//    ((List<EInfo>)listOfAllocatons).ForEach(x =>
//    {
//        x.AllocEndDate = DateTime.Now.AddDays(-1);
//    });

//    return listOfAllocatons;
//}

//private static IEnumerable<RInfo> SelectRAllocations(Stakeholders stakeholder)
//{
//    var _session = SessionManager.GetCurrentSession();

//    var listOfAllocatons = _session.QueryOver<RAlloc>()
//                                   .Fetch(x => x.RInfo).Eager
//                                   .Fetch(x => x.Stakeholder).Eager
//                                   .Where(x => x.Stakeholder != null)
//                                   .And(x => x.Stakeholder.Id == stakeholder.Id)
//                                   .Select(x => x.RInfo)
//                                   .List<RInfo>();
//    ((List<RInfo>)listOfAllocatons).ForEach(x =>
//    {
//        x.AllocEndDate = DateTime.Now.AddDays(-1);
//    });
//    return listOfAllocatons;
//}