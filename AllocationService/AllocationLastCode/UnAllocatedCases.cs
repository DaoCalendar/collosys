#region references

using System;
using System.Collections.Generic;
using System.Linq;
using ColloSys.AllocationService.Generic;
using ColloSys.DataLayer.Allocation;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SharedDomain;
using ColloSys.QueryBuilder.AllocationBuilder;
using ColloSys.QueryBuilder.ClientDataBuilder;

#endregion

namespace ColloSys.AllocationService.AllocationLastCode
{
    public static class UnAllocatedCases
    {
        private static readonly InfoBuilder InfoBuilder =new InfoBuilder();
        private static readonly AllocBuilder AllocBuilder=new AllocBuilder();

        public static void Init(ScbEnums.Products products)
        {
            var infoData =InfoBuilder.UnAllocatedCases(products).ToList();
            InfoBuilder.Save(SetList(infoData,products));
            var allocList = SetAllocList(infoData);
            AllocBuilder.Save(allocList);
        }

        private static IEnumerable<Allocations> SetAllocList(List<CustomerInfo> eInfoData)
        {
            var allocList = new List<Allocations>();
            if (eInfoData == null || eInfoData.Count == 0)
                return allocList;
            allocList.AddRange(eInfoData.Select(cInfo => new Allocations
                {
                    IsAllocated = false,
                    AmountDue = cInfo.TotalDue,
                    AllocStatus = cInfo.AllocStatus,
                    NoAllocResons = cInfo.NoAllocResons,
                    Bucket = (int) cInfo.Bucket,
                    StartDate = cInfo.AllocStartDate.HasValue
                                    ? cInfo.AllocStartDate.Value
                                    : DateTime.Today,
                    EndDate = cInfo.AllocEndDate.HasValue
                                  ? cInfo.AllocEndDate.Value
                                  : DateTime.Today.AddMonths(1),
                    Info = cInfo
                }));
            return allocList;
        }

        private static IEnumerable<T> SetList<T>(IReadOnlyCollection<T> list, ScbEnums.Products products)
            where T : CustomerInfo
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
            
            return list;
        }
    }
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

//var system = Util.GetSystemOnProduct(products);
//switch (system)
//{

//    case ScbEnums.ScbSystems.CCMS:
//        var cInfoData = DbLayer.GetUnAllocatedCases<CInfo>(products);
//        DbLayer.SaveList(SetList(cInfoData, products));
//        var cAllocList = SetCAllocList(cInfoData);
//        DbLayer.SaveList(cAllocList);
//        break;

//    case ScbEnums.ScbSystems.EBBS:
//        var eInfoData = DbLayer.GetUnAllocatedCases<EInfo>(products);
//        DbLayer.SaveList(SetList(eInfoData, products));
//        var eAllocList = SetAllocList(eInfoData);
//        DbLayer.SaveList(eAllocList);
//        break;

//    case ScbEnums.ScbSystems.RLS:
//         var rInfoData = DbLayer.GetUnAllocatedCases<RInfo>(products);
//        DbLayer.SaveList(SetList(rInfoData, products));
//        var rAllocList = SetRAllocList(rInfoData);
//        DbLayer.SaveList(rAllocList);
//        break;
//    default:
//        throw new ArgumentOutOfRangeException();
//}

//private static IList<CAlloc> SetCAllocList(List<CInfo> cInfoData)
//{
//    var allocList = new List<CAlloc>();
//    if (cInfoData == null || cInfoData.Count == 0)
//        return allocList;
//    foreach (var cInfo in cInfoData)
//    {
//        var alloc = new CAlloc();
//        alloc.IsAllocated = false;
//        alloc.AmountDue = cInfo.TotalDue;
//        alloc.AllocStatus = cInfo.AllocStatus;
//        alloc.NoAllocResons = cInfo.NoAllocResons;
//        alloc.Bucket = (int)cInfo.Bucket;
//        alloc.StartDate = cInfo.AllocStartDate.HasValue ? cInfo.AllocStartDate.Value : DateTime.Today;
//        alloc.EndDate = cInfo.AllocEndDate.HasValue ? cInfo.AllocEndDate.Value : DateTime.Today.AddMonths(1);
//        alloc.CInfo = cInfo;
//        allocList.Add(alloc);
//    }
//    return allocList;
//}

//private static IList<RAlloc> SetRAllocList(List<RInfo> rInfoData)
//{
//    var allocList = new List<RAlloc>();
//    if (rInfoData == null || rInfoData.Count == 0)
//        return allocList;
//    foreach (var rInfo in rInfoData)
//    {
//        var alloc = new RAlloc();
//        alloc.IsAllocated = false;
//        alloc.AmountDue = rInfo.TotalDue;
//        alloc.AllocStatus = rInfo.AllocStatus;
//        alloc.NoAllocResons = rInfo.NoAllocResons;
//        alloc.Bucket = (int)rInfo.Bucket;
//        alloc.StartDate = rInfo.AllocStartDate.HasValue ? rInfo.AllocStartDate.Value : DateTime.Today;
//        alloc.EndDate = rInfo.AllocEndDate.HasValue ? rInfo.AllocEndDate.Value : DateTime.Today.AddMonths(1);
//        alloc.RInfo = rInfo;
//        allocList.Add(alloc);
//    }
//    return allocList;
//}
