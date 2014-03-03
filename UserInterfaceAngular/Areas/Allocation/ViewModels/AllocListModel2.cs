#region references

using System;
using System.Collections;
using System.Collections.Generic;
using ColloSys.DataLayer.Allocation;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.SharedDomain;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;

#endregion

namespace ColloSys.UserInterface.Areas.Allocation.ViewModels
{
    public class AllocListModel2
    {
        public AllocListModel2()
        {
            Properties = new List<string>();
        }

        public List<string> Properties { get; set; }
        public SharedAlloc Alloc { get; set; }
        public ISoftDelq Liner { get; set; }
        public IDelinquentCustomer Writeoff { get; set; }
        public Stakeholders Stakeholder { get; set; }

        public List<AllocListModel2> GetAllocations(DataModel2 model)
        {
            var obj = new List<AllocListModel2>();
            switch (model.Product)
            {
                case ScbEnums.Products.SME_BIL:
                case ScbEnums.Products.SME_LAP:
                case ScbEnums.Products.SME_ME:
                case ScbEnums.Products.MORT:
                case ScbEnums.Products.AUTO:
                case ScbEnums.Products.PL:
                   // obj = ConvertRAlloc(AllocationsListRAlloc(model));
                    break;

                case ScbEnums.Products.SME_LAP_OD:
                case ScbEnums.Products.AUTO_OD:
                case ScbEnums.Products.SMC:
                    obj = ConvertEAlloc(AllocationsListEAlloc(model));
                    break;

                case ScbEnums.Products.CC:
                    //obj = ConvertCAlloc(AllocationsListCAlloc(model));
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
            return obj;
        }

        private IList AllocationsListEAlloc(DataModel2 model)
        {
            ICriteria criteria = null;
            using (var session = SessionManager.GetNewSession())
            {
                using (var trans = session.BeginTransaction())
                {
                    criteria = session.CreateCriteria<EAlloc>("Alloc");
                    

                    if (model.Category == ScbEnums.Category.Liner)
                    {
                        criteria.CreateCriteria("ELiner", "EObj", JoinType.InnerJoin)
                                .Add(Restrictions.Eq("EObj.Product", model.Product));
                    }

                    else
                    {
                        criteria.CreateCriteria("EWriteoff", "EObj", JoinType.InnerJoin).Add(Restrictions.Eq("EObj.Product", model.Product));
                    }

                    if (model.SelecAllocType != ColloSysEnums.AllocStatus.None)
                    {
                        criteria.Add(Restrictions.Eq("EObj.AllocStatus", model.SelecAllocType));
                    }

                    //criteria.Add(Restrictions.Gt("Alloc.CreatedOn", model.AllocDate))
                    //        .Add(Restrictions.Lt("Alloc.CreatedOn", model.AllocDate.AddDays(1).AddSeconds(-1)));

                    var list = criteria.List();

                    trans.Rollback();

                    return list;
                }
            }
        }

        private List<AllocListModel2> ConvertEAlloc(IList list)
        {
            var list2 = new List<AllocListModel2>();

            foreach (var sharedAlloc in list)
            {
                var temp = new AllocListModel2();
                temp.Alloc =(SharedAlloc) sharedAlloc;
                temp.Stakeholder = ((SharedAlloc)sharedAlloc).Stakeholder;
                temp.Liner = DeepClone(((EAlloc)sharedAlloc).ELiner);
                temp.Writeoff = DeepClone(((EAlloc)sharedAlloc).EWriteoff);
                list2.Add(temp);
            }
            return list2;
        }

        private List<AllocListModel2> ConvertRAlloc(IList<SharedAlloc> list)
        {
            var list2 = new List<AllocListModel2>();

            foreach (var sharedAlloc in list)
            {
                var temp = new AllocListModel2();
                temp.Alloc = sharedAlloc;
                temp.Stakeholder = sharedAlloc.Stakeholder;
                temp.Liner = DeepClone(((RAlloc)sharedAlloc).RLiner);
                temp.Writeoff = DeepClone(((RAlloc)sharedAlloc).RWriteoff);
                list2.Add(temp);
            }
            return list2;
        }
        private List<AllocListModel2> ConvertCAlloc(IList<SharedAlloc> list)
        {
            var list2 = new List<AllocListModel2>();

            foreach (var sharedAlloc in list)
            {
                var temp = new AllocListModel2();
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
    }
}