using System;
using System.Collections.Generic;
using System.Linq;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SessionMgr;
using NLog;

namespace BillingService2.DBLayer
{
    internal static class BillingSubpolicyDbLayer
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static List<BillingSubpolicy> GetFormulas(ScbEnums.Products products)
        {
            try
            {
                var session = SessionManager.GetCurrentSession();

                var billingPolicy = session.QueryOver<BillingSubpolicy>()
                    .Where(x => x.Products == products
                                && x.PayoutSubpolicyType ==ColloSysEnums.PayoutSubpolicyType.Formula)
                    .List<BillingSubpolicy>();
                return billingPolicy.ToList();
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("BillingPolicyDbLayer.GetSubpolicies() : {0}", ex.Message));

                return null;
            }
        }
    }
}