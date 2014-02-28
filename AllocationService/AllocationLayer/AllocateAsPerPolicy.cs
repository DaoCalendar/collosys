#region references

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ColloSys.AllocationService.Generic;
using ColloSys.AllocationService.Models;
using ColloSys.DataLayer.Allocation;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SharedDomain;
using ColloSys.Shared.Types4Product;

#endregion


namespace ColloSys.AllocationService.AllocationLayer
{
    public static class AllocateAsPerPolicy
    {
        public static List<Entity> Init(Type classType, IList delqList, AllocRelation relationCondition,
            ScbEnums.Products product)
        {
            //allocatin list
            var list = new List<Entity>();

            //stakeholders with theire pincodes list
            var stakePincodeList = new List<StakePincodes>();

            //stakeholders list
            var stakeholdersList = DBLayer.DbLayer.GetListOfStakeholders(product);

            //get all pincodes
            var pincodeData = DBLayer.DbLayer.PincodeList();

            //pincodes on stakeholder level
            var pincodesOfStake = new List<GPincode>();

            foreach (var stakeholderse in stakeholdersList)
            {
                stakePincodeList.Add(new StakePincodes
                {
                    Stakeholders = stakeholderse,
                    Pincodes = GeneratePincodeList(stakeholderse, pincodeData)
                });
                pincodesOfStake.AddRange(GeneratePincodeList(stakeholderse, pincodeData));
            }

            stakePincodeList = stakePincodeList.Where(x => x.Pincodes.Count > 0).ToList();

            //calculate month start date and last date 
            var baseDate = Util.GetTodayDate();
            var hasMonthlyReset = DBLayer.DbLayer.IsMonthWiseReset(product);

            //no of months allocation not happen
            var notAllocateInMonths = (int)relationCondition.AllocSubpolicy.NoAllocMonth;
            notAllocateInMonths = notAllocateInMonths <= 0 ? 0 : notAllocateInMonths;

            foreach (var delqCust in delqList)
            {
                //get cycle code form object
                uint cycleCode;
                try
                {
                    cycleCode = (uint)delqCust.GetType().GetProperty("Cycle").GetValue(delqCust);
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
                obj.IsAllocated = true;
                obj.StartDate = thisMonthStart;
                obj.EndDate = thisMonthEnd;
                //obj.Stakeholder = relationCondition.AllocSubpolicy.Stakeholder;
                obj.Bucket = 7; //for writeoff 7, for liner set in respective method
                obj.WithTelecalling = false;

                string accountno;
                string customerName;

                var alloc = SetAlloc(obj, delqCust, out accountno, out customerName, stakePincodeList);
                if (alloc.Stakeholder != null)
                {
                    delqCust.GetType().GetProperty("AllocStatus").SetValue(delqCust, ColloSysEnums.AllocStatus.AsPerWorking);
                    alloc.AllocStatus = ColloSysEnums.AllocStatus.AsPerWorking;
                    list.Add(alloc);
                }
                else
                {
                    alloc.Info.AllocEndDate = null;
                }
            }
            
            return list;
        }

        public static List<GPincode> GeneratePincodeList(Stakeholders stakeholders
            , IList<GPincode> pincodeData)
        {
            var stkhpincodes = new List<GPincode>();
            foreach (var stkhWorking in stakeholders.StkhWorkings)
            {
                switch (stkhWorking.LocationLevel)
                {
                    case "Country":
                        stkhpincodes.AddRange(
                            pincodeData.Where(x => x.Country.Trim().ToUpperInvariant() == stkhWorking.Country.Trim().ToUpperInvariant()));
                        break;
                    case "Region":
                        stkhpincodes.AddRange(
                            pincodeData.Where(x => x.Region.Trim().ToUpperInvariant() == stkhWorking.Region.Trim().ToUpperInvariant()));
                        break;
                    case "Cluster":
                        stkhpincodes.AddRange(
                            pincodeData.Where(x => x.Region.Trim().ToUpperInvariant() == stkhWorking.Region.Trim().ToUpperInvariant() &&
                                x.State.Trim().ToUpperInvariant() == stkhWorking.State.Trim().ToUpperInvariant() &&
                            x.Cluster.Trim().ToUpperInvariant() == stkhWorking.Cluster.Trim().ToUpperInvariant()));
                        break;
                    case "State":
                        stkhpincodes.AddRange(
                           pincodeData.Where(x => x.Region.Trim().ToUpperInvariant() == stkhWorking.Region.Trim().ToUpperInvariant() &&
                           x.State.Trim().ToUpperInvariant() == stkhWorking.State.Trim().ToUpperInvariant()));
                        break;
                    case "District":
                        stkhpincodes.AddRange(
                            pincodeData.Where(x => x.Region.Trim().ToUpperInvariant() == stkhWorking.Region.Trim().ToUpperInvariant() &&
                            x.Cluster.Trim().ToUpperInvariant() == stkhWorking.Cluster.Trim().ToUpperInvariant() &&
                            x.State.Trim().ToUpperInvariant() == stkhWorking.State.Trim().ToUpperInvariant() &&
                            x.District.Trim().ToUpperInvariant() == stkhWorking.District.Trim().ToUpperInvariant()));
                        break;
                    case "City":
                        stkhpincodes.AddRange(
                            pincodeData.Where(x => x.Region.Trim().ToUpperInvariant() == stkhWorking.Region.Trim().ToUpperInvariant() &&
                            x.Cluster.Trim().ToUpperInvariant() == stkhWorking.Cluster.Trim().ToUpperInvariant() &&
                            x.State.Trim().ToUpperInvariant() == stkhWorking.State.Trim().ToUpperInvariant() &&
                            x.City.Trim().ToUpperInvariant() == stkhWorking.City.Trim().ToUpperInvariant()));
                        break;
                    case "Area":
                        stkhpincodes.AddRange(
                            pincodeData.Where(x => x.Region.Trim().ToUpperInvariant() == stkhWorking.Region.Trim().ToUpperInvariant() &&
                            x.Cluster.Trim().ToUpperInvariant() == stkhWorking.Cluster.Trim().ToUpperInvariant() &&
                            x.State.Trim().ToUpperInvariant() == stkhWorking.State.Trim().ToUpperInvariant() &&
                            x.City.Trim().ToUpperInvariant() == stkhWorking.City.Trim().ToUpperInvariant() &&
                            x.Area.Trim().ToUpperInvariant() == stkhWorking.Area.Trim().ToUpperInvariant()));
                        break;
                }
            }
            return stkhpincodes;
        }

        #region Private

        private static Alloc SetAlloc(Alloc obj,
            object dataObject, out string accno, out string customerName, List<StakePincodes> stakePincods)
        {
            var ralloc = (Alloc)obj;
            var gpincodeId = ((Info)dataObject).GPincode.Id;
            var bucket = ((Info) dataObject).Bucket;
            ralloc.Stakeholder = GetStakeholderForAllocation(gpincodeId, stakePincods,bucket);
            if (ralloc.Stakeholder == null)
            {
                ((Info)dataObject).AllocStatus = ColloSysEnums.AllocStatus.AllocationError;
                ((Info)dataObject).NoAllocResons = ColloSysEnums.NoAllocResons.NoStakeholder;
                ralloc.AllocStatus = ColloSysEnums.AllocStatus.AllocationError;
                ralloc.NoAllocResons = ColloSysEnums.NoAllocResons.NoStakeholder;
                ralloc.IsAllocated = false;
            }
            ralloc.Info = (Info)dataObject;
            accno = ralloc.Info.AccountNo;
            customerName = ralloc.Info.CustomerName;
            ralloc.Bucket = (int)ralloc.Info.Bucket;
            ralloc.AmountDue = ((Info)dataObject).TotalDue;
            ralloc.Info.AllocEndDate = ralloc.EndDate;
            ralloc.Info.AllocStartDate = ralloc.StartDate;
            var stakepincode = stakePincods.SingleOrDefault(x => x.Stakeholders != null &&
                                                                 ralloc.Stakeholder != null &&
                                                                 x.Stakeholders.Id == ralloc.Stakeholder.Id);
            if (stakepincode != null) stakepincode.Allocations.Add(ralloc);
            return ralloc;
        }

        private static Stakeholders GetStakeholderForAllocation(Guid gpincodeId, List<StakePincodes> stakePincods, uint bucket)
        {
            var list = stakePincods
                    .Where(x => x.Pincodes.Any(y => y.Id == gpincodeId) && x.Stakeholders.StkhWorkings.Any(y=>y.BucketStart==bucket))
                    .ToList();
            //TODO: Receive bucket as parameter and check any stakeholder working for that bucket
            //TODO: if bucket is 0 then stakeholder working for all buckets.
            //if (bucket != 0)
            //{
            //    list = list.Where(x => x.Stakeholders.StkhWorkings.Any(y => y.BucketStart == bucket));
            //}
            if (list.Count == 0)
            {
                return null;
            }

            if (list.Count == 1)
            {
                return list.First().Stakeholders;
            }

            var listSumAndStake = (from d in stakePincods
                                   group d by d.Stakeholders
                                       into d2
                                       select new
                                           {
                                               TotalAmount = Math.Abs(d2.Sum(x => x.Allocations.Sum(y => y.AmountDue))),
                                               Stakeholder = d2.Select(x => x.Stakeholders).SingleOrDefault()
                                           }).ToList();

            var min = listSumAndStake.Min(y => y.TotalAmount);
            var minstakeholder = listSumAndStake.Where(x => x.TotalAmount <= min)
                                                .Select(x => x.Stakeholder)
                                                .ToList().Cast<Stakeholders>()
                                                .First();
            //if (minstakeholder.Count() > 1)
            //    return (Stakeholders) minstakeholder.First();

            return  minstakeholder;
        }

        #endregion

    }
}

//set RAlloc object
//if (obj.GetType().IsAssignableFrom(typeof(RAlloc)))
//{
//    var ralloc = SetAlloc(obj, delqCust, out accountno, out customerName, stakePincodeList);
//    if (ralloc.Stakeholder != null)
//    {
//        delqCust.GetType().GetProperty("AllocStatus").SetValue(delqCust, ColloSysEnums.AllocStatus.AsPerWorking);
//        ralloc.AllocStatus = ColloSysEnums.AllocStatus.AsPerWorking;
//        list.Add(ralloc);
//    }
//    else
//    {
//        ralloc.RInfo.AllocEndDate = null; 
//    }
//}

//set EAlloc object
//if (obj.GetType().IsAssignableFrom(typeof(EAlloc)))
//{
//    var ealloc = SetEalloc(obj, delqCust, out accountno, out customerName, stakePincodeList);
//    if (ealloc.Stakeholder != null)
//    {
//        delqCust.GetType().GetProperty("AllocStatus").SetValue(delqCust, ColloSysEnums.AllocStatus.AsPerWorking);
//        ealloc.AllocStatus = ColloSysEnums.AllocStatus.AsPerWorking;
//        list.Add(ealloc);
//    }
//    else
//    {
//        ealloc.EInfo.AllocEndDate = null;
//    }

//}

////set CAlloc object
//if (obj.GetType().IsAssignableFrom(typeof(CAlloc)))
//{
//    var calloc = SetCalloc(obj, delqCust, out accountno, out customerName, stakePincodeList);

//    if (calloc.Stakeholder != null)
//    {
//        delqCust.GetType().GetProperty("AllocStatus").SetValue(delqCust, ColloSysEnums.AllocStatus.AsPerWorking);
//        calloc.AllocStatus = ColloSysEnums.AllocStatus.AsPerWorking;
//        list.Add(calloc);
//    }
//    else
//    {
//        calloc.CInfo.AllocEndDate = null;
//    }

//}
 //private static CAlloc SetCalloc(Alloc obj,
 //           object dataObject, out string accno, out string customerName, List<StakePincodes> stakePincods)
 //       {
 //           var calloc = (CAlloc)obj;
 //           var gpincodeId = ((CInfo)dataObject).GPincode.Id;
 //           var bucket = ((CInfo) dataObject).Bucket;

 //           //allocate to stakeholder
 //           calloc.Stakeholder = GetStakeholderForAllocation(gpincodeId, stakePincods,bucket);

 //           if (calloc.Stakeholder == null)
 //           {
 //               ((CInfo)dataObject).AllocStatus = ColloSysEnums.AllocStatus.AllocationError;
 //               ((CInfo)dataObject).NoAllocResons = ColloSysEnums.NoAllocResons.NoStakeholder;
 //               calloc.AllocStatus = ColloSysEnums.AllocStatus.AllocationError;
 //               calloc.NoAllocResons = ColloSysEnums.NoAllocResons.NoStakeholder;
 //               calloc.IsAllocated = false;
 //           }

 //           calloc.CInfo = (CInfo)dataObject;
 //           calloc.Bucket = (int)calloc.CInfo.Bucket;
 //           calloc.AmountDue = ((CInfo)dataObject).TotalDue;
 //           calloc.CInfo.AllocEndDate = calloc.EndDate;
 //           calloc.CInfo.AllocStartDate = calloc.StartDate;
 //           var stakepincode = stakePincods.SingleOrDefault(x => x.Stakeholders != null &&
 //                                                                calloc.Stakeholder != null &&
 //                                                                x.Stakeholders.Id == calloc.Stakeholder.Id);
 //           if (stakepincode != null)
 //               stakepincode.Allocations.Add(calloc);
 //           accno = calloc.CInfo.AccountNo;
 //           customerName = calloc.CInfo.CustomerName;

 //           return calloc;
 //       }

 //       private static EAlloc SetEalloc(Alloc obj,
 //           object dataObject, out string accno, out string customerName, List<StakePincodes> stakePincods)
 //       {
 //           var ealloc = (EAlloc)obj;
 //           var gpincodeId = ((EInfo)dataObject).GPincode.Id;
 //           var bucket = ((EInfo) dataObject).Bucket;
 //           ealloc.Stakeholder = GetStakeholderForAllocation(gpincodeId, stakePincods,bucket);
 //           if (ealloc.Stakeholder == null)
 //           {
 //               ((EInfo)dataObject).AllocStatus = ColloSysEnums.AllocStatus.AllocationError;
 //               ((EInfo)dataObject).NoAllocResons = ColloSysEnums.NoAllocResons.NoStakeholder;
 //               ealloc.AllocStatus = ColloSysEnums.AllocStatus.AllocationError;
 //               ealloc.NoAllocResons = ColloSysEnums.NoAllocResons.NoStakeholder;
 //               ealloc.IsAllocated = false;
 //           }
 //           ealloc.EInfo = (EInfo)dataObject;
 //           accno = ealloc.EInfo.AccountNo;
 //           customerName = ealloc.EInfo.CustomerName;
 //           ealloc.Bucket = (int)ealloc.EInfo.Bucket;
 //           ealloc.EInfo.AllocEndDate = ealloc.EndDate;
 //           ealloc.EInfo.AllocStartDate = ealloc.StartDate;
 //           ealloc.AmountDue = ((EInfo)dataObject).TotalDue;
 //           var stakepincode = stakePincods.SingleOrDefault(x => x.Stakeholders != null
 //                                                                && ealloc.Stakeholder != null &&
 //                                                                x.Stakeholders.Id == ealloc.Stakeholder.Id);
 //           if (stakepincode != null) stakepincode.Allocations.Add(ealloc);
 //           return ealloc;
 //       }
//private static RInfo SetRInfo(RAlloc ralloc, ulong accno, string customerName)
//{
//    var rInfo = DBLayer.DbLayer.GetInfo<RInfo>(accno) ?? new RInfo();
//    rInfo.AccountNo = accno;
//    rInfo.CustomerName = customerName;
//    rInfo.PinCode= ralloc
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