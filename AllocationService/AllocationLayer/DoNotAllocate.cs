#region references

using System;
using System.Collections;
using System.Collections.Generic;
using ColloSys.AllocationService.Generic;
using ColloSys.DataLayer.Allocation;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SharedDomain;
using ColloSys.Shared.Types4Product;

#endregion


namespace ColloSys.AllocationService.AllocationLayer
{
    public static class DoNotAllocate
    {
        public static List<Entity> SetDoNotAllocateAc(Type classType, IList data, AllocRelation relationCondition,
            ScbEnums.Products product)
        {
            var list = new List<Entity>();

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
                    cycleCode = (uint)dataObject.GetType().GetProperty("Cycle").GetValue(dataObject);
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
                var obj = ClassType.CreateAllocObject(classType.Name);

                //set base properties appear in SharedAlloc
                obj.AllocPolicy = relationCondition.AllocPolicy;
                obj.AllocSubpolicy = relationCondition.AllocSubpolicy;
                obj.IsAllocated = false;
                obj.StartDate = thisMonthStart;
                obj.EndDate = thisMonthEnd;
                //obj.AmountDue = 0;
                obj.Bucket = 7; //for writeoff 7, for liner set in respective method
                obj.WithTelecalling = false;

                string accountno;

                //set RAlloc object
                if (obj.GetType().IsAssignableFrom(typeof(RAlloc)))
                {
                    var ralloc = SetRalloc(obj, dataObject, out accountno);
                    list.Add(ralloc);

                }

                //set EAlloc object
                if (obj.GetType().IsAssignableFrom(typeof(EAlloc)))
                {
                    var ealloc = SetEalloc(obj, dataObject, out accountno);

                    //var eInfo = SetEInfo(thisMonthStart, thisMonthEnd, ealloc, accountno);

                    list.Add(ealloc);
                    //list.Add(eInfo);
                }

                //set CAlloc object
                if (obj.GetType().IsAssignableFrom(typeof(CAlloc)))
                {
                    var calloc = SetCalloc(obj, dataObject, out accountno);

                    //var cInfo = SetCInfo(thisMonthStart, thisMonthEnd, calloc, accountno);

                    list.Add(calloc);
                    //list.Add(cInfo);
                }
                dataObject.GetType().GetProperty("AllocStatus").SetValue(dataObject, ColloSysEnums.AllocStatus.DoNotAllocate);
            }

            //set allocstatus
            list.ForEach(x=>((SharedAlloc)x).AllocStatus=ColloSysEnums.AllocStatus.DoNotAllocate);
            return list;
        }



        #region Private

      

        private static CAlloc SetCalloc(SharedAlloc obj, object dataObject, out string accno)
        {
            var calloc = (CAlloc)obj;
            calloc.CInfo = (CInfo)dataObject;
            calloc.Bucket = (int)calloc.CInfo.Bucket;
            calloc.AmountDue = ((CInfo)dataObject).TotalDue;
            accno = calloc.CInfo.AccountNo;
            //set cinfo allocstartdate and allocenddate
            calloc.CInfo.AllocStartDate = calloc.StartDate;
            calloc.CInfo.AllocEndDate = calloc.EndDate;
            return calloc;
        }

        private static EAlloc SetEalloc(SharedAlloc obj, object dataObject, out string accno)
        {
            var ealloc = (EAlloc)obj;
            ealloc.EInfo = (EInfo)dataObject;
            ealloc.Bucket = (int)ealloc.EInfo.Bucket;
            accno = ealloc.EInfo.AccountNo;
            ealloc.AmountDue = ((EInfo)dataObject).TotalDue;
            //set einfo allocstartdate and allocenddate
            ealloc.EInfo.AllocStartDate = ealloc.StartDate;
            ealloc.EInfo.AllocEndDate = ealloc.EndDate;
            return ealloc;
        }

        private static RAlloc SetRalloc(SharedAlloc obj, object dataObject, out string accno)
        {
            var ralloc = (RAlloc)obj;
            ralloc.RInfo = (RInfo)dataObject;
            ralloc.Bucket = (int)ralloc.RInfo.Bucket;
            accno = ralloc.RInfo.AccountNo;
            ralloc.AmountDue = ((RInfo)dataObject).TotalDue;

            //set allocstartdate and allocenddate for rinfo
            ralloc.RInfo.AllocStartDate = ralloc.StartDate;
            ralloc.RInfo.AllocEndDate = ralloc.EndDate;
            return ralloc;
        }

        #endregion
    }
}

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