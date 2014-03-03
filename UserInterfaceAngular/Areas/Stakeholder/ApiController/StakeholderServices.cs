using System;
using System.Collections.Generic;
using System.Linq;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using Iesi.Collections;
using Iesi.Collections.Generic;
using NHibernate.Linq;
using NHibernate.Transform;
using NLog;

namespace UserInterfaceAngular.app
{
    public static class StakeholderServices
    {
        public static IEnumerable<Stakeholders> GetAllStakeholders()
        {
            var session = SessionManager.GetCurrentSession();

            var stakeList = session.QueryOver<Stakeholders>()
                .Where(x => x.LeavingDate > DateTime.Now || x.LeavingDate == null)
                                   .Fetch(x => x.StkhPayments).Eager
                                   .Fetch(x => x.StkhRegistrations).Eager
                                   .Fetch(x => x.StkhWorkings).Eager
                                   .TransformUsing(Transformers.DistinctRootEntity)
                                   .List();

            foreach (var stakeholderse in stakeList)
            {
                stakeholderse.GAddress = GetAddressDetails(stakeholderse.Id);
            }

            var logger = LogManager.GetCurrentClassLogger();
            logger.Info("StakeholderServices: Total Stakeholders loaded " + stakeList.Count());
            return stakeList;
        }

        public static IEnumerable<Stakeholders> GetReportiesStakeholders(Guid reportToId)
        {
            var session = SessionManager.GetCurrentSession();
            //var stakeList = session.QueryOver<Stakeholders>(() => stak).Left
            //                       .JoinAlias(x => x.StkhPayments, () => pay)
            //                       .Where(() => stak.LeavingDate > DateTime.Now || stak.LeavingDate == null)
            //                       .And(() => stak.ReportsTo == reportToId)
            //                       .And(() => pay.StartDate < DateTime.Today && pay.EndDate > DateTime.Today)
            //                       .List();
            var stakeList = session.QueryOver<Stakeholders>()
                                   .Fetch(x => x.StkhPayments).Eager
                                   .Where(x => x.LeavingDate > DateTime.Now || x.LeavingDate == null)
                                   .And(x => x.ReportingManager == reportToId)
                                   .TransformUsing(Transformers.DistinctRootEntity)
                                   .List();
            foreach (var stake in stakeList)
            {
                var paymentList = stake.StkhPayments;
                var data = (from payment in paymentList
                                      where payment.StartDate < DateTime.Now && payment.EndDate > DateTime.Now
                                      select payment).ToList() ;
                stake.StkhPayments = new List<StkhPayment>();
                foreach (var stkhPayment in data)
                {
                    stake.StkhPayments.Add(stkhPayment);
                }
            }

            return stakeList;
        }

        public static IEnumerable<StkhHierarchy> GetHierarchy(string designation, string hierarchy)
        {
            var session = SessionManager.GetCurrentSession();

            var data = session.Query<StkhHierarchy>()
                              .Where(x => x.Designation == designation && x.Hierarchy == hierarchy)
                              .Select(x => x).ToList();
            if (data.Any())
                LogManager.GetCurrentClassLogger().Info("StakeholderServices: Total Hierarchy loaded " + data.Count());
            return data;

        }

        public static IEnumerable<Stakeholders> GetListForApprove()
        {
            var session = SessionManager.GetCurrentSession();

            var data = session.QueryOver<Stakeholders>()
                              .Where(x => x.Status == ColloSysEnums.ApproveStatus.Submitted)
                              .Fetch(x => x.StkhPayments).Eager
                              .Fetch(x => x.StkhRegistrations).Eager
                              .Fetch(x => x.StkhWorkings).Eager
                              .TransformUsing(Transformers.DistinctRootEntity)
                              .List();
            foreach (var stakeholderse in data)
            {
                stakeholderse.GAddress = GetAddressDetails(stakeholderse.Id);
            }
            LogManager.GetCurrentClassLogger()
                      .Info("StakeholderServices: Total Stakeholders for Approve loaded " + data.Count());

            return data;
        }

        public static Stakeholders Update(Stakeholders stakeholders)
        {
            stakeholders = SetStakeholder(stakeholders);
            var listOfNewAddress = new List<StakeAddress>();
            var session = SessionManager.GetCurrentSession();
            session.SaveOrUpdate(stakeholders);
            if (stakeholders.GAddress.Any())
            {
                var listOfAddresses = SetGAddress(stakeholders);
                foreach (var gAddress in listOfAddresses)
                {
                    session.SaveOrUpdate(gAddress);
                    listOfNewAddress.Add(gAddress);
                }
            }
           
            LogManager.GetCurrentClassLogger().Info("StakeholderServices: Stakeholder Updated with ID: " + stakeholders.Id);
            stakeholders.GAddress = new List<StakeAddress>(listOfNewAddress);
            
            return stakeholders;
        }

        public static bool CheckUserExist(string userName)
        {
            var session = SessionManager.GetCurrentSession();

            var data = session.Query<Users>().Count(x => x.Username == userName);
            return data > 0;
        }

        public static IEnumerable<string> PanNoList()
        {
            var session = SessionManager.GetCurrentSession();
            var data = session.Query<StkhRegistration>()
                              .Select(x => x.PanNo).ToList();

            LogManager.GetCurrentClassLogger().Info("StakeholderServices: Panno list count: " + data.Count);

            return data;
        }

        public static bool GetIsFixedIndividual(Guid hierarchyId)
        {
            var session = SessionManager.GetCurrentSession();
            var data = session.QueryOver<StkhHierarchy>()
                              .Where(x => x.Id == hierarchyId).SingleOrDefault();
            return data.HasServiceCharge;
        }
        #region approve individual

        public static Stakeholders ApproveIndividual(Stakeholders stakeholders, string approve, string userName)
        {
            switch (approve)
            {
                case "Working":
                    LogManager.GetCurrentClassLogger().Info("StakeholderServices: Stakeholders Working set for approve with Stake id: " + stakeholders.Id);
                    SetApprovedWorking(stakeholders, userName);
                    break;
                case "Payment":
                    LogManager.GetCurrentClassLogger().Info("StakeholderServices: Stakeholders Payment set for approve with Stake id: " + stakeholders.Id);
                    SetApprovedPayment(stakeholders, userName);
                    break;
                case "Address":
                    LogManager.GetCurrentClassLogger().Info("StakeholderServices: Stakeholders Address set for approve with Stake id: " + stakeholders.Id);
                    SetApprovedAddress(stakeholders, userName);
                    break;
            }
            Update(stakeholders);
            return stakeholders;
        }

        private static void SetApprovedAddress(Stakeholders stakeholders, string userName)
        {
            foreach (var address in stakeholders.GAddress)
            {
                address.Status = ColloSysEnums.ApproveStatus.Approved;
                address.ApprovedBy = userName;
                address.ApprovedOn = DateTime.Now;
            }
        }

        private static void SetApprovedPayment(Stakeholders stakeholders, string userName)
        {
            foreach (var payment in stakeholders.StkhPayments)
            {
                payment.Status = ColloSysEnums.ApproveStatus.Approved;
                payment.ApprovedBy = userName;
                payment.ApprovedOn = DateTime.Now;
            }
        }

        private static void SetApprovedWorking(Stakeholders stakeholders, string userName)
        {
            foreach (var working in stakeholders.StkhWorkings)
            {
                working.Status = ColloSysEnums.ApproveStatus.Approved;
                working.ApprovedBy = userName;
                working.ApprovedOn = DateTime.Now;
            }
        }

        #endregion

        #region
        public static Stakeholders RejectIndividual(Stakeholders stakeholders, string reject, string userName)
        {
            switch (reject)
            {
                case "Working":
                    SetRejectedWorking(stakeholders, userName);
                    LogManager.GetCurrentClassLogger().Info("StakeholderServices: Stakeholders Working set for reject with Stake id: " + stakeholders.Id);
                    break;
                case "Payment":
                    SetRejectedPayment(stakeholders, userName);
                    LogManager.GetCurrentClassLogger().Info("StakeholderServices: Stakeholders Payment set for reject with Stake id: " + stakeholders.Id);
                    break;
                case "Address":
                    SetRejectedAddress(stakeholders, userName);
                    LogManager.GetCurrentClassLogger().Info("StakeholderServices: Stakeholders Address set for reject with Stake id: " + stakeholders.Id);
                    break;
            }
            Update(stakeholders);
            //RetrieveRevision(stakeholders);

            return stakeholders;
        }

        private static void SetRejectedPayment(Stakeholders stakeholders, string userName)
        {
            foreach (var payment in stakeholders.StkhPayments)
            {
                payment.Status = ColloSysEnums.ApproveStatus.Rejected;
                payment.ApprovedBy = userName;
                payment.ApprovedOn = DateTime.Now;
            }
        }

        private static void SetRejectedWorking(Stakeholders stakeholders, string userName)
        {
            foreach (var working in stakeholders.StkhWorkings)
            {
                working.Status = ColloSysEnums.ApproveStatus.Rejected;
                working.ApprovedBy = userName;
                working.ApprovedOn = DateTime.Now;
            }
        }

        private static void SetRejectedAddress(Stakeholders stakeholders, string userName)
        {
            foreach (var address in stakeholders.GAddress)
            {
                address.Status = ColloSysEnums.ApproveStatus.Rejected;
                address.ApprovedBy = userName;
                address.ApprovedOn = DateTime.Now;
            }
        }
        #endregion

        private static IList<StakeAddress> GetAddressDetails(Guid id)
        {
            var session = SessionManager.GetCurrentSession();
            var stakeAddress = session.QueryOver<StakeAddress>()
                                      //.Where(x => x.SourceId == id && x.Source == "Stakeholder")
                                      .List();
            return stakeAddress;
        }

        #region
        private static IEnumerable<StakeAddress> SetGAddress(Stakeholders stakeholders)
        {
            var gAddresses = stakeholders.GAddress;
            foreach (var gAddress in gAddresses)
            {
                //gAddress.Source = "Stakeholder";
                gAddress.Country = "India";
                //gAddress.SourceId = stakeholders.Id;
            }
            return gAddresses;
        }

        private static Stakeholders SetStakeholder(Stakeholders stakeholders)
        {
            //set working
            SetWorking(stakeholders);
            //set payment
            SetPayment(stakeholders);
            //set registration
            SetRegistration(stakeholders);
            return stakeholders;
        }

        private static void SetRegistration(Stakeholders stakeholders)
        {
            if (stakeholders.StkhRegistrations.Any())
            {
                foreach (var stkhRegistration in stakeholders.StkhRegistrations)
                {
                    stkhRegistration.Stakeholder = stakeholders;
                }
            }
        }

        private static void SetPayment(Stakeholders stakeholders)
        {
            if (stakeholders.StkhPayments.Any())
            {
                foreach (var stkhPayment in stakeholders.StkhPayments)
                {
                    stkhPayment.Stakeholder = stakeholders;
                }
            }
        }

        private static void SetWorking(Stakeholders stakeholders)
        {
            if (stakeholders.StkhWorkings.Any())
            {
                foreach (var gWorking in stakeholders.StkhWorkings)
                {
                    gWorking.Stakeholder = stakeholders;
                }
            }
        }
        #endregion
    }
}