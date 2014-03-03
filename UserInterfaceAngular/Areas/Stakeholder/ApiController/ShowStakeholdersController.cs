#region references

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Security;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.UserInterface.Areas.Generic.Models;
using ColloSys.UserInterface.Areas.Stakeholder.ViewModels;
using ColloSys.UserInterface.AuthNAuth.Membership;
using ColloSys.UserInterface.Shared;
using ColloSys.UserInterface.Shared.Attributes;
using NLog;
using Newtonsoft.Json;

#endregion

namespace UserInterfaceAngular.app
{
    public class ShowStakeholdersController : BaseApiController<Stakeholders>
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        [HttpGet]
        [HttpTransaction]
        public IEnumerable<Stakeholders> GetAllStakeholders()
        {
            var data = StakeholderServices.GetAllStakeholders();
            if (data.Any())
                _log.Info("All Stakeholders loaded");

            return data;
        }

        [HttpGet]
        [HttpTransaction]
        public StkhHierarchy GetHierarchy(string designation, string hierarchy)
        {
            _log.Info("In load hierarchy");
            _log.Info("Designation:" + designation + " and Hierarchy" + hierarchy);
            var data = StakeholderServices.GetHierarchy(designation, hierarchy);
            _log.Info("Hierarchy:" + data);

            return data.Any() ? data.First() : null;
        }

        [HttpGet]
        [HttpTransaction]
        public IEnumerable<Stakeholders> ListForApprove()
        {
            var data = StakeholderServices.GetListForApprove();
            if (data.Any())
                _log.Info("Stakeholders list for approve is loaded");
            return data;
        }

        [HttpPost]
        [HttpTransaction(Persist = true)]
        public Stakeholders SaveEditedStakeholder(Stakeholders stakeholders)
        {
            _log.Info("In ShowStakeholders api/post");
            Stakeholders stake = null;
            try
            {
                stake = StakeholderServices.Update(stakeholders);
                _log.Info("Stakeholder approved of id: " + stake.Id);
                return stake;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
            return stake;
        }

        [HttpPost]
        [HttpTransaction(Persist = true)]
        public void SaveApprovedWithUser(Stakeholders stakeholders)
        {
            stakeholders.ApprovedBy = HttpContext.Current.User.Identity.Name;
            stakeholders.ApprovedOn = DateTime.Now;
            var hierarchy = StakeholderServices.GetHierarchy(stakeholders.Hierarchy.Designation, stakeholders.Hierarchy.Hierarchy).ToList().FirstOrDefault();
            _log.Info("Hierarchy received for user in approve");
            _log.Info("User Creation started");
            if (hierarchy != null && (hierarchy.IsUser && !StakeholderServices.CheckUserExist(stakeholders.ExternalId)))
            {
                var provider = new FnhMembershipProvider();
                MembershipCreateStatus status;
                var role = hierarchy;
                provider.CreateUser(stakeholders.ExternalId, stakeholders.EmailId, stakeholders.JoiningDate, role, out status);
                var body = EmailNotification.TemplateForApproveuser(stakeholders.Name, stakeholders.ExternalId);
                _log.Info("Sending mail to User");
                MailReport.SendMail(body, stakeholders.EmailId);
                _log.Info("Mail sended to user");
                _log.Info("User created");
            }
            StakeholderServices.Update(stakeholders);
            _log.Info("Stakeholder Approved and user created");
        }

        [HttpPost]
        [HttpTransaction(Persist = true)]
        public void ApproveAllStakeholders(IEnumerable<Stakeholders> stakeholderses)
        {
            foreach (var stakeholderse in stakeholderses)
            {
                stakeholderse.Status = ColloSysEnums.ApproveStatus.Approved;
                SaveApprovedWithUser(stakeholderse);
            }
        }

        [HttpPost]
        [HttpTransaction(Persist = true)]
        public void SaveApprovedWithoutUser(Stakeholders stakeholders)
        {
            stakeholders.ApprovedBy = HttpContext.Current.User.Identity.Name;
            stakeholders.ApprovedOn = DateTime.Now;
            StakeholderServices.Update(stakeholders);
        }

        [HttpGet]
        [HttpTransaction(Persist = true)]
        public Stakeholders ApproveIndividual(Stakeholders stakeholders, string approve)
        {
            var userName = HttpContext.Current.User.Identity.Name;
            return StakeholderServices.ApproveIndividual(stakeholders, approve, userName);
        }

        [HttpPost]
        [HttpTransaction(Persist = true)]
        public Stakeholders ApproveWorking(Stakeholders stakeholders)
        {
            var userName = HttpContext.Current.User.Identity.Name;
            _log.Info("In Stkeholders ShowStkaeholdersApi/ApproveWorking");
            return StakeholderServices.ApproveIndividual(stakeholders, "Working", userName);
        }

        [HttpPost]
        [HttpTransaction(Persist = true)]
        public Stakeholders ApprovePayment(Stakeholders stakeholders)
        {
            var userName = HttpContext.Current.User.Identity.Name;
            _log.Info("In Stkeholders ShowStkaeholdersApi/ApprovePayment");
            return StakeholderServices.ApproveIndividual(stakeholders, "Payment", userName);
        }

        [HttpPost]
        [HttpTransaction(Persist = true)]
        public Stakeholders ApproveAddress(Stakeholders stakeholders)
        {
            var userName = HttpContext.Current.User.Identity.Name;
            _log.Info("In Stkeholders ShowStkaeholdersApi/ApproveAddress");
            return StakeholderServices.ApproveIndividual(stakeholders, "Address", userName);
        }

        [HttpPost]
        [HttpTransaction(Persist = true)]
        public Stakeholders RejectWorking(Stakeholders stakeholders)
        {
            var userName = HttpContext.Current.User.Identity.Name;
            _log.Info("In Stkeholders ShowStkaeholdersApi/RejectWorking");
            return StakeholderServices.RejectIndividual(stakeholders, "Working", userName);
        }

        [HttpPost]
        [HttpTransaction(Persist = true)]
        public Stakeholders RejectPayment(Stakeholders stakeholders)
        {
            var userName = HttpContext.Current.User.Identity.Name;
            _log.Info("In Stkeholders ShowStkaeholdersApi/RejectPayment");
            return StakeholderServices.RejectIndividual(stakeholders, "Payment", userName);
        }

        [HttpPost]
        [HttpTransaction(Persist = true)]
        public Stakeholders RejectAddress(Stakeholders stakeholders)
        {
            var userName = HttpContext.Current.User.Identity.Name;
            _log.Info("In Stkeholders ShowStkaeholdersApi/RejectAddress");
            return StakeholderServices.RejectIndividual(stakeholders, "Address", userName);
        }

        [HttpGet]
        [HttpTransaction]
        public HttpResponseMessage GetReporteesStakeholder(Guid reportsToId, Guid hierarchyid)
        {
            var reporties = new ReporteesDetails
                {
                    ReportsToList = StakeholderServices.GetReportiesStakeholders(reportsToId).ToList(),
                    HasServiceCharge = StakeholderServices.GetIsFixedIndividual(hierarchyid)
                };

            return Request.CreateResponse(HttpStatusCode.OK, reporties);
        }
    }

    #region

    #endregion

}
