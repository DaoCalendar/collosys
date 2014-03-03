using System;
using System.Collections.Generic;
using System.Web.Http;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.UserInterface.Areas.Allocation.ViewModels;
using ColloSys.UserInterface.Shared.Attributes;
using NHibernate.SqlCommand;
using NHibernate.Transform;

namespace ColloSys.UserInterface.Areas.Allocation.apiController
{
    public class AllocationsListController : ApiController
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
        public IEnumerable<AllocListModel> GetAllocations(DataModel model)
        {
            if (model.AllocDate == DateTime.MinValue)
            {
                model.AllocDate = DateTime.Now;
            }
            //if (model.StartDate == DateTime.MinValue)
            //{
            //    model.StartDate = DateTime.Now.AddYears(-1);
            //}
            //if (model.EndDate == DateTime.MinValue)
            //{
            //    model.EndDate = DateTime.Now.AddYears(1);
            //}
            var allocListModel = new AllocListModel();
            var list = allocListModel.GetAllocations(model);
            return list;
        }

        [HttpGet]
        [HttpTransaction]
        public IEnumerable<Stakeholders> GetStakeholders()
        {
            Stakeholders stakeholders = null;
            StkhWorking workings = null;
            StkhHierarchy hierarchy = null;
            using (var session = SessionManager.GetNewSession())
            {
                using (var trans = session.BeginTransaction())
                {
                    var data = session.QueryOver(() => stakeholders)
                                      .Fetch(x => x.StkhWorkings).Eager
                                      .JoinAlias(() => stakeholders.StkhWorkings, () => workings, JoinType.LeftOuterJoin)
                                      .JoinAlias(() => stakeholders.Hierarchy, () => hierarchy, JoinType.LeftOuterJoin)
                                      //.Where(() => hierarchy.IsInAllocation)
                                      .Where(() => stakeholders.Status == ColloSysEnums.ApproveStatus.Approved)
                                      .And(()=>hierarchy.IsInAllocation)
                                      //.And(() => stakeholders.JoiningDate < Util.GetTodayDate())
                                      //.And(() => stakeholders.LeavingDate == null ||
                                      //           stakeholders.LeavingDate > Util.GetTodayDate())
                                      //.And(() => stakeholders.Status == ColloSysEnums.ApproveStatus.Approved)
                                      .TransformUsing(Transformers.DistinctRootEntity)
                                      .List();
                    trans.Rollback();
                    return data;
                }
            }
        }
    }
}
