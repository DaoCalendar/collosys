using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AngularUI.Shared.apis;
using AngularUI.Stakeholder.addedit.Working;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Stakeholder;
using ColloSys.QueryBuilder.GenericBuilder;
using ColloSys.QueryBuilder.StakeholderBuilder;
using NHibernate.Linq;

namespace AngularUI.Stakeholder.AddEdit2.Working
{
    public class WorkingApiController : BaseApiController<StkhWorking>
    {
        private static readonly GKeyValueBuilder GKeyValueBuilder = new GKeyValueBuilder();
        private static readonly StakePaymentQueryBuilder StakePaymentBuilder = new StakePaymentQueryBuilder();

        [HttpGet]
        public HttpResponseMessage GetStakeWorkingData(Guid stakeholderId)
        {
            var stkh = Session.Query<Stakeholders>()
                .Where(x => x.Id == stakeholderId)
                .Fetch(x => x.Hierarchy)
                .Fetch(x => x.StkhPayments)
                .Fetch(x => x.StkhWorkings)
                .Single();
            return Request.CreateResponse(HttpStatusCode.OK, stkh);
        }

        //TODO: rename
        [HttpGet]
        public HttpResponseMessage GetStakePaymentData()
        {
            var gKeyValue = GKeyValueBuilder.ForStakeholders();
            return Request.CreateResponse(HttpStatusCode.OK, gKeyValue);
        }

        [HttpPost]
        public HttpResponseMessage GetPincodeData(WorkingModel workingModel)
        {
            workingModel.SetWorkingList(workingModel);
            return Request.CreateResponse(HttpStatusCode.OK, workingModel);
        }


        [HttpPost]
        public void SavePayment(StkhPayment paymentData)
        {
            paymentData.StartDate = paymentData.Stakeholder.JoiningDate;
            paymentData.Stakeholder = Session.Load<Stakeholders>(paymentData.Stakeholder.Id);

            StakePaymentBuilder.Save(paymentData);
        }

        [HttpGet]
        public HttpResponseMessage GetWorkingReportsTo(Guid id, ColloSysEnums.ReportingLevel level)
        {
            var data = WorkingPaymentHelper.GetReportsOnreportingLevel(id,level);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }
    }
}