using System;
using System.Collections.Generic;
using System.Linq;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.SharedDomain;
using ColloSys.QueryBuilder.AllocationBuilder;
using ColloSys.QueryBuilder.StakeholderBuilder;
using Itenso.TimePeriod;

//stakeholders calls changed
namespace ColloSys.UserInterface.Areas.Allocation.ViewModels
{
    public class BulkAllocationModel
    {
        private static readonly StakeQueryBuilder StakeQuery = new StakeQueryBuilder();
        private static readonly AllocBuilder AllocBuilder=new AllocBuilder();

        public BulkAllocationModel()
        {
            SaveRAllocs=new List<Alloc>();
            SaveEAllocs=new List<Alloc>();
            SaveCAllocs=new List<Alloc>();
        }

        public ScbEnums.Products Product { get; set; }
        public ScbEnums.Category Category { get; set; }
        public Guid Stakeholder { get; set; }
        public ColloSysEnums.ChangeAllocReason ChangeReason { get; set; }
        public Guid ToStakeholder { get; set; }
        public ColloSysEnums.AllocationType AllocationType { get; set; }


        public IList<Alloc> RAllocs { get; set; }
        public IList<Alloc> CAllocs { get; set; }
        public IList<Alloc> EAllocs { get; set; }

        public IList<Alloc> SaveRAllocs { get; set; }
        public IList<Alloc> SaveCAllocs { get; set; }
        public IList<Alloc> SaveEAllocs { get; set; }

        public BulkAllocationModel GetAllocations(BulkAllocationModel model)
        {
            switch (model.Product)
            {
                case ScbEnums.Products.UNKNOWN:
                    break;
                case ScbEnums.Products.SME_BIL:
                case ScbEnums.Products.SME_ME:
                case ScbEnums.Products.SME_LAP:
                case ScbEnums.Products.AUTO:
                case ScbEnums.Products.PL:
                case ScbEnums.Products.MORT:
                    model.RAllocs = GetAllocationsForAlloc(model);
                    break;

                case ScbEnums.Products.CC:
                    model.CAllocs = GetAllocationsForAlloc(model);
                    break;

                case ScbEnums.Products.AUTO_OD:
                case ScbEnums.Products.SME_LAP_OD:
                case ScbEnums.Products.SMC:
                    model.EAllocs = GetAllocationsForAlloc(model);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return this;
        }

        #region get allocations 

        private static IList<Alloc> GetAllocationsForAlloc(BulkAllocationModel model)
        {
            return AllocBuilder.AllocationsForStakeholder(model.Stakeholder, model.Product).ToList();
        }

        #endregion


        #region save allocations without churn option

        public BulkAllocationModel SaveAllocationsWithoutChurn(BulkAllocationModel model)
        {
            model = ClearSavingLists(model);
            switch (model.Product)
            {
                case ScbEnums.Products.UNKNOWN:
                    break;

                case ScbEnums.Products.SME_BIL:
                case ScbEnums.Products.SME_ME:
                case ScbEnums.Products.SME_LAP:
                case ScbEnums.Products.PL:
                case ScbEnums.Products.AUTO:
                case ScbEnums.Products.MORT:
                    SaveRAllocations(model);
                    break;

                case ScbEnums.Products.CC:
                    SaveCAllocations(model);
                    break;

                case ScbEnums.Products.AUTO_OD:
                case ScbEnums.Products.SME_LAP_OD:
                case ScbEnums.Products.SMC:
                    SaveEAllocations(model);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
            return model;
        }

        #endregion

        #region Save Allocations for RLS, EBBS, CCMS

        private static void SaveRAllocations(BulkAllocationModel model)
        {
            if (model.AllocationType == ColloSysEnums.AllocationType.DoNotAllocate)
            {
                model = ChangeRAllocationDoNotAllocate(model);
                AllocBuilder.Save(model.SaveRAllocs.First());
                AllocBuilder.Save(model.RAllocs.First());
                //SaveList(model.SaveRAllocs);
                //SaveList(model.RAllocs);
                return;
            }
            if (model.AllocationType == ColloSysEnums.AllocationType.AllocateToStkholder)
            {
                model = ChangeRAllocationsToStakeholder(model);
                AllocBuilder.Save(model.RAllocs[1]);
                AllocBuilder.Save(model.SaveRAllocs[1]);
                //SaveList(model.RAllocs);
                //SaveList(model.SaveRAllocs);
            }

        }

        private static void SaveCAllocations(BulkAllocationModel model)
        {
            if (model.AllocationType == ColloSysEnums.AllocationType.DoNotAllocate)
            {
                model = ChangeCAllocationDoNotAllocate(model);
                AllocBuilder.Save(model.CAllocs);
                AllocBuilder.Save(model.SaveCAllocs);
                return;
            }
            if (model.AllocationType == ColloSysEnums.AllocationType.AllocateToStkholder)
            {
                model = ChangeCAllocationsToStakeholder(model);
                AllocBuilder.Save(model.CAllocs);
                AllocBuilder.Save(model.SaveCAllocs);
            }
        }

        private static void SaveEAllocations(BulkAllocationModel model)
        {
            if (model.AllocationType == ColloSysEnums.AllocationType.DoNotAllocate)
            {
                model = ChangeEAllocationDoNotAllocate(model);
                AllocBuilder.Save(model.EAllocs);
                AllocBuilder.Save(model.SaveCAllocs);
                return;
            }
            if (model.AllocationType == ColloSysEnums.AllocationType.AllocateToStkholder)
            {
                model = ChangeEAllocationsToStakeholder(model);
                AllocBuilder.Save(model.EAllocs);
                AllocBuilder.Save(model.SaveEAllocs);
            }
        }

        #endregion

        #region Change Allocations for DoNotAllocate

        private static BulkAllocationModel ChangeRAllocationDoNotAllocate(BulkAllocationModel model)
        {
            foreach (var rAlloc in model.RAllocs)
            {
                var rAllocNew = DeepClone(rAlloc);
                rAllocNew.ChangeReason = model.ChangeReason.ToString();
                rAllocNew.Id = new Guid();
                rAllocNew.Version = default(int);
                rAllocNew.StartDate = DateTime.Today.AddDays(1);
                var monthDiff = new DateDiff(rAlloc.StartDate, rAlloc.EndDate.Value).Months;
                rAllocNew.EndDate = DateTime.Today.AddMonths(monthDiff > 0 ? monthDiff : 1);
                rAlloc.EndDate = DateTime.Today;
                if (model.Category == ScbEnums.Category.Liner)
                {
                    //rAllocNew.RLiner.AllocStatus = ColloSysEnums.AllocStatus.DoNotAllocate;
                }
                else
                {
                    //rAlloc.RWriteoff.AllocStatus = ColloSysEnums.AllocStatus.DoNotAllocate;
                }
                model.SaveRAllocs.Add(rAllocNew);
            }

            return model;
        }

        private static BulkAllocationModel ChangeCAllocationDoNotAllocate(BulkAllocationModel model)
        {
            foreach (var cAlloc in model.CAllocs)
            {
                var cAllocNew = DeepClone(cAlloc);
                cAllocNew.ChangeReason = model.ChangeReason.ToString();
                cAllocNew.Id = new Guid();
                cAllocNew.Version = default(int);
                cAllocNew.StartDate = DateTime.Today.AddDays(1);
                var monthDiff = new DateDiff(cAlloc.StartDate, cAlloc.EndDate.Value).Months;
                cAllocNew.EndDate = DateTime.Today.AddMonths(monthDiff > 0 ? monthDiff : 1);
                cAlloc.EndDate = DateTime.Today;
                if (model.Category == ScbEnums.Category.Liner)
                {
                    //cAllocNew.CLiner.AllocStatus = ColloSysEnums.AllocStatus.DoNotAllocate;
                }
                else
                {
                    //cAlloc.CWriteoff.AllocStatus = ColloSysEnums.AllocStatus.DoNotAllocate;
                }
                model.SaveCAllocs.Add(cAllocNew);
            }
            return model;
        }

        private static BulkAllocationModel ChangeEAllocationDoNotAllocate(BulkAllocationModel model)
        {
            foreach (var eAlloc in model.EAllocs)
            {
                var eAllocNew = DeepClone(eAlloc);
                eAllocNew.ChangeReason = model.ChangeReason.ToString();
                eAllocNew.Id = new Guid();
                eAllocNew.Version = default(int);
                eAllocNew.StartDate = DateTime.Today.AddDays(1);
                var monthDiff = new DateDiff(eAlloc.StartDate, eAlloc.EndDate.Value).Months;
                eAllocNew.EndDate = DateTime.Today.AddMonths(monthDiff > 0 ? monthDiff : 1);
                eAlloc.EndDate = DateTime.Today;
                if (model.Category == ScbEnums.Category.Liner)
                {
                    //eAllocNew.ELiner.AllocStatus = ColloSysEnums.AllocStatus.DoNotAllocate;
                }
                else
                {
                    //eAlloc.EWriteoff.AllocStatus = ColloSysEnums.AllocStatus.DoNotAllocate;
                }
                model.SaveEAllocs.Add(eAllocNew);
            }
            return model;
        }

        #endregion

        #region Change Allocations for Allocate to Stakeholder

        private static BulkAllocationModel ChangeRAllocationsToStakeholder(BulkAllocationModel model)
        {
            foreach (var rAlloc in model.RAllocs)
            {
                var rAllocNew = DeepClone(rAlloc);
                rAllocNew.ChangeReason = model.ChangeReason.ToString();
                rAllocNew.Id = new Guid();
                rAllocNew.Version = default(int);
                rAllocNew.StartDate = DateTime.Today.AddDays(1);
                var monthDiff = new DateDiff(rAlloc.StartDate, rAlloc.EndDate.Value).Months;
                rAllocNew.EndDate = DateTime.Today.AddMonths(monthDiff > 0 ? monthDiff : 1);
                rAlloc.EndDate = DateTime.Today;
                rAllocNew.Stakeholder = GetStakeholder(model.ToStakeholder);
                 if (model.Category == ScbEnums.Category.Liner)
                 {
                     //rAlloc.RLiner.AllocStatus = ColloSysEnums.AllocStatus.AllocateToStakeholder;
                 }
                 else
                 {
                     //rAlloc.RWriteoff.AllocStatus = ColloSysEnums.AllocStatus.DoNotAllocate;
                 }
                model.SaveRAllocs.Add(rAllocNew);
            }
           
            return model;
        }

        private static BulkAllocationModel ChangeCAllocationsToStakeholder(BulkAllocationModel model)
        {
            foreach (var cAlloc in model.CAllocs)
            {
                var cAllocNew = DeepClone(cAlloc);
                cAllocNew.ChangeReason = model.ChangeReason.ToString();
                cAllocNew.Id = new Guid();
                cAllocNew.Version = default(int);
                cAllocNew.StartDate = DateTime.Today.AddDays(1);
                var monthDiff = new DateDiff(cAlloc.StartDate, cAlloc.EndDate.Value).Months;
                cAllocNew.EndDate = DateTime.Today.AddMonths(monthDiff > 0 ? monthDiff : 1);
                cAlloc.EndDate = DateTime.Today;
                cAllocNew.Stakeholder = GetStakeholder(model.ToStakeholder);
                if (model.Category == ScbEnums.Category.Liner)
                {
                    //cAlloc.CLiner.AllocStatus = ColloSysEnums.AllocStatus.AllocateToStakeholder;
                }
                else
                {
                    //cAlloc.CWriteoff.AllocStatus = ColloSysEnums.AllocStatus.DoNotAllocate;
                }
                model.SaveCAllocs.Add(cAllocNew);
            }
            return model;
        }

        private static BulkAllocationModel ChangeEAllocationsToStakeholder(BulkAllocationModel model)
        {
            foreach (var eAlloc in model.EAllocs)
            {
                var eAllocNew = DeepClone(eAlloc);
                eAllocNew.ChangeReason = model.ChangeReason.ToString();
                eAllocNew.Id = new Guid();
                eAllocNew.Version = default(int);
                eAllocNew.StartDate = DateTime.Today.AddDays(1);
                var monthDiff = new DateDiff(eAlloc.StartDate, eAlloc.EndDate.Value).Months;
                eAllocNew.EndDate = DateTime.Today.AddMonths(monthDiff > 0 ? monthDiff : 1);
                eAlloc.EndDate = DateTime.Today;
                eAllocNew.Stakeholder = GetStakeholder(model.ToStakeholder);
                if (model.Category == ScbEnums.Category.Liner)
                {
                    //eAlloc.ELiner.AllocStatus = ColloSysEnums.AllocStatus.AllocateToStakeholder;
                }
                else
                {
                    //eAlloc.EWriteoff.AllocStatus = ColloSysEnums.AllocStatus.DoNotAllocate;
                }
                model.SaveEAllocs.Add(eAllocNew);
            }
            return model;
        }

        #endregion

        #region get stakeholder(id)

        private static Stakeholders GetStakeholder(Guid stakeholdersId)
        {
            return StakeQuery.Get(stakeholdersId);
        }

        #endregion


        #region page utilities

        private static T DeepClone<T>(T obj)
        {
            if (obj == null)
            {
                return Activator.CreateInstance<T>();
            }
            var newPerson = Activator.CreateInstance<T>();
            var fields = newPerson.GetType().GetProperties();
            foreach (var field in fields)
            {
                var value = field.GetValue(obj);
                field.SetValue(newPerson, value);
            }
            return newPerson;
        }

        private static BulkAllocationModel ClearSavingLists(BulkAllocationModel model)
        {
            model.SaveCAllocs.Clear();
            model.SaveEAllocs.Clear();
            model.SaveRAllocs.Clear();
            return model;
        }

        #endregion

    }
}

//private static IList<Alloc> GetAllocationsForCAlloc(BulkAllocationModel model)
//{
//    using (var session = SessionManager.GetNewSession())
//    {
//        using (var trans = session.BeginTransaction())
//        {
//            var data = session.QueryOver<Alloc>()
//                              .Fetch(x => x.AllocPolicy).Eager
//                              .Fetch(x => x.AllocSubpolicy).Eager
//                              //.Fetch(x => x.CLiner).Eager
//                              //.Fetch(x => x.CWriteoff).Eager
//                              .Fetch(x => x.Stakeholder).Eager
//                              .Where(x => x.Stakeholder.Id == model.Stakeholder)
//                              .List();
//            trans.Rollback();
//            return data;
//        }
//    }
//}

//private static IList<Alloc> GetAllocationForEAlloc(BulkAllocationModel model)
//{
//    using (var session = SessionManager.GetNewSession())
//    {
//        using (var trans = session.BeginTransaction())
//        {
//            var data = session.QueryOver<Alloc>()
//                              .Fetch(x => x.AllocPolicy).Eager
//                              .Fetch(x => x.AllocSubpolicy).Eager
//                              //.Fetch(x => x.ELiner).Eager
//                              //.Fetch(x => x.EWriteoff).Eager
//                              .Fetch(x => x.Stakeholder).Eager
//                              .Where(x => x.Stakeholder.Id == model.Stakeholder)
//                              .List();
//            trans.Rollback();
//            return data;
//        }
//    }
//}