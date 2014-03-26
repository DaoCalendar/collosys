#region references

using System;
using System.Collections.Generic;
using System.Linq;
using ColloSys.AllocationService.Generic;
using ColloSys.DataLayer.Allocation;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SharedDomain;
using ColloSys.Shared.Types4Product;

#endregion

namespace ColloSys.AllocationService.AllocationLayer
{
    public static class HandleByTelecaller
    {
        public static List<Allocations> Init(IList<Info> dataOnCondition, AllocRelation relationCondition, ScbEnums.Products product)
        {
            var list = new List<Allocations>();

            dataOnCondition = dataOnCondition.Where(x => x.IsReferred).ToList();

            //calculate month start date and last date 
            var baseDate = Util.GetTodayDate();
            var hasMonthlyReset = DBLayer.DbLayer.IsMonthWiseReset(product);

            //no of months allocation not happen
            var notAllocateInMonths = (int)relationCondition.AllocSubpolicy.NoAllocMonth;
            notAllocateInMonths = notAllocateInMonths <= 1 ? 1 : notAllocateInMonths;

            //set object of sharedalloc 
            foreach (var dataObject in dataOnCondition)
            {
                //get cycle code form object
                uint cycleCode;
                try
                {
                    cycleCode = dataObject.Cycle;
                }
                catch (Exception)
                {
                    cycleCode = 0;
                }
                var thisMonthStart = (cycleCode == 0 || hasMonthlyReset)
                                         ? baseDate.AddDays(1 - baseDate.Day)
                                         : Util.ComputeStartDate((int)cycleCode);

                var thisMonthEnd = thisMonthStart.AddMonths(notAllocateInMonths).AddSeconds(-1);

                //create object of type
                var obj = new Allocations
                    {
                        AllocPolicy = relationCondition.AllocPolicy,
                        AllocSubpolicy = relationCondition.AllocSubpolicy,
                        IsAllocated = false,
                        StartDate = thisMonthStart,
                        EndDate = thisMonthEnd,
                        Bucket = 7,
                        WithTelecalling = false
                    };

                //set base properties appear in SharedAlloc
                //obj.AmountDue = 0;

                string accountno;

                var ralloc = SetAlloc(obj, dataObject, out accountno);
                list.Add(ralloc);
                dataObject.AllocStatus=ColloSysEnums.AllocStatus.AllocateToTelecalling;
            }

            //set allocstatus
            list.ForEach(x => x.AllocStatus = ColloSysEnums.AllocStatus.AllocateToTelecalling);
            dataOnCondition.Where(x=>x.IsReferred).ToList().ForEach(x=>x.IsReferred=false);
            return list;
        }

        private static Allocations SetAlloc(Allocations alloc, Info dataObject, out string accountno)
        {
            var ralloc = alloc;
            ralloc.Info = dataObject;
            ralloc.Bucket = (int)ralloc.Info.Bucket;
            accountno = ralloc.Info.AccountNo;
            ralloc.AmountDue = dataObject.TotalDue;

            //set allocstartdate and allocenddate for rinfo
            ralloc.Info.AllocStartDate = ralloc.StartDate;
            ralloc.Info.AllocEndDate = ralloc.EndDate;
            return ralloc;
        }


    }
}

//private static EAlloc SetEalloc(Alloc alloc, object dataObject, out string accountno)
//{
//    var ealloc = (EAlloc)alloc;
//    ealloc.EInfo = (EInfo)dataObject;
//    ealloc.Bucket = (int)ealloc.EInfo.Bucket;
//    accountno = ealloc.EInfo.AccountNo;
//    ealloc.AmountDue = ((EInfo)dataObject).TotalDue;
//    //set einfo allocstartdate and allocenddate
//    ealloc.EInfo.AllocStartDate = ealloc.StartDate;
//    ealloc.EInfo.AllocEndDate = ealloc.EndDate;
//    return ealloc;
//}

//private static CAlloc SetCalloc(Alloc alloc, object dataObject, out string accountno)
//{
//    var calloc = (CAlloc)alloc;
//    calloc.CInfo = (CInfo)dataObject;
//    calloc.Bucket = (int)calloc.CInfo.Bucket;
//    calloc.AmountDue = ((CInfo)dataObject).TotalDue;
//    accountno = calloc.CInfo.AccountNo;
//    //set cinfo allocstartdate and allocenddate
//    calloc.CInfo.AllocStartDate = calloc.StartDate;
//    calloc.CInfo.AllocEndDate = calloc.EndDate;
//    return calloc;
//}

//var listToRemove = (from dd in
//                        (from d in cacsData
//                         select new
//                             {
//                                 o = (from c in list
//                                      where c.AccountNo == d.AccountNo
//                                      select c).SingleOrDefault()
//                             }).ToList()
//                    select dd.o).ToList();
//list = list.Except(listToRemove).ToList();

//var listToAllocate = (from d in cacsData
//                     select new
//                         {
//                             o = (from c in list
//                                  where c.AccountNo != d.AccountNo
//                                  select c).SingleOrDefault()
//                         }).ToList();

//var listToAllocate2 = (from d in listToAllocate
//                   select d.o).ToList();

////set RAlloc object
//if (obj.GetType().IsAssignableFrom(typeof(RAlloc)))
//{

//}

////set EAlloc object
//if (obj.GetType().IsAssignableFrom(typeof(EAlloc)))
//{
//    var ealloc = SetEalloc(obj, dataObject, out accountno);
//    list.Add(ealloc);
//}

////set CAlloc object
//if (obj.GetType().IsAssignableFrom(typeof(CAlloc)))
//{
//    var calloc = SetCalloc(obj, dataObject, out accountno);
//    list.Add(calloc);
//}