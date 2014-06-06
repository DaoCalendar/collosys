using System;
using System.Collections.Generic;
using System.Globalization;
using ColloSys.DataLayer.Billing;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SessionMgr;
using NLog;

namespace BillingService2.DBLayer
{
    internal static class BillingPolicyDbLayer
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        // get list of billing policy based on product
        public static BillingPolicy GetPolicies(ScbEnums.Products products, ScbEnums.Category category)
        {
            try
            {
                var session = SessionManager.GetCurrentSession();

                var billingPolicy = session.QueryOver<BillingPolicy>()
                                              .Where(x => x.Products == products)
                                              .And(x => x.Category == category)
                                              .SingleOrDefault();
                return billingPolicy;
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("BillingPolicyDbLayer.GetSubpolicies() : {0}", ex.Message));

                return null;
            }
        }


        // get list of billing subpolicy based on billing policy and category
        public static IList<BillingSubpolicy> GetSubpolicies(BillingPolicy billingPolicy, uint billMonth)
        {
            var startDate = DateTime.ParseExact(string.Format("{0}01", billMonth), "yyyyMMdd",
                                               CultureInfo.InvariantCulture);

            if (billingPolicy == null)
                return new List<BillingSubpolicy>();

            try
            {
                var session = SessionManager.GetCurrentSession();

                var billingSubpolicy = session.QueryOver<BillingRelation>()
                                              .Fetch(x => x.BillingSubpolicy).Eager
                                              //.Fetch(x => x.BillingSubpolicy.BConditions).Eager
                                              .Where(x => x.BillingPolicy.Id == billingPolicy.Id)
                                              .And(x => (x.EndDate == null || x.EndDate >= startDate))
                                              .Select(x => x.BillingSubpolicy)
                                              .OrderBy(x => x.Priority).Asc
                                              .List<BillingSubpolicy>();
                return billingSubpolicy;
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("BillingPolicyDbLayer.GetSubpolicies() : {0}", ex.Message));

                return new List<BillingSubpolicy>();
            }
        }


        public static BillingSubpolicy GetFormula(ScbEnums.Products products, string formulaName)
        {
            try
            {
                var session = SessionManager.GetCurrentSession();

                var formula = session.QueryOver<BillingSubpolicy>()
                                     //.Fetch(x => x.BConditions).Eager
                                     .Where(x => x.Products == products
                                         && x.PayoutSubpolicyType == ColloSysEnums.PayoutSubpolicyType.Formula)
                                     .And(x => x.Name == formulaName)
                                     .SingleOrDefault();
                return formula;
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("BillingPolicyDbLayer.GetSubpolicies() : {0}", ex.Message));
                return null;
            }
        }
    }
}
