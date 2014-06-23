using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AngularUI.Shared.apis;
using ColloSys.DataLayer.Stakeholder;
using ColloSys.QueryBuilder.StakeholderBuilder;
using StkhLeaveRepository = ColloSys.QueryBuilder.GenericBuilder.StkhLeaveRepository;

namespace AngularUI.Generic.stkhleave
{
    public class StkhLeaveApiController : BaseApiController<StkhLeave>
    {
        private static readonly StkhLeaveRepository StaRepository = new StkhLeaveRepository();

        [HttpGet]
        public HttpResponseMessage GetLeavesHistory()
        {
            var stkhRepo = new StakeQueryBuilder();
            var stkh = stkhRepo.GetStakeByExtId(GetUsername());
            var data = StaRepository.FetchLeaves(stkh.Id);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpGet]
        public HttpResponseMessage GetDeligateToStkh()
        {
            var stkhRepo = new StakeQueryBuilder();
            var stkh = stkhRepo.GetStakeByExtId(GetUsername());
            var data = StaRepository.DelegateToStakeholdersList(stkh.Id);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        //[HttpPost]
        //public HttpResponseMessage SaveLeave(LeaveRequest request)
        //{
        //    var stkhRepo = new StakeQueryBuilder();
        //    var stkh = stkhRepo.GetStakeByExtId(GetUsername());
        //    var leave = new StkhLeave
        //    {
        //        Stakeholder = stkh,
        //        ToDate = request.ToDate,
        //        FromDate = request.FromDate,
        //        DelegatedTo = Session.Load<Stakeholders>(request.DelegateTo)
        //    };
        //    StaRepository.Save(leave);
        //    return Request.CreateResponse(HttpStatusCode.OK, leave);
        //}

        [HttpPost]
        public HttpResponseMessage SaveLeave2(StkhLeave leave)
        {
            var stkhRepo = new StakeQueryBuilder();
            var stkh = stkhRepo.GetStakeByExtId(GetUsername());
            leave.Stakeholder = stkh;
            StaRepository.Save(leave);
            return Request.CreateResponse(HttpStatusCode.OK, leave);
        }

        [HttpPost]
        public HttpResponseMessage Deleteleave(StkhLeave stkh)
        {
            StaRepository.Delete(stkh);
            return Request.CreateResponse(HttpStatusCode.OK, stkh);
        }
        
    }

    //public class LeaveRequest
    //{
    //    public DateTime FromDate;
    //    public DateTime ToDate;
    //    public Guid DelegateTo;
    //}

   
}
