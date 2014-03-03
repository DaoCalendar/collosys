using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Script.Serialization;
using ColloSys.DataLayer.Allocation;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.UserInterface.Areas.Allocation.ViewModels;
using ColloSys.UserInterface.Shared.Attributes;
using NHibernate.Linq;
using Newtonsoft.Json.Linq;

namespace ColloSys.UserInterface.Areas.Allocation.apiController
{
    public class AllocationChangeController : ApiController
    {
        [HttpGet]
        [HttpTransaction]
        public IEnumerable<ScbEnums.Products> GetProducts()
        {
            using (var session = SessionManager.GetNewSession())
            {
                using (var trans = session.BeginTransaction())
                {
                    var data = session.QueryOver<ProductConfig>()
                                      .Select(x => x.Product)
                                      .List<ScbEnums.Products>();
                    trans.Rollback();
                    return data;
                }
            }
        }

        [HttpPost]
        [HttpTransaction]
        public List<AllocationChange> GetAllocations(DataModel2 model)
        {
            if (model.AllocDate == DateTime.MinValue)
            {
                model.AllocDate = DateTime.Now;
            }

            var allocListModel = new AllocationChange();
            var list = allocListModel.GetAllocations(model);
            //list.ForEach(x => x.Alloc.Stakeholder.MakeEmpty2(true));

            //foreach (AllocationChange allocationChange in list)
            //{
            //    allocationChange.Alloc.Stakeholder.MakeEmpty2(true);
            //}

            return list;
        }

        [HttpGet]
        [HttpTransaction]
        public Array GetChangeReasonList()
        {
            var values = Enum.GetValues(typeof(ColloSysEnums.ChangeAllocReason));
            return values;
        }

        [HttpPost]
        [HttpTransaction]
        public bool ChangeAllocationList(JObject jObject)
        {
            var dictionary = jObject.ToObject<Dictionary<string, object>>();

            var serializer = new JavaScriptSerializer();
            var objList = serializer.Deserialize<IEnumerable<object>>(dictionary["list"].ToString()).ToArray();
            var dataModel = serializer.Deserialize<DataModel2>(dictionary["Model2"].ToString());

            //foreach (IDictionary<string, object> obj in objList)
            //{
            //    var change = new AllocationChange();
            //    change.Alloc = serializer.Deserialize<SharedAlloc>(JObject.FromObject(obj["Alloc"]).ToString());
            //    change.Liner = serializer.Deserialize<RLiner>(JObject.FromObject(obj["Liner"]).ToString());
            //    change.Writeoff = serializer.Deserialize<RLiner>(JObject.FromObject(obj["Writeoff"]).ToString());
            //    change.NewStakeholderId = obj["NewStakeholderId"].ToString();
            //}

            using (var session = SessionManager.GetNewSession())
            {
                using (var trans = session.BeginTransaction())
                {
                    foreach (IDictionary<string, object> obj in objList)
                    {
                        //var test = obj["NewStakeholderId"].ToString();
                        //if (string.IsNullOrWhiteSpace(test))
                        //    continue;

                        switch (dataModel.Product)
                        {
                            case ScbEnums.Products.SME_BIL:
                            case ScbEnums.Products.SME_ME:
                            case ScbEnums.Products.SME_LAP:
                            case ScbEnums.Products.MORT:
                            case ScbEnums.Products.AUTO:
                            case ScbEnums.Products.PL:
                                var alloc = serializer.Deserialize<RAlloc>(JObject.FromObject(obj["Alloc"]).ToString());
                                if (string.IsNullOrWhiteSpace(alloc.ChangeReason))
                                    continue;
                                var liner = serializer.Deserialize<RLiner>(JObject.FromObject(obj["Liner"]).ToString());
                                var newStakeholderId = Guid.Parse(obj["NewStakeholderId"].ToString());

                                var oldAlloc = session.QueryOver<RAlloc>().Where(x => x.Id == alloc.Id).SingleOrDefault<RAlloc>();
                                oldAlloc.RLiner = liner;
                                oldAlloc.Stakeholder = session.QueryOver<Stakeholders>().Where(x => x.Id == newStakeholderId).SingleOrDefault<Stakeholders>();
                                oldAlloc.ChangeReason = alloc.ChangeReason;
                                session.SaveOrUpdate(oldAlloc);
                                break;
                            case ScbEnums.Products.CC:
                                //session.SaveOrUpdate((CAlloc)change.Alloc);
                                break;
                            case ScbEnums.Products.SME_LAP_OD:
                            case ScbEnums.Products.AUTO_OD:
                            case ScbEnums.Products.SMC:
                                //session.SaveOrUpdate((EAlloc)change.Alloc);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                    trans.Commit();
                }
            }

            //ChangeAllocationModel model = new ChangeAllocationModel();
            //var list = model.list;
            //var savelist = list.Where(x => x.Alloc.ChangeReason != null).ToList();
            //savelist.ForEach(x => x.Alloc.Status = ColloSysEnums.ApproveStatus.Submitted);
            //using (var session = SessionManager.GetNewSession())
            //{
            //    using (var trans = session.BeginTransaction())
            //    {
            //        foreach (var change in savelist)
            //        {
            //            switch (model.Model2.Product)
            //            {
            //                case ScbEnums.Products.SME_BIL:
            //                case ScbEnums.Products.SME_ME:
            //                case ScbEnums.Products.SME_LAP:
            //                case ScbEnums.Products.AUTO:
            //                case ScbEnums.Products.MORT:
            //                case ScbEnums.Products.PL:
            //                    //session.SaveOrUpdate((RAlloc)change.Alloc);
            //                    break;
            //                case ScbEnums.Products.CC:
            //                    //session.SaveOrUpdate((CAlloc)change.Alloc);
            //                    break;
            //                case ScbEnums.Products.SME_LAP_OD:
            //                case ScbEnums.Products.AUTO_OD:
            //                case ScbEnums.Products.SMC:
            //                    //session.SaveOrUpdate((EAlloc)change.Alloc);
            //                    break;
            //                default:
            //                    throw new ArgumentOutOfRangeException();
            //            }
            //        }
            //        trans.Commit();
            //    }
            //}
            return true;
        }

        [HttpGet]
        [HttpTransaction]
        public IList<Stakeholders> GetStakeholders()
        {
            using (var session = SessionManager.GetNewSession())
            {
                using (var trans = session.BeginTransaction())
                {
                    var data = session.Query<Stakeholders>()
                                      .FetchMany(x => x.StkhWorkings)
                                      .ThenFetch(x => x.GPincode).ToList();
                    trans.Rollback();
                    return data;
                }
            }
        }
    }
}
