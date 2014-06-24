#region references

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AngularUI.Shared.apis;
using AngularUI.Stakeholder.addedit.BasicInfo;
using AngularUI.Stakeholder.view;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Stakeholder;
using ColloSys.QueryBuilder.GenericBuilder;
using ColloSys.QueryBuilder.StakeholderBuilder;
using NHibernate.Linq;
using NHibernate.Mapping;

#endregion

namespace AngularUI.Stakeholder.addedit.Working
{
    public class WorkingApiController : BaseApiController<StkhWorking>
    {
        private static readonly StakePaymentQueryBuilder StakePaymentBuilder = new StakePaymentQueryBuilder();
        private static readonly StakeWorkingQueryBuilder StakeWorkingQueryBuilder = new StakeWorkingQueryBuilder();
        private static readonly StakeQueryBuilder StakeQueryBuilder = new StakeQueryBuilder();

        [HttpGet]
        public HttpResponseMessage GetStakeholder(Guid stakeholderId)
        {
            var stkh = Session.Query<Stakeholders>()
                .Where(x => x.Id == stakeholderId)
                .Fetch(x => x.Hierarchy)
                .Fetch(x => x.StkhPayments)
                .Fetch(x => x.StkhWorkings)
                .Single();

            var reportsToIds = stkh.StkhWorkings.Select(stkhWorking => stkhWorking.ReportsTo).ToList();
            var reportsToStakeholders = StakeQueryBuilder.GetByIdList(reportsToIds);

            var data = new
                {
                    Stakeholder = stkh,
                    ReportsToStakes = reportsToStakeholders
                };
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpPost]
        public HttpResponseMessage GetPincodeData(WorkingModel workingModel)
        {
            workingModel.SetWorkingList(workingModel);
            return Request.CreateResponse(HttpStatusCode.OK, workingModel);
        }

        [HttpPost]
        public HttpResponseMessage GetGPincodeData(WorkingModel workingModel)
        {
            workingModel.GetGPincodeData(workingModel);
            return Request.CreateResponse(HttpStatusCode.OK, workingModel);
        }

        [HttpGet]
        public HttpResponseMessage GetWorkingReportsTo(Guid id, ColloSysEnums.ReportingLevel level, ScbEnums.Products product)
        {
            var data = WorkingPaymentHelper.GetStkhWorkingByReportingLevel(id, level, product);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpPost]
        public void SaveWorking(IEnumerable<StkhWorking> workingData)
        {
            StakeWorkingQueryBuilder.Save(workingData);
        }

        [HttpPost]
        public HttpResponseMessage DeleteWorking(IEnumerable<StkhWorking> deleteList)
        {
            var stkhWorkings = deleteList as IList<StkhWorking> ?? deleteList.ToList();
            foreach (var stkhWorking in stkhWorkings.Where(stkhWorking => stkhWorking.Id != Guid.Empty))
            {
                if (stkhWorking.Status == ColloSysEnums.ApproveStatus.Approved || stkhWorking.Status == ColloSysEnums.ApproveStatus.Changed)
                {
                    StakeWorkingQueryBuilder.Save(stkhWorking);
                }
                else
                {
                    StakeWorkingQueryBuilder.Delete(stkhWorking);

                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, stkhWorkings);
        }

        [HttpPost]
        public HttpResponseMessage GetSalaryDetails(PaymentIds paymentId)
        {
            var gKeyValueBuilder = new GKeyValueBuilder();
            var gKeyValue = gKeyValueBuilder.ForStakeholders();
            var fixPay = gKeyValue.ToDictionary(keyValue => keyValue.ParamName, keyValue => decimal.Parse(keyValue.Value));
            var result = WorkingPaymentHelper.GetSalaryDetails(paymentId, fixPay);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpPost]
        public HttpResponseMessage SavePayment(StkhPayment paymentData)
        {
            paymentData.StartDate = paymentData.Stakeholder.JoiningDate;
            paymentData.Stakeholder = Session.Load<Stakeholders>(paymentData.Stakeholder.Id);
            StakePaymentBuilder.Save(paymentData);

            return Request.CreateResponse(HttpStatusCode.OK, paymentData);
        }

        [HttpPost]
        public HttpResponseMessage ApproveStakeholder(StkhId stakeholder)
        {
            var stkh = StakeQueryBuilder.GetStakeWithWorkings(stakeholder.Id);

            switch (stakeholder.SetStatusFor)
            {
                case "working":
                    foreach (var stkhWorking in stkh.StkhWorkings
                .Where(stkhWorking => stkhWorking.Status == ColloSysEnums.ApproveStatus.Submitted))
                    {
                        stkhWorking.Status = ColloSysEnums.ApproveStatus.Approved;
                    }

                    StakeWorkingQueryBuilder.Save(stkh.StkhWorkings);

                    return Request.CreateResponse(HttpStatusCode.OK,
                        stkh.StkhWorkings);
                case "payment":
                    if (stkh.StkhPayments[0] != null && stkh.StkhPayments[0].ApprovalStatus == ColloSysEnums.ApproveStatus.Submitted)
                    {
                        stkh.StkhPayments[0].ApprovalStatus = ColloSysEnums.ApproveStatus.Approved;
                    }
                    StakePaymentBuilder.Save(stkh.StkhPayments[0]);
                    return Request.CreateResponse(HttpStatusCode.OK,
                        stkh.StkhPayments[0]);
                default:
                    throw new Exception();
            }

        }

        [HttpPost]
        public HttpResponseMessage RejectStakeholder(StkhId stakeholder)
        {
            var stkh = StakeQueryBuilder.OnId(stakeholder.Id);
            foreach (var stkhWorking in stkh.StkhWorkings)
            {
                stkhWorking.Status = ColloSysEnums.ApproveStatus.Rejected;
            }

            StakeWorkingQueryBuilder.Save(stkh.StkhWorkings);

            return Request.CreateResponse(HttpStatusCode.OK,
                stkh.StkhWorkings);
        }
    }

    public class PaymentIds
    {
        public Guid ReportingId { get; set; }
        public Guid? PaymentId { get; set; }
    }
}
