#region references

using System;
using System.Collections.Generic;
using ColloSys.DataLayer.Allocation;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.SharedDomain;
using NHibernate.SqlCommand;

#endregion

namespace ColloSys.UserInterface.Areas.Allocation.ViewModels
{
    public class AllocListModel
    {
        public AllocListModel()
        {
            Properties = new List<string>();
        }

        public List<string> Properties { get; set; }
        public SharedAlloc Alloc { get; set; }
        public ISoftDelq Liner { get; set; }
        public IDelinquentCustomer Writeoff { get; set; }
        public Stakeholders Stakeholder { get; set; }
        public string AllocationFor { get; set; }

        public List<AllocListModel> GetAllocations(DataModel model)
        {
            var obj = new List<AllocListModel>();
            switch (model.Product)
            {
                case ScbEnums.Products.SME_BIL:
                case ScbEnums.Products.SME_ME:
                case ScbEnums.Products.SME_LAP:
                case ScbEnums.Products.MORT:
                case ScbEnums.Products.AUTO:
                case ScbEnums.Products.PL:
                    obj = ConvertRAlloc(AllocationsListRAlloc(model));
                    break;

                case ScbEnums.Products.SME_LAP_OD:
                case ScbEnums.Products.AUTO_OD:
                case ScbEnums.Products.SMC:
                    obj = ConvertEAlloc(AllocationsListEAlloc(model));
                    break;
                
                case ScbEnums.Products.CC:
                    obj = ConvertCAlloc(AllocationsListCAlloc(model));
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return obj;
        }

        private IList<SharedAlloc> AllocationsListEAlloc(DataModel model)
        {
            using (var session = SessionManager.GetNewSession())
            {
                using (var trans = session.BeginTransaction())
                {
                    ELiner liner = null;
                    EAlloc alloc = null;
                    EWriteoff writeoff = null;
                    IList<SharedAlloc> data =new List<SharedAlloc>();
                    if (model.Category == ScbEnums.Category.Liner)
                    {
                        if(model.StaekholderId==Guid.Empty)
                         data = session.QueryOver(()=>alloc)
                                          .Fetch(x => x.Stakeholder).Eager
                                          .Fetch(x => x.ELiner).Eager
                                          .Fetch(x => x.EWriteoff).Eager
                                          .JoinAlias(()=>alloc.ELiner,()=>liner,JoinType.LeftOuterJoin)
                                          .JoinAlias(()=>alloc.EWriteoff,()=>writeoff,JoinType.LeftOuterJoin)
                                          .Where(()=>liner.Product==model.Product)
                                          .And(()=>alloc.CreatedOn > model.AllocDate.Date)
                                          .And(()=>alloc.CreatedOn < model.AllocDate.AddDays(1).AddSeconds(-1))
                                          //.And(()=>(model.StaekholderId!=Guid.Empty)&& alloc.Stakeholder.Id!=null && alloc.Stakeholder.Id==model.StaekholderId)
                                          //.And(() => alloc.StartDate > model.StartDate)
                                          //.And(() => alloc.EndDate < model.EndDate)
                                          .OrderBy(() => alloc.AmountDue).Asc
                                          .List<SharedAlloc>();
                        if(model.StaekholderId!=Guid.Empty)
                            data = session.QueryOver(() => alloc)
                                         .Fetch(x => x.Stakeholder).Eager
                                         .Fetch(x => x.ELiner).Eager
                                         .Fetch(x => x.EWriteoff).Eager
                                         .JoinAlias(() => alloc.ELiner, () => liner, JoinType.LeftOuterJoin)
                                         .JoinAlias(() => alloc.EWriteoff, () => writeoff, JoinType.LeftOuterJoin)
                                         .Where(() => liner.Product == model.Product)
                                         .And(() => alloc.Stakeholder.Id != null && alloc.Stakeholder.Id == model.StaekholderId)
                                //.And(() => alloc.StartDate > model.StartDate)
                                //.And(() => alloc.EndDate < model.EndDate)
                                         .OrderBy(() => alloc.AmountDue).Asc
                                         .List<SharedAlloc>();
                        trans.Rollback();
                    }
                    else
                    {
                        if(model.StaekholderId==Guid.Empty)
                         data = session.QueryOver(() => alloc)
                                          .Fetch(x => x.Stakeholder).Eager
                                          .Fetch(x => x.ELiner).Eager
                                          .Fetch(x => x.EWriteoff).Eager
                                          .JoinAlias(() => alloc.ELiner, () => liner, JoinType.LeftOuterJoin)
                                          .JoinAlias(() => alloc.EWriteoff, () => writeoff, JoinType.LeftOuterJoin)
                                          .Where(() => writeoff.Product == model.Product)
                                          //.And(() => alloc.StartDate > model.StartDate)
                                          //.And(() => alloc.EndDate < model.EndDate)
                                          .OrderBy(() => alloc.AmountDue).Asc
                                          .List<SharedAlloc>();
                        if(model.StaekholderId!=Guid.Empty)
                            data = session.QueryOver(() => alloc)
                                         .Fetch(x => x.Stakeholder).Eager
                                         .Fetch(x => x.ELiner).Eager
                                         .Fetch(x => x.EWriteoff).Eager
                                         .JoinAlias(() => alloc.ELiner, () => liner, JoinType.LeftOuterJoin)
                                         .JoinAlias(() => alloc.EWriteoff, () => writeoff, JoinType.LeftOuterJoin)
                                         .Where(() => writeoff.Product == model.Product)
                                         .And(() => alloc.Stakeholder.Id != null && alloc.Stakeholder.Id == model.StaekholderId)
                                         .OrderBy(() => alloc.AmountDue).Asc
                                         .List<SharedAlloc>();
                        trans.Rollback();
                        return data;
                    }
                    return data;
                }
            }
        }

        private IList<SharedAlloc> AllocationsListCAlloc(DataModel model)
        {
            using (var session = SessionManager.GetNewSession())
            {
                using (var trans = session.BeginTransaction())
                {
                   CLiner liner = null;
                    CAlloc alloc = null;
                    CWriteoff writeoff = null;
                    IList<SharedAlloc> data = new List<SharedAlloc>();
                    if (model.Category == ScbEnums.Category.Liner)
                    {
                        if (model.StaekholderId == Guid.Empty)
                            data = session.QueryOver(() => alloc)
                                             .Fetch(x => x.Stakeholder).Eager
                                             .Fetch(x => x.CLiner).Eager
                                             .Fetch(x => x.CWriteoff).Eager
                                             .JoinAlias(() => alloc.CLiner, () => liner, JoinType.LeftOuterJoin)
                                             .JoinAlias(() => alloc.CWriteoff, () => writeoff, JoinType.LeftOuterJoin)
                                             .Where(() => liner.Product == model.Product)
                                             .And(() => alloc.CreatedOn > model.AllocDate.Date)
                                             .And(() => alloc.CreatedOn < model.AllocDate.AddDays(1).AddSeconds(-1))
                                //.And(()=>(model.StaekholderId!=Guid.Empty)&& alloc.Stakeholder.Id!=null && alloc.Stakeholder.Id==model.StaekholderId)
                                //.And(() => alloc.StartDate > model.StartDate)
                                //.And(() => alloc.EndDate < model.EndDate)
                                             .OrderBy(() => alloc.AmountDue).Asc
                                             .List<SharedAlloc>();
                        if (model.StaekholderId != Guid.Empty)
                            data = session.QueryOver(() => alloc)
                                         .Fetch(x => x.Stakeholder).Eager
                                         .Fetch(x => x.CLiner).Eager
                                         .Fetch(x => x.CWriteoff).Eager
                                         .JoinAlias(() => alloc.CLiner, () => liner, JoinType.LeftOuterJoin)
                                         .JoinAlias(() => alloc.CWriteoff, () => writeoff, JoinType.LeftOuterJoin)
                                         .Where(() => liner.Product == model.Product)
                                         .And(() => alloc.Stakeholder.Id != null && alloc.Stakeholder.Id == model.StaekholderId)
                                //.And(() => alloc.StartDate > model.StartDate)
                                //.And(() => alloc.EndDate < model.EndDate)
                                         .OrderBy(() => alloc.AmountDue).Asc
                                         .List<SharedAlloc>();
                        trans.Rollback();
                    }
                    else
                    {
                        if (model.StaekholderId == Guid.Empty)
                            data = session.QueryOver(() => alloc)
                                             .Fetch(x => x.Stakeholder).Eager
                                             .Fetch(x => x.CLiner).Eager
                                             .Fetch(x => x.CWriteoff).Eager
                                             .JoinAlias(() => alloc.CLiner, () => liner, JoinType.LeftOuterJoin)
                                             .JoinAlias(() => alloc.CWriteoff, () => writeoff, JoinType.LeftOuterJoin)
                                             .Where(() => writeoff.Product == model.Product)
                                //.And(() => alloc.StartDate > model.StartDate)
                                //.And(() => alloc.EndDate < model.EndDate)
                                             .OrderBy(() => alloc.AmountDue).Asc
                                             .List<SharedAlloc>();
                        if (model.StaekholderId != Guid.Empty)
                            data = session.QueryOver(() => alloc)
                                         .Fetch(x => x.Stakeholder).Eager
                                         .Fetch(x => x.CLiner).Eager
                                         .Fetch(x => x.CWriteoff).Eager
                                         .JoinAlias(() => alloc.CLiner, () => liner, JoinType.LeftOuterJoin)
                                         .JoinAlias(() => alloc.CWriteoff, () => writeoff, JoinType.LeftOuterJoin)
                                         .Where(() => writeoff.Product == model.Product)
                                         .And(() => alloc.Stakeholder.Id != null && alloc.Stakeholder.Id == model.StaekholderId)
                                         .OrderBy(() => alloc.AmountDue).Asc
                                         .List<SharedAlloc>();
                        trans.Rollback();
                        return data;
                    }
                    return data;
                }
            }
        }

        private IList<SharedAlloc> AllocationsListRAlloc(DataModel model)
        {
            using (var session = SessionManager.GetNewSession())
            {
                using (var trans = session.BeginTransaction())
                {
                    RLiner liner = null;
                    RAlloc alloc = null;
                    RWriteoff writeoff = null;
                    IList<SharedAlloc> data = new List<SharedAlloc>();
                    if (model.Category == ScbEnums.Category.Liner)
                    {
                        if (model.StaekholderId == Guid.Empty)
                            data = session.QueryOver(() => alloc)
                                             .Fetch(x => x.Stakeholder).Eager
                                             .Fetch(x => x.RLiner).Eager
                                             .Fetch(x => x.RWriteoff).Eager
                                             .JoinAlias(() => alloc.RLiner, () => liner, JoinType.LeftOuterJoin)
                                             .JoinAlias(() => alloc.RWriteoff, () => writeoff, JoinType.LeftOuterJoin)
                                             .Where(() => liner.Product == model.Product)
                                             .And(() => alloc.CreatedOn > model.AllocDate.Date)
                                             .And(() => alloc.CreatedOn < model.AllocDate.AddDays(1).AddSeconds(-1))
                                //.And(()=>(model.StaekholderId!=Guid.Empty)&& alloc.Stakeholder.Id!=null && alloc.Stakeholder.Id==model.StaekholderId)
                                //.And(() => alloc.StartDate > model.StartDate)
                                //.And(() => alloc.EndDate < model.EndDate)
                                             .OrderBy(() => alloc.AmountDue).Asc
                                             .List<SharedAlloc>();
                        if (model.StaekholderId != Guid.Empty)
                            data = session.QueryOver(() => alloc)
                                         .Fetch(x => x.Stakeholder).Eager
                                         .Fetch(x => x.RLiner).Eager
                                         .Fetch(x => x.RWriteoff).Eager
                                         .JoinAlias(() => alloc.RLiner, () => liner, JoinType.LeftOuterJoin)
                                         .JoinAlias(() => alloc.RWriteoff, () => writeoff, JoinType.LeftOuterJoin)
                                         .Where(() => liner.Product == model.Product)
                                         .And(() => alloc.Stakeholder.Id != null && alloc.Stakeholder.Id == model.StaekholderId)
                                //.And(() => alloc.StartDate > model.StartDate)
                                //.And(() => alloc.EndDate < model.EndDate)
                                         .OrderBy(() => alloc.AmountDue).Asc
                                         .List<SharedAlloc>();
                        trans.Rollback();
                    }
                    else
                    {
                        if (model.StaekholderId == Guid.Empty)
                            data = session.QueryOver(() => alloc)
                                             .Fetch(x => x.Stakeholder).Eager
                                             .Fetch(x => x.RLiner).Eager
                                             .Fetch(x => x.RWriteoff).Eager
                                             .JoinAlias(() => alloc.RLiner, () => liner, JoinType.LeftOuterJoin)
                                             .JoinAlias(() => alloc.RWriteoff, () => writeoff, JoinType.LeftOuterJoin)
                                             .Where(() => writeoff.Product == model.Product)
                                //.And(() => alloc.StartDate > model.StartDate)
                                //.And(() => alloc.EndDate < model.EndDate)
                                             .OrderBy(() => alloc.AmountDue).Asc
                                             .List<SharedAlloc>();
                        if (model.StaekholderId != Guid.Empty)
                            data = session.QueryOver(() => alloc)
                                         .Fetch(x => x.Stakeholder).Eager
                                         .Fetch(x => x.RLiner).Eager
                                         .Fetch(x => x.RWriteoff).Eager
                                         .JoinAlias(() => alloc.RLiner, () => liner, JoinType.LeftOuterJoin)
                                         .JoinAlias(() => alloc.RWriteoff, () => writeoff, JoinType.LeftOuterJoin)
                                         .Where(() => writeoff.Product == model.Product)
                                         .And(() => alloc.Stakeholder.Id != null && alloc.Stakeholder.Id == model.StaekholderId)
                                         .OrderBy(() => alloc.AmountDue).Asc
                                         .List<SharedAlloc>();
                        trans.Rollback();
                        return data;
                    }
                    return data;
                }
            }
        }

        private List<AllocListModel> ConvertEAlloc(IList<SharedAlloc> list)
        {
            var list2 = new List<AllocListModel>();

            foreach (var sharedAlloc in list)
            {
                var temp = new AllocListModel();
                temp.Alloc = sharedAlloc;
                temp.Stakeholder = sharedAlloc.Stakeholder;
                temp.Liner = DeepClone(((EAlloc)sharedAlloc).ELiner);
                temp.Writeoff = DeepClone(((EAlloc)sharedAlloc).EWriteoff);
                list2.Add(temp);
            }

            return list2;
        }


        private List<AllocListModel> ConvertRAlloc(IList<SharedAlloc> list)
        {
            var list2 = new List<AllocListModel>();

            foreach (var sharedAlloc in list)
            {
                var temp = new AllocListModel();
                temp.Alloc = sharedAlloc;
                temp.Stakeholder = sharedAlloc.Stakeholder;
                temp.Liner = DeepClone(((RAlloc)sharedAlloc).RLiner);
                temp.Writeoff = DeepClone(((RAlloc)sharedAlloc).RWriteoff);
                list2.Add(temp);
            }

            return list2;
        }
        private List<AllocListModel> ConvertCAlloc(IList<SharedAlloc> list)
        {
            var list2 = new List<AllocListModel>();

            foreach (var sharedAlloc in list)
            {
                var temp = new AllocListModel();
                temp.Alloc = sharedAlloc;
                temp.Stakeholder = sharedAlloc.Stakeholder;
                temp.Liner = DeepClone(((CAlloc)sharedAlloc).CLiner);
                temp.Writeoff = DeepClone(((CAlloc)sharedAlloc).CWriteoff);
                list2.Add(temp);
            }

            return list2;
        }

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

        //public static T DeepClone<T>(T obj)
        //{
        //    using (var ms = new MemoryStream())
        //    {
        //        var formatter = new BinaryFormatter();
        //        formatter.Serialize(ms, obj);
        //        ms.Position = 0;
        //        return (T)formatter.Deserialize(ms);
        //    }
        //}
    }

}