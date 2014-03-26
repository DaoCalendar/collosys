#region references

using System;
using System.Collections.Generic;
using ColloSys.AllocationService.Generic;
using ColloSys.DataLayer.Allocation;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SharedDomain;

#endregion


namespace ColloSys.AllocationService.AllocationLayer
{
    public static class DoNotAllocate
    {
        public static List<Allocations> SetDoNotAllocateAc(IEnumerable<Info> data, AllocRelation relationCondition,
            ScbEnums.Products product)
        {
            var list = new List<Allocations>();

            //calculate month start date and last date 
            var baseDate = Util.GetTodayDate();
            var hasMonthlyReset = DBLayer.DbLayer.IsMonthWiseReset(product);

            //no of months allocation not happen
            var notAllocateInMonths = (int)relationCondition.AllocSubpolicy.NoAllocMonth;
            notAllocateInMonths = notAllocateInMonths <= 0 ? 0 : notAllocateInMonths;

            //set object of sharedalloc 
            foreach (var dataObject in data)
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

                var thisMonthEnd = notAllocateInMonths == 0
                                       ? DateTime.MaxValue
                                       : thisMonthStart.AddMonths(notAllocateInMonths).AddSeconds(-1);

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

                string accountno;

                var ralloc = SetAlloc(obj, dataObject, out accountno);
                list.Add(ralloc);
                dataObject.AllocStatus = ColloSysEnums.AllocStatus.DoNotAllocate;
            }

            //set allocstatus
            list.ForEach(x => x.AllocStatus = ColloSysEnums.AllocStatus.DoNotAllocate);
            return list;
        }

        #region Private
        private static Allocations SetAlloc(Allocations obj, Info dataObject, out string accno)
        {
            var ralloc = obj;
            ralloc.Info = dataObject;
            ralloc.Bucket = (int)ralloc.Info.Bucket;
            accno = ralloc.Info.AccountNo;
            ralloc.AmountDue = dataObject.TotalDue;

            //set allocstartdate and allocenddate for rinfo
            ralloc.Info.AllocStartDate = ralloc.StartDate;
            ralloc.Info.AllocEndDate = ralloc.EndDate;
            return ralloc;
        }

        #endregion
    }
}

////set RAlloc object
//if (obj.GetType().IsAssignableFrom(typeof(RAlloc)))
//{


//}

////set EAlloc object
//if (obj.GetType().IsAssignableFrom(typeof(EAlloc)))
//{
//    var ealloc = SetEalloc(obj, dataObject, out accountno);

//    //var eInfo = SetEInfo(thisMonthStart, thisMonthEnd, ealloc, accountno);

//    list.Add(ealloc);
//    //list.Add(eInfo);
//}

////set CAlloc object
//if (obj.GetType().IsAssignableFrom(typeof(CAlloc)))
//{
//    var calloc = SetCalloc(obj, dataObject, out accountno);

//    //var cInfo = SetCInfo(thisMonthStart, thisMonthEnd, calloc, accountno);

//    list.Add(calloc);
//    //list.Add(cInfo);
//}
//private static CAlloc SetCalloc(Alloc obj, object dataObject, out string accno)
//{
//    var calloc = (CAlloc)obj;
//    calloc.CInfo = (CInfo)dataObject;
//    calloc.Bucket = (int)calloc.CInfo.Bucket;
//    calloc.AmountDue = ((CInfo)dataObject).TotalDue;
//    accno = calloc.CInfo.AccountNo;
//    //set cinfo allocstartdate and allocenddate
//    calloc.CInfo.AllocStartDate = calloc.StartDate;
//    calloc.CInfo.AllocEndDate = calloc.EndDate;
//    return calloc;
//}

//private static EAlloc SetEalloc(Alloc obj, object dataObject, out string accno)
//{
//    var ealloc = (EAlloc)obj;
//    ealloc.EInfo = (EInfo)dataObject;
//    ealloc.Bucket = (int)ealloc.EInfo.Bucket;
//    accno = ealloc.EInfo.AccountNo;
//    ealloc.AmountDue = ((EInfo)dataObject).TotalDue;
//    //set einfo allocstartdate and allocenddate
//    ealloc.EInfo.AllocStartDate = ealloc.StartDate;
//    ealloc.EInfo.AllocEndDate = ealloc.EndDate;
//    return ealloc;
//}
//private static RInfo SetRInfo(DateTime thisMonthStart, DateTime thisMonthEnd, RAlloc ralloc, ulong accno)
//{
//    var rInfo = DBLayer.DbLayer.GetInfo<RInfo>(accno) ?? new RInfo();
//    rInfo.AccountNo = accno;
//    rInfo.AllocStartDate = thisMonthStart;
//    rInfo.AllocEndDate = thisMonthEnd;
//    rInfo.DoAllocate = false;
//    rInfo.AllocPolicy = ralloc.AllocPolicy;
//    rInfo.AllocSubpolicy = ralloc.AllocSubpolicy;
//    return rInfo;
//}

//private static EInfo SetEInfo(DateTime thisMonthStart, DateTime thisMonthEnd, EAlloc ealloc, ulong accno)
//{
//    var eInfo = DBLayer.DbLayer.GetInfo<EInfo>(accno) ?? new EInfo();
//    eInfo.AccountNo = accno;
//    eInfo.AllocStartDate = thisMonthStart;
//    eInfo.AllocEndDate = thisMonthEnd;
//    eInfo.DoAllocate = false;
//    eInfo.AllocPolicy = ealloc.AllocPolicy;
//    eInfo.AllocSubpolicy = ealloc.AllocSubpolicy;
//    return eInfo;
//}

//private static CInfo SetCInfo(DateTime thisMonthStart, DateTime thisMonthEnd, CAlloc cAlloc, ulong accno)
//{
//    var cInfo = DBLayer.DbLayer.GetInfo<CInfo>(accno) ?? new CInfo();
//    cInfo.AccountNo = accno;
//    cInfo.AllocStartDate = thisMonthStart;
//    cInfo.AllocEndDate = thisMonthEnd;
//    cInfo.DoAllocate = false;
//    cInfo.AllocPolicy = cAlloc.AllocPolicy;
//    cInfo.AllocSubpolicy = cAlloc.AllocSubpolicy;
//    return cInfo;
//}