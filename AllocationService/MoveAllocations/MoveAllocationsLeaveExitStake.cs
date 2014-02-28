using System;
using System.Collections.Generic;
using ColloSys.AllocationService.DBLayer;
using ColloSys.AllocationService.Generic;
using ColloSys.DataLayer.Allocation;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using NHibernate;
using NHibernate.SqlCommand;
using NHibernate.Transform;

namespace ColloSys.AllocationService.MoveAllocations
{
    public static class MoveAllocationsLeaveExitStake
    {
        // private static ISession _session = SessionManager.GetCurrentSession();

        public static IEnumerable<Stakeholders> GetStakeholders(ScbEnums.Products products)
        {
            Stakeholders stakeholders = null;
            StkhWorking working = null;
            StkhHierarchy hierarchy = null;
            var _session = SessionManager.GetCurrentSession();

            var listOfStakeholders = _session.QueryOver<Stakeholders>(() => stakeholders)
                                             .Fetch(x => x.StkhWorkings).Eager
                                             .Fetch(x => x.Hierarchy).Eager
                                             .JoinQueryOver(() => stakeholders.StkhWorkings, () => working,
                                                            JoinType.InnerJoin)
                                             .JoinQueryOver(() => stakeholders.Hierarchy, () => hierarchy,
                                                            JoinType.InnerJoin)
                                             .Where(() => stakeholders.LeavingDate < DateTime.Now)
                                             .And(() => stakeholders.Status == ColloSysEnums.ApproveStatus.Approved)
                                             .And(() => working.Products == products)
                                             .And(() => hierarchy.IsInAllocation)
                                             .And(() => hierarchy.IsInField)
                                             .TransformUsing(Transformers.DistinctRootEntity)
                                             .List();
            return listOfStakeholders;
        }

        public static IEnumerable<Stakeholders> GetStakeholdersOnLeave(ScbEnums.Products products)
        {
            Stakeholders stakeholders = null;
            StkhWorking working = null;
            StkhHierarchy hierarchy = null;
            var _session = SessionManager.GetCurrentSession();

            var listOfStakeholders = _session.QueryOver<Stakeholders>(() => stakeholders)
                                             .Fetch(x => x.StkhWorkings).Eager
                                             .Fetch(x => x.Hierarchy).Eager
                                             .JoinQueryOver(() => stakeholders.StkhWorkings, () => working,
                                                            JoinType.LeftOuterJoin)
                                             .JoinQueryOver(() => stakeholders.Hierarchy, () => hierarchy,
                                                            JoinType.LeftOuterJoin)
                                             .Where(() => stakeholders.LeavingDate == null || stakeholders.LeavingDate < DateTime.Now)
                                             .And(() => working.Products == products)
                                             .And(() => working.EndDate <= DateTime.Now)
                                             .And(() => stakeholders.Status == ColloSysEnums.ApproveStatus.Approved)
                                             .And(() => hierarchy.IsInAllocation)
                                             .And(() => hierarchy.IsInField)
                                             .TransformUsing(Transformers.DistinctRootEntity)
                                             .List();
            return listOfStakeholders;
        }

        public static void SetAllocations(Stakeholders stakeholder, ScbEnums.Products products)
        {
            if (products == ScbEnums.Products.UNKNOWN)
                return;
            var systemOnProduct = Util.GetSystemOnProduct(products);
            switch (systemOnProduct)
            {
                case ScbEnums.ScbSystems.CCMS:
                    var listOfCAllocations = SelectCAllocations(stakeholder);
                    DbLayer.SaveList(listOfCAllocations);
                    break;
                case ScbEnums.ScbSystems.EBBS:
                    var listOfEAllocations = SelectEAllocations(stakeholder);
                    DbLayer.SaveList(listOfEAllocations);
                    break;
                case ScbEnums.ScbSystems.RLS:
                    var listOfRAllocations = SelectRAllocations(stakeholder);
                    DbLayer.SaveList(listOfRAllocations);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static IEnumerable<EInfo> SelectEAllocations(Stakeholders stakeholder)
        {
            var _session = SessionManager.GetCurrentSession();

            var listOfAllocatons = _session.QueryOver<EAlloc>()
                                           .Fetch(x => x.EInfo).Eager
                                           .Fetch(x => x.Stakeholder).Eager
                                           .Where(x => x.Stakeholder != null)
                                           .And(x => x.Stakeholder.Id == stakeholder.Id)
                                           .Select(x => x.EInfo)
                                           .List<EInfo>();

            ((List<EInfo>)listOfAllocatons).ForEach(x =>
            {
                x.AllocEndDate = DateTime.Now.AddDays(-1);
            });

            return listOfAllocatons;
        }

        private static IEnumerable<CInfo> SelectCAllocations(Stakeholders stakeholder)
        {
            var _session = SessionManager.GetCurrentSession();

            var listOfAllocatons = _session.QueryOver<CAlloc>()
                                            .Fetch(x => x.CInfo).Eager
                                            .Fetch(x => x.Stakeholder).Eager
                                            .Where(x => x.Stakeholder != null)
                                            .And(x => x.Stakeholder.Id == stakeholder.Id)
                                            .Select(x => x.CInfo)
                                            .List<CInfo>();

            ((List<CInfo>)listOfAllocatons).ForEach(x =>
            {
                x.AllocEndDate = DateTime.Now.AddDays(-1);
            });

            return listOfAllocatons;
        }

        private static IEnumerable<RInfo> SelectRAllocations(Stakeholders stakeholder)
        {
            var _session = SessionManager.GetCurrentSession();

            var listOfAllocatons = _session.QueryOver<RAlloc>()
                                           .Fetch(x => x.RInfo).Eager
                                           .Fetch(x => x.Stakeholder).Eager
                                           .Where(x => x.Stakeholder != null)
                                           .And(x => x.Stakeholder.Id == stakeholder.Id)
                                           .Select(x => x.RInfo)
                                           .List<RInfo>();
            ((List<RInfo>)listOfAllocatons).ForEach(x =>
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