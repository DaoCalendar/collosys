using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ColloSys.AllocationService.Generic;
using ColloSys.DataLayer.Allocation;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SharedDomain;
using ColloSys.Shared.Types4Product;

namespace ColloSys.AllocationService.AllocationLayer
{
    public static class HandleByTelecaller
    {
        public static List<Entity> Init(Type classType, IList dataOnCondition, AllocRelation relationCondition, ScbEnums.Products product)
        {
            var list = new List<Entity>();

            //var cacsData = FRCases.DataAccess.GetDataFromCacs(product);

            //dataOnCondition = GetCasesToAllocate(dataOnCondition, cacsData);

            dataOnCondition = dataOnCondition.Cast<Info>().Where(x => x.IsReferred).ToList();

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
                    cycleCode = (uint)dataObject.GetType().GetProperty("Cycle").GetValue(dataObject);
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
                    list.Add(ealloc);
                }

                //set CAlloc object
                if (obj.GetType().IsAssignableFrom(typeof(CAlloc)))
                {
                    var calloc = SetCalloc(obj, dataObject, out accountno);
                    list.Add(calloc);
                }
                dataObject.GetType().GetProperty("AllocStatus").SetValue(dataObject, ColloSysEnums.AllocStatus.AllocateToTelecalling);
            }

            //set allocstatus
            list.ForEach(x => ((Alloc)x).AllocStatus = ColloSysEnums.AllocStatus.AllocateToTelecalling);
            dataOnCondition.Cast<Info>().Where(x=>x.IsReferred).ToList().ForEach(x=>x.IsReferred=false);
            return list;
        }

        private static IList GetCasesToAllocate(IList dataOnCondition, IList<CacsActivity> cacsData)
        {
            var list = dataOnCondition.Cast<Info>().ToList();// ConvertList(dataOnCondition);
            var accounts = cacsData.Select(x => x.AccountNo).ToList();
            list = list.Where(x => !accounts.Contains(x.AccountNo)).ToList();

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



            return list;
        }

        private static EAlloc SetEalloc(Alloc alloc, object dataObject, out string accountno)
        {
            var ealloc = (EAlloc)alloc;
            ealloc.EInfo = (EInfo)dataObject;
            ealloc.Bucket = (int)ealloc.EInfo.Bucket;
            accountno = ealloc.EInfo.AccountNo;
            ealloc.AmountDue = ((EInfo)dataObject).TotalDue;
            //set einfo allocstartdate and allocenddate
            ealloc.EInfo.AllocStartDate = ealloc.StartDate;
            ealloc.EInfo.AllocEndDate = ealloc.EndDate;
            return ealloc;
        }

        private static RAlloc SetRalloc(Alloc alloc, object dataObject, out string accountno)
        {
            var ralloc = (RAlloc)alloc;
            ralloc.RInfo = (RInfo)dataObject;
            ralloc.Bucket = (int)ralloc.RInfo.Bucket;
            accountno = ralloc.RInfo.AccountNo;
            ralloc.AmountDue = ((RInfo)dataObject).TotalDue;

            //set allocstartdate and allocenddate for rinfo
            ralloc.RInfo.AllocStartDate = ralloc.StartDate;
            ralloc.RInfo.AllocEndDate = ralloc.EndDate;
            return ralloc;
        }

        private static CAlloc SetCalloc(Alloc alloc, object dataObject, out string accountno)
        {
            var calloc = (CAlloc)alloc;
            calloc.CInfo = (CInfo)dataObject;
            calloc.Bucket = (int)calloc.CInfo.Bucket;
            calloc.AmountDue = ((CInfo)dataObject).TotalDue;
            accountno = calloc.CInfo.AccountNo;
            //set cinfo allocstartdate and allocenddate
            calloc.CInfo.AllocStartDate = calloc.StartDate;
            calloc.CInfo.AllocEndDate = calloc.EndDate;
            return calloc;
        }
    }
}