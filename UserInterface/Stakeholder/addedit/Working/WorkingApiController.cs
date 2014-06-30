#region references

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AngularUI.Generic.home;
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
        #region get data

        private static readonly StakePaymentQueryBuilder StakePaymentBuilder = new StakePaymentQueryBuilder();
        private static readonly StakeWorkingQueryBuilder StakeWorkingQueryBuilder = new StakeWorkingQueryBuilder();
        private static readonly StakeQueryBuilder StakeQueryBuilder = new StakeQueryBuilder();
        private static readonly StkhNotificationRepository StkhNotificationRepository = new StkhNotificationRepository();

        [HttpGet]
        public HttpResponseMessage GetStakeholder(Guid stakeholderId)
        {
            var stkh = Session.Query<Stakeholders>()
                              .Where(x => x.Id == stakeholderId)
                              .Fetch(x => x.Hierarchy)
                              .Fetch(x => x.StkhPayments)
                              .Fetch(x => x.StkhWorkings)
                              .Single();
            stkh.StkhWorkings = WorkingPaymentHelper.FilterWorkList(stkh.StkhWorkings);

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

        #endregion

        #region working

        [HttpPost]
        public HttpResponseMessage SaveWorking(List<StkhWorking> workingData)
        {
            workingData = WorkingPaymentHelper.SetStatusForSave(workingData);
            StakeWorkingQueryBuilder.Save(workingData);
            var reportsToIds = workingData.Select(stkhWorking => stkhWorking.ReportsTo).ToList();
            var reportsToStakeholders = StakeQueryBuilder.GetByIdList(reportsToIds);

            var data = new
            {
                WorkList = workingData,
                ReportsToList = reportsToStakeholders
            };

            if (!StkhNotificationRepository.DoesNotificationExist(
                ColloSysEnums.NotificationType.StakeholderWorkingChange, workingData[0].Stakeholder.Id))
            {
                var notify = new StakeholderNotificationManager(GetUsername());
                notify.NotifyStkhWorkingAdded(workingData[0].Stakeholder);
            }

            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpPost]
        public HttpResponseMessage DeleteWorking(List<StkhWorking> deleteList)
        {
            var hasAnyValidDelete = false;
            foreach (var stkhWorking in deleteList)
            {
                switch (stkhWorking.ApprovalStatus)
                {
                    case ColloSysEnums.ApproveStatus.Submitted:
                        StakeWorkingQueryBuilder.Delete(stkhWorking);
                        hasAnyValidDelete = true;
                        break;
                    case ColloSysEnums.ApproveStatus.Approved:
                        stkhWorking.ApprovalStatus = ColloSysEnums.ApproveStatus.Changed;
                        StakeWorkingQueryBuilder.Save(stkhWorking);
                        hasAnyValidDelete = true;
                        break;
                }
            }

            if (hasAnyValidDelete && (!StkhNotificationRepository.DoesNotificationExist(
                    ColloSysEnums.NotificationType.StakeholderWorkingChange, deleteList[0].Stakeholder.Id)))
            {
                var notify = new StakeholderNotificationManager(GetUsername());
                notify.NotifyStkhWorkingAdded(deleteList[0].Stakeholder);
            }

            var list = WorkingPaymentHelper.FilterWorkList(deleteList);
            return Request.CreateResponse(HttpStatusCode.OK, list);
        }

        [HttpPost]
        public HttpResponseMessage ApproveWorking(StkhId stakeholder)
        {
            var stkh = StakeQueryBuilder.GetStakeWorkingPayment(stakeholder.Id);
            return ApproveWorkingList(stkh.StkhWorkings);
        }

        [HttpPost]
        public HttpResponseMessage ApproveWorkingList(IList<StkhWorking> worklist)
        {
            StakeWorkingQueryBuilder.Save(WorkingPaymentHelper.SetStatusForApprove(worklist));

            var manager = new StakeholderNotificationManager(GetUsername());
            manager.NotifyApprove(ColloSysEnums.NotificationType.StakeholderWorkingChange, worklist[0].Stakeholder.Id);

            var reportsToIds = worklist.Select(stkhWorking => stkhWorking.ReportsTo).ToList();
            var reportsToStakeholders = StakeQueryBuilder.GetByIdList(reportsToIds);

            var data = new
            {
                WorkList = WorkingPaymentHelper.FilterWorkList(worklist),
                ReportsToList = reportsToStakeholders
            };

            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpPost]
        public HttpResponseMessage RejectWorking(StkhId stakeholder)
        {
            var stkh = StakeQueryBuilder.GetStakeWorkingPayment(stakeholder.Id);
            return RejectWorkingList(stkh.StkhWorkings);
        }

        [HttpPost]
        public HttpResponseMessage RejectWorkingList(IList<StkhWorking> workList)
        {
            foreach (var stkhWorking in workList)
            {
                switch (stkhWorking.ApprovalStatus)
                {
                    case ColloSysEnums.ApproveStatus.NotApplicable:
                    case ColloSysEnums.ApproveStatus.Submitted:
                        StakeWorkingQueryBuilder.Delete(stkhWorking);
                        break;
                    case ColloSysEnums.ApproveStatus.Changed:
                        stkhWorking.EndDate = null;
                        stkhWorking.ApprovalStatus = ColloSysEnums.ApproveStatus.Approved;
                        StakeWorkingQueryBuilder.Save(stkhWorking);
                        break;
                    case ColloSysEnums.ApproveStatus.Approved:
                        break;
                    default:
                        throw new Exception("invalid approval status: " + stkhWorking.ApprovalStatus);
                }
            }

            var manager = new StakeholderNotificationManager(GetUsername());
            manager.NotifyReject(ColloSysEnums.NotificationType.StakeholderWorkingChange, workList[0].Stakeholder.Id);

            var reportsToIds = workList.Select(stkhWorking => stkhWorking.ReportsTo).ToList();
            var reportsToStakeholders = StakeQueryBuilder.GetByIdList(reportsToIds);

            var data = new
            {
                WorkList = WorkingPaymentHelper.FilterWorkList(workList),
                ReportsToList = reportsToStakeholders
            };

            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        #endregion

        #region payment

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

            switch (paymentData.ApprovalStatus)
            {
                case ColloSysEnums.ApproveStatus.Approved:
                    paymentData.ApprovalStatus = ColloSysEnums.ApproveStatus.Changed;
                    break;
                case ColloSysEnums.ApproveStatus.NotApplicable:
                    paymentData.ApprovalStatus = ColloSysEnums.ApproveStatus.Submitted;
                    break;
            }
            StakePaymentBuilder.Save(paymentData);

            if (!StkhNotificationRepository.DoesNotificationExist(
                ColloSysEnums.NotificationType.StakeholderPaymentChange, paymentData.Stakeholder.Id))
            {
                var notify = new StakeholderNotificationManager(GetUsername());
                notify.NotifyStkhPaymentAdded(paymentData.Stakeholder);
            }

            return Request.CreateResponse(HttpStatusCode.OK, paymentData);
        }

        [HttpPost]
        public HttpResponseMessage ApprovePayment(StkhId stakeholder)
        {
            var stkh = StakeQueryBuilder.GetStakeWorkingPayment(stakeholder.Id);
            if (stkh.StkhPayments[0] != null &&
                stkh.StkhPayments[0].ApprovalStatus == ColloSysEnums.ApproveStatus.Submitted)
            {
                stkh.StkhPayments[0].ApprovalStatus = ColloSysEnums.ApproveStatus.Approved;
                StakePaymentBuilder.Save(stkh.StkhPayments[0]);
            }

            var manager = new StakeholderNotificationManager(GetUsername());
            manager.NotifyApprove(ColloSysEnums.NotificationType.StakeholderPaymentChange, stakeholder.Id);

            return Request.CreateResponse(HttpStatusCode.OK, stkh.StkhPayments[0]);
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

            var manager = new StakeholderNotificationManager(GetUsername());
            manager.NotifyReject(ColloSysEnums.NotificationType.StakeholderPaymentChange, stakeholder.Id);

            return Request.CreateResponse(HttpStatusCode.OK, stkh.StkhPayments[0]);
        }

        #endregion
    }

    #region helpers for payment

    public class PaymentIds
    {
        public Guid ReportingId { get; set; }
        public Guid? PaymentId { get; set; }
    }

    #endregion
}
