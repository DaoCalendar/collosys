#region references

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AngularUI.Shared.apis;
using AngularUI.Stakeholder.view;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Stakeholder;
using ColloSys.QueryBuilder.GenericBuilder;
using ColloSys.QueryBuilder.StakeholderBuilder;
using NHibernate.Linq;

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
        public HttpResponseMessage GetWorkingReportsTo(Guid id, ColloSysEnums.ReportingLevel level,
                                                       ScbEnums.Products product)
        {
            var data = WorkingPaymentHelper.GetStkhWorkingByReportingLevel(id, level, product);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpPost]
        public HttpResponseMessage SaveWorking(IEnumerable<StkhWorking> workingData)
        {
            var listOfObjects = workingData as IList<StkhWorking> ?? workingData.ToList();
            StakeWorkingQueryBuilder.Save(listOfObjects);
            var reportsToIds = listOfObjects.Select(stkhWorking => stkhWorking.ReportsTo).ToList();
            var reportsToStakeholders = StakeQueryBuilder.GetByIdList(reportsToIds);

            var data = new
                {
                    WorkList = listOfObjects,
                    ReportsToList = reportsToStakeholders
                };

            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpPost]
        public HttpResponseMessage DeleteWorking(IEnumerable<StkhWorking> deleteList)
        {
            var stkhWorkings = deleteList as IList<StkhWorking> ?? deleteList.ToList();
            foreach (var stkhWorking in stkhWorkings.Where(stkhWorking => stkhWorking.Id != Guid.Empty))
            {
                if (stkhWorking.Status == ColloSysEnums.ApproveStatus.Approved ||
                    stkhWorking.Status == ColloSysEnums.ApproveStatus.Changed)
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
            var fixPay = gKeyValue.ToDictionary(keyValue => keyValue.ParamName,
                                                keyValue => decimal.Parse(keyValue.Value));
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
        public HttpResponseMessage ApproveWorking(StkhId stakeholder)
        {
            var stkh = StakeQueryBuilder.GetStakeWorkingPayment(stakeholder.Id);

            foreach (var stkhWorking in stkh.StkhWorkings
                                            .Where(
                                                stkhWorking =>
                                                stkhWorking.Status == ColloSysEnums.ApproveStatus.Submitted))
            {
                stkhWorking.Status = ColloSysEnums.ApproveStatus.Approved;
            }

            StakeWorkingQueryBuilder.Save(stkh.StkhWorkings);
            var reportsToIds = stkh.StkhWorkings.Select(stkhWorking => stkhWorking.ReportsTo).ToList();
            var reportsToStakeholders = StakeQueryBuilder.GetByIdList(reportsToIds);

            var data = new
                {
                    WorkList = stkh.StkhWorkings,
                    ReportsToList = reportsToStakeholders
                };

            return Request.CreateResponse(HttpStatusCode.OK,
                                          data);
        }

        [HttpPost]
        public HttpResponseMessage ApprovePayment(StkhId stakeholder)
        {
            var stkh = StakeQueryBuilder.GetStakeWorkingPayment(stakeholder.Id);
            if (stkh.StkhPayments[0] != null &&
                stkh.StkhPayments[0].ApprovalStatus == ColloSysEnums.ApproveStatus.Submitted)
            {
                stkh.StkhPayments[0].ApprovalStatus = ColloSysEnums.ApproveStatus.Approved;
            }
            StakePaymentBuilder.Save(stkh.StkhPayments[0]);
            return Request.CreateResponse(HttpStatusCode.OK,
                                          stkh.StkhPayments[0]);
        }

        [HttpPost]
        public HttpResponseMessage RejectWorking(StkhId stakeholder)
        {
            var stkh = StakeQueryBuilder.GetStakeWorkingPayment(stakeholder.Id);

            foreach (var stkhWorking in stkh.StkhWorkings)
            {
                stkhWorking.Status = ColloSysEnums.ApproveStatus.Rejected;
            }

            StakeWorkingQueryBuilder.Save(stkh.StkhWorkings);

            return Request.CreateResponse(HttpStatusCode.OK,
                                          stkh.StkhWorkings);
        }

        [HttpPost]
        public HttpResponseMessage RejectPayment(StkhId stakeholder)
        {
            var stkh = StakeQueryBuilder.GetStakeWorkingPayment(stakeholder.Id);
            if (stkh.StkhPayments[0] != null)
            {
                stkh.StkhPayments[0].ApprovalStatus = ColloSysEnums.ApproveStatus.Rejected;
            }
            StakePaymentBuilder.Save(stkh.StkhPayments[0]);
            return Request.CreateResponse(HttpStatusCode.OK,
                                          stkh.StkhPayments[0]);
        }
    }
    public class PaymentIds
    {
        public Guid ReportingId { get; set; }
        public Guid? PaymentId { get; set; }
    }
}
