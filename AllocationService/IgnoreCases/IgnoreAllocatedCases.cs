#region

using System;
using System.Collections.Generic;
using System.Linq;
using ColloSys.DataLayer.Allocation;
using ColloSys.DataLayer.BaseEntity;
//using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SharedDomain;

//using ColloSys.DataLayer.SharedDomain;

#endregion

namespace ColloSys.AllocationService.IgnoreCases
{
    public static class IgnoreAllocatedCases
    {
        public static IEnumerable<Entity> Init()
        {
            var listAll = new List<Entity>();
            foreach (ScbEnums.Products products in Enum.GetValues(typeof(ScbEnums.Products)))
            {
                if (products == ScbEnums.Products.UNKNOWN)
                    continue;

                var dataCLiner = DataAccess.GetInfoData(products);
                if (dataCLiner.Any())
                    listAll.AddRange(IgnoreLinerCases(dataCLiner));


            }
            //TODO: save this list to database
            return listAll;
        }

        private static IEnumerable<Entity> IgnoreLinerCases<TLiner>(IEnumerable<TLiner> linerList)
            where TLiner : Entity, IDelinquentCustomer
        //where TInfo : SharedInfo
        {
            return (from liner in linerList
                    select DataAccess.CheckInInfo(liner)
                    into oldLiner
                    where oldLiner != null
                    select SetAlloc(oldLiner as CustomerInfo))
                .Cast<Entity>()
                .ToList();
        }

        private static Allocations SetAlloc(CustomerInfo cInfo)
        {
            var calloc = new Allocations
                {
                    AllocPolicy = cInfo.Allocs.First().AllocPolicy,
                    AllocSubpolicy = cInfo.Allocs.First().AllocSubpolicy,
                    Bucket = (int)cInfo.Allocs.First().Bucket,
                    EndDate = cInfo.Allocs.First().EndDate,
                    StartDate = cInfo.Allocs.First().StartDate,
                    AmountDue = cInfo.Allocs.First().AmountDue,
                    IsAllocated = cInfo.Allocs.First().IsAllocated,
                    Info = cInfo
                };
            return calloc;
        }
    }
}
//private static RAlloc SetRAllocWriteoff(RWriteoff writeoff)
//{
//    var ralloc = new RAlloc
//        {
//            AllocPolicy = writeoff.Allocs.First().AllocPolicy,
//            AllocSubpolicy = writeoff.Allocs.First().AllocSubpolicy,
//            Bucket = writeoff.Allocs.First().Bucket,
//            EndDate = writeoff.Allocs.First().EndDate,
//            StartDate = writeoff.Allocs.First().StartDate,
//            AmountDue = writeoff.Allocs.First().AmountDue,
//            IsAllocated = writeoff.Allocs.First().IsAllocated,
//            RWriteoff = writeoff
//        };

//    return ralloc;
//}

//private static EAlloc SetEAllocWriteoff(EWriteoff writeoff)
//{
//    var ealloc = new EAlloc
//        {
//            AllocPolicy = writeoff.Allocs.First().AllocPolicy,
//            AllocSubpolicy = writeoff.Allocs.First().AllocSubpolicy,
//            Bucket = writeoff.Allocs.First().Bucket,
//            EndDate = writeoff.Allocs.First().EndDate,
//            StartDate = writeoff.Allocs.First().StartDate,
//            AmountDue = writeoff.Allocs.First().AmountDue,
//            IsAllocated = writeoff.Allocs.First().IsAllocated,
//            EWriteoff = writeoff
//        };

//    return ealloc;
//}

//private static CAlloc SetCAllocWriteoff(CWriteoff writeoff)
//{
//    var calloc = new CAlloc
//        {
//            AllocPolicy = writeoff.Allocs.First().AllocPolicy,
//            AllocSubpolicy = writeoff.Allocs.First().AllocSubpolicy,
//            Bucket = writeoff.Allocs.First().Bucket,
//            EndDate = writeoff.Allocs.First().EndDate,
//            StartDate = writeoff.Allocs.First().StartDate,
//            AmountDue = writeoff.Allocs.First().AmountDue,
//            IsAllocated = writeoff.Allocs.First().IsAllocated,
//            CWriteoff = writeoff
//        };

//    return calloc;
//}
//private static RAlloc SetRAllocLiner(RInfo rInfo)
//{
//    var alloc = rInfo.RAllocs.First();
//    var ralloc = new RAlloc
//        {
//            AllocPolicy = alloc.AllocPolicy,
//            AllocSubpolicy = alloc.AllocSubpolicy,
//            Bucket = alloc.Bucket,
//            EndDate = alloc.EndDate,
//            StartDate = alloc.StartDate,
//            AmountDue = alloc.AmountDue,
//            IsAllocated = alloc.IsAllocated,
//            RInfo = rInfo
//        };

//    return ralloc;
//}

//private static EAlloc SetEAllocLiner(EInfo eInfo)
//{
//    var ealloc = new EAlloc
//        {
//            AllocPolicy = eInfo.EAllocs.First().AllocPolicy,
//            AllocSubpolicy = eInfo.EAllocs.First().AllocSubpolicy,
//            Bucket = eInfo.EAllocs.First().Bucket,
//            EndDate = eInfo.EAllocs.First().EndDate,
//            StartDate = eInfo.EAllocs.First().StartDate,
//            AmountDue = eInfo.EAllocs.First().AmountDue,
//            IsAllocated = eInfo.EAllocs.First().IsAllocated,
//            EInfo = eInfo
//        };

//    return ealloc;
//}
//var systemOnProduct = Util.GetSystemOnProduct(products);
//switch (systemOnProduct)
//{
//    case ScbEnums.ScbSystems.CCMS:

//        break;
//    case ScbEnums.ScbSystems.EBBS:
//        var ealloc = SetEAllocLiner(oldLiner as EInfo);
//        listAllocs.Add(ealloc);
//        break;
//    case ScbEnums.ScbSystems.RLS:
//         var ralloc = SetRAllocLiner(oldLiner as RInfo);
//        listAllocs.Add(ralloc);
//        break;
//    default:
//        throw new ArgumentOutOfRangeException();
//}
//var systemOnProduct = Util.GetSystemOnProduct(products);
//switch (systemOnProduct)
//{
//    case ScbEnums.ScbSystems.CCMS:

//        break;
//    case ScbEnums.ScbSystems.EBBS:
//        var dataEliner = DataAccess.GetInfoData<EInfo>(products);
//        if(dataEliner.Any())
//        listAll.AddRange(IgnoreLinerCases(dataEliner, products));
//        break;
//    case ScbEnums.ScbSystems.RLS:
//         var dataRLiner = DataAccess.GetInfoData<RInfo>(products);
//        if(dataRLiner.Any())
//        listAll.AddRange(IgnoreLinerCases(dataRLiner, products));
//        break;
//    default:
//        throw new ArgumentOutOfRangeException();
//}
//private static IEnumerable<Entity> IgnoreWriteOffCases<TWriteoff>(IEnumerable<TWriteoff> writeoffList, ScbEnums.Products products)
//    where TWriteoff : Entity, IFileUploadable, IDelinquentCustomer
//    //where TInfo : SharedInfo
//{
//    var listAllocs = new List<Entity>();
//    foreach (var writeoff in writeoffList)
//    {
//        var oldWriteOff = DataAccess.CheckInInfo(writeoff);

//        if(oldWriteOff== null)
//            continue;
//        var systemOnProduct = Util.GetSystemOnProduct(products);
//        switch (systemOnProduct)
//        {
//            case ScbEnums.ScbSystems.CCMS:
//                var calloc = SetCAllocWriteoff(oldWriteOff as CWriteoff);
//                listAllocs.Add(calloc);
//                break;
//            case ScbEnums.ScbSystems.EBBS:
//                var ealloc = SetEAllocWriteoff(oldWriteOff as EWriteoff);
//                listAllocs.Add(ealloc);
//                break;
//            case ScbEnums.ScbSystems.RLS:
//                var ralloc = SetRAllocWriteoff(oldWriteOff as RWriteoff);
//                listAllocs.Add(ralloc);
//                break;
//            default:
//                throw new ArgumentOutOfRangeException();
//        }
//    }
//    return listAllocs;
//}
/*
 * 
 * 
   private static TAlloc SetAllocLinerRLiner<TAlloc, TLinerWriteoff>(TLinerWriteoff linerWriteoff)
            where TAlloc : SharedAlloc
            where TLinerWriteoff : Entity, IFileUploadable, ICustomerMandatoryInfo
        {
            var objAlloc = ClassType.CreateObject(typeof(TLinerWriteoff).Name);
            var ralloc = (RAlloc)objAlloc;
            if (objAlloc.GetType().IsAssignableFrom(typeof(EAlloc)))
            {
                ralloc.AllocPolicy = linerWriteoff.Allocs.AllocPolicy;
                ralloc.AllocSubpolicy = linerWriteoff.Allocs.AllocSubpolicy;
                ralloc.Bucket = linerWriteoff.Allocs.Bucket;
                ralloc.EndDate = linerWriteoff.Allocs.EndDate;
                ralloc.StartDate = linerWriteoff.Allocs.StartDate;//TODO:Start here
                ralloc.AmountDue = linerWriteoff.Allocs.AmountDue;
                ralloc.IsAllocated = linerWriteoff.Allocs.IsAllocated;
                ralloc.IsReferred = linerWriteoff.Allocs.IsReferred;
                ralloc.RLiner = linerWriteoff as RLiner;
            }
            objAlloc = ralloc;

            return (TAlloc)objAlloc;
        }

        private static TAlloc SetAllocLinerELiner<TAlloc, TLiner>(TLiner liner)
            where TAlloc : SharedAlloc
            where TLiner : Entity, IFileUploadable, ICustomerMandatoryInfo
        {
            var objAlloc = ClassType.CreateObject(typeof(TLiner).Name);
            var ealloc = (EAlloc)objAlloc;
            if (objAlloc.GetType().IsAssignableFrom(typeof(EAlloc)))
            {
                ealloc.AllocPolicy = liner.Allocs.AllocPolicy;
                ealloc.AllocSubpolicy = liner.Allocs.AllocSubpolicy;
                ealloc.Bucket = liner.Allocs.Bucket;
                ealloc.EndDate = liner.Allocs.EndDate;
                ealloc.StartDate = liner.Allocs.StartDate;
                ealloc.AmountDue = liner.Allocs.AmountDue;
                ealloc.IsAllocated = liner.Allocs.IsAllocated;
                ealloc.IsReferred = liner.Allocs.IsReferred;
                ealloc.ELiner = liner as ELiner;
            }
            objAlloc = ealloc;
            return (TAlloc)objAlloc;
        }

        private static TAlloc SetAllocLinerCLiner<TAlloc, TLiner>(TLiner liner)
            where TAlloc : SharedAlloc
            where TLiner : Entity, IFileUploadable, ICustomerMandatoryInfo
        {
            var objAlloc = ClassType.CreateObject(typeof(TLiner).Name);

            var clinerAlloc = (CAlloc)objAlloc;

            if (objAlloc.GetType().IsAssignableFrom(typeof(CAlloc)))
            {
                clinerAlloc.AllocPolicy = liner.Allocs.AllocPolicy;
                clinerAlloc.AllocSubpolicy = liner.Allocs.AllocSubpolicy;
                clinerAlloc.Bucket = liner.Allocs.Bucket;
                clinerAlloc.EndDate = liner.Allocs.EndDate;
                clinerAlloc.StartDate = liner.Allocs.StartDate;
                clinerAlloc.AmountDue = liner.Allocs.AmountDue;
                clinerAlloc.IsAllocated = liner.Allocs.IsAllocated;
                clinerAlloc.IsReferred = liner.Allocs.IsReferred;
                clinerAlloc.CLiner = liner as CLiner;
            }
            objAlloc = clinerAlloc;

            return (TAlloc)objAlloc;
        }

        private static TAlloc SetAllocWriteoffRWriteoff<TAlloc, TWriteoff>(TWriteoff writeoff)
            where TAlloc : SharedAlloc
            where TWriteoff : Entity, IFileUploadable, ICustomerMandatoryInfo
        {
            var objAlloc = ClassType.CreateObject(typeof(TWriteoff).Name);
            var rwriteoff = (RAlloc)objAlloc;

            if (objAlloc.GetType().IsAssignableFrom(typeof(RAlloc)))
            {
                rwriteoff.AllocPolicy = writeoff.Allocs.AllocPolicy;
                rwriteoff.AllocSubpolicy = writeoff.Allocs.AllocSubpolicy;
                rwriteoff.Bucket = writeoff.Allocs.Bucket;
                rwriteoff.EndDate = writeoff.Allocs.EndDate;
                rwriteoff.StartDate = writeoff.Allocs.StartDate;//TODO:Start here
                rwriteoff.AmountDue = writeoff.Allocs.AmountDue;
                rwriteoff.IsAllocated = writeoff.Allocs.IsAllocated;
                rwriteoff.IsReferred = writeoff.Allocs.IsReferred;
                rwriteoff.RWriteoff = writeoff as RWriteoff;
            }
            objAlloc = rwriteoff;
            return (TAlloc)objAlloc;
        }

        private static TAlloc SetAllocWriteoffEWriteoff<TAlloc, TWriteoff>(TWriteoff writeoff)
            where TAlloc : SharedAlloc
            where TWriteoff : Entity, IFileUploadable, ICustomerMandatoryInfo
        {
            var objAlloc = ClassType.CreateObject(typeof(TWriteoff).Name);

            var ewriteoff = (EAlloc)objAlloc;

            if (objAlloc.GetType().IsAssignableFrom(typeof(EAlloc)))
            {
                ewriteoff.AllocPolicy = writeoff.Allocs.AllocPolicy;
                ewriteoff.AllocSubpolicy = writeoff.Allocs.AllocSubpolicy;
                ewriteoff.Bucket = writeoff.Allocs.Bucket;
                ewriteoff.EndDate = writeoff.Allocs.EndDate;
                ewriteoff.StartDate = writeoff.Allocs.StartDate;
                ewriteoff.AmountDue = writeoff.Allocs.AmountDue;
                ewriteoff.IsAllocated = writeoff.Allocs.IsAllocated;
                ewriteoff.IsReferred = writeoff.Allocs.IsReferred;
                ewriteoff.EWriteoff = writeoff as EWriteoff;
            }
            objAlloc = ewriteoff;
            return (TAlloc)objAlloc;
        }

        private static TAlloc SetAllocWriteoffCWriteoff<TAlloc, TWriteoff>(TWriteoff writeoff)
            where TAlloc : SharedAlloc
            where TWriteoff : Entity, IFileUploadable, ICustomerMandatoryInfo
        {
            var objAlloc = ClassType.CreateObject(typeof(TWriteoff).Name);
            var cwriteoff = (CAlloc)objAlloc;

            if (objAlloc.GetType().IsAssignableFrom(typeof(CAlloc)))
            {
                cwriteoff.AllocPolicy = writeoff.Allocs.AllocPolicy;
                cwriteoff.AllocSubpolicy = writeoff.Allocs.AllocSubpolicy;
                cwriteoff.Bucket = writeoff.Allocs.Bucket;
                cwriteoff.EndDate = writeoff.Allocs.EndDate;
                cwriteoff.StartDate = writeoff.Allocs.StartDate;
                cwriteoff.AmountDue = writeoff.Allocs.AmountDue;
                cwriteoff.IsAllocated = writeoff.Allocs.IsAllocated;
                cwriteoff.IsReferred = writeoff.Allocs.IsReferred;
                cwriteoff.CWriteoff = writeoff as CWriteoff;
            }
            objAlloc = cwriteoff;
            return (TAlloc)objAlloc;
        }
 */
