#region references

using System;
using System.Collections.Generic;
using System.Linq;
using ColloSys.AllocationService.Generic;
using ColloSys.DataLayer.Allocation;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SharedDomain;

#endregion

namespace ColloSys.AllocationService.AllocationLastCode
{
    public static class UnAllocatedCases
    {
        public static void Init(ScbEnums.Products products)
        {
            var system = Util.GetSystemOnProduct(products);
            switch (system)
            {

                case ScbEnums.ScbSystems.CCMS:
                    var cInfoData = DbLayer.GetUnAllocatedCases<CInfo>(products);
                    DbLayer.SaveList(SetList(cInfoData, products));
                    var cAllocList = SetCAllocList(cInfoData);
                    DbLayer.SaveList(cAllocList);
                    break;

                case ScbEnums.ScbSystems.EBBS:
                    var eInfoData = DbLayer.GetUnAllocatedCases<EInfo>(products);
                    DbLayer.SaveList(SetList(eInfoData, products));
                    var eAllocList = SetEAllocList(eInfoData);
                    DbLayer.SaveList(eAllocList);
                    break;

                case ScbEnums.ScbSystems.RLS:
                     var rInfoData = DbLayer.GetUnAllocatedCases<RInfo>(products);
                    DbLayer.SaveList(SetList(rInfoData, products));
                    var rAllocList = SetRAllocList(rInfoData);
                    DbLayer.SaveList(rAllocList);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
        }

        private static IList<EAlloc> SetEAllocList(List<EInfo> eInfoData)
        {
            var allocList = new List<EAlloc>();
            if (eInfoData == null || eInfoData.Count == 0)
                return allocList;
            foreach (var cInfo in eInfoData)
            {
                var alloc = new EAlloc();
                alloc.IsAllocated = false;
                alloc.AmountDue = cInfo.TotalDue;
                alloc.AllocStatus = cInfo.AllocStatus;
                alloc.NoAllocResons = cInfo.NoAllocResons;
                alloc.Bucket = (int)cInfo.Bucket;
                alloc.StartDate = cInfo.AllocStartDate.HasValue ? cInfo.AllocStartDate.Value : DateTime.Today;
                alloc.EndDate = cInfo.AllocEndDate.HasValue ? cInfo.AllocEndDate.Value : DateTime.Today.AddMonths(1);
                alloc.EInfo = cInfo;
                allocList.Add(alloc);
            }
            return allocList;
        }

        private static IList<CAlloc> SetCAllocList(List<CInfo> cInfoData)
        {
            var allocList = new List<CAlloc>();
            if (cInfoData == null || cInfoData.Count == 0)
                return allocList;
            foreach (var cInfo in cInfoData)
            {
                var alloc = new CAlloc();
                alloc.IsAllocated = false;
                alloc.AmountDue = cInfo.TotalDue;
                alloc.AllocStatus = cInfo.AllocStatus;
                alloc.NoAllocResons = cInfo.NoAllocResons;
                alloc.Bucket = (int)cInfo.Bucket;
                alloc.StartDate = cInfo.AllocStartDate.HasValue ? cInfo.AllocStartDate.Value : DateTime.Today;
                alloc.EndDate = cInfo.AllocEndDate.HasValue ? cInfo.AllocEndDate.Value : DateTime.Today.AddMonths(1);
                alloc.CInfo = cInfo;
                allocList.Add(alloc);
            }
            return allocList;
        }

        private static IList<RAlloc> SetRAllocList(List<RInfo> rInfoData)
        {
            var allocList = new List<RAlloc>();
            if (rInfoData == null || rInfoData.Count == 0)
                return allocList;
            foreach (var rInfo in rInfoData)
            {
                var alloc = new RAlloc();
                alloc.IsAllocated = false;
                alloc.AmountDue = rInfo.TotalDue;
                alloc.AllocStatus = rInfo.AllocStatus;
                alloc.NoAllocResons = rInfo.NoAllocResons;
                alloc.Bucket = (int)rInfo.Bucket;
                alloc.StartDate = rInfo.AllocStartDate.HasValue ? rInfo.AllocStartDate.Value : DateTime.Today;
                alloc.EndDate = rInfo.AllocEndDate.HasValue ? rInfo.AllocEndDate.Value : DateTime.Today.AddMonths(1);
                alloc.RInfo = rInfo;
                allocList.Add(alloc);
            }
            return allocList;
        }

        private static List<T> SetList<T>(List<T> list, ScbEnums.Products products)
            where T : Info
        {
            if (list.Count == 0)
                return list;

            var hasMonthlyReset = DBLayer.DbLayer.IsMonthWiseReset(products);
            var baseDate = Util.GetTodayDate();
            foreach (var entity in list)
            {

                entity.AllocStartDate = (entity.Cycle == 0 || hasMonthlyReset)
                                    ? baseDate.AddDays(1 - baseDate.Day)
                                    : Util.ComputeStartDate((int)entity.Cycle);

                entity.AllocEndDate = entity.AllocStartDate.Value.AddMonths(1).AddSeconds(-1);
            }

            foreach (var entity in list.Where(x => x.Pincode == 0 || x.GPincode == null))
            {
                entity.AllocStatus = ColloSysEnums.AllocStatus.AllocationError;
                entity.NoAllocResons = ColloSysEnums.NoAllocResons.MissingPincode;
            }
            //foreach (var entity in list.Where(entity => (entity.AllocStatus == ColloSysEnums.AllocStatus.AllocationError 
            //    || entity.AllocStatus!=ColloSysEnums.AllocStatus.None)
            //    && entity.NoAllocResons != ColloSysEnums.NoAllocResons.MissingPincode))
            //{
            //    entity.AllocStartDate = (entity.Cycle == 0 || hasMonthlyReset)
            //                        ? baseDate.AddDays(1 - baseDate.Day)
            //                        : Util.ComputeStartDate((int)entity.Cycle);

            //    entity.AllocEndDate = entity.AllocStartDate.Value.AddMonths(1).AddSeconds(-1);
            //}
            return list;
        }
    }
}
