using System;
using System.Collections.Generic;
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
        private static readonly StakeWorkingQueryBuilder StakeWorkingQueryBuilder = new StakeWorkingQueryBuilder();

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

        [HttpPost]
        public void SaveWorking(List<StkhWorking> workingData)
        {
            StakeWorkingQueryBuilder.Save(workingData);
        }

        [HttpPost]
        public HttpResponseMessage GetGPincodeData(WorkingModel workingModel)
        {
            workingModel.GetGPincodeData(workingModel);
            return Request.CreateResponse(HttpStatusCode.OK, workingModel);
        }

        public HttpResponseMessage GetSalaryDetails(StkhPayment payment)
        {
            var gKeyValue = GKeyValueBuilder.ForStakeholders();
            var fixPay = gKeyValue.ToDictionary(keyValue => keyValue.ParamName, keyValue => decimal.Parse(keyValue.Value));
            return Request.CreateResponse(HttpStatusCode.OK, WorkingPaymentHelper.GetSalaryDetails(payment, fixPay));
        }

        [HttpGet]
        public HttpResponseMessage GetWorkingReportsTo(Guid id, ColloSysEnums.ReportingLevel level)
        {
            var data = WorkingPaymentHelper.GetReportsOnreportingLevel(id, level);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }
    }
}