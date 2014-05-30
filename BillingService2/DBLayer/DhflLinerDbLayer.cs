using System.Collections.Generic;
using System.Linq;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SessionMgr;

namespace BillingService2.DBLayer
{
    internal static class DhflLinerDbLayer
    {
        public static List<DHFL_Liner> GetDhflLinerDbData(ScbEnums.Products products, uint billMonth)
        {
            var session = SessionManager.GetCurrentSession();
            var dhflLiners = session.QueryOver<DHFL_Liner>().List<DHFL_Liner>();

            return dhflLiners.ToList();
        }
    }

    internal static class StkholderDbLayer
    {
        public static List<Stakeholders> GetStakeholdersDbData(List<string> userIds)
        {
            var session = SessionManager.GetCurrentSession();
            var stakeholdersList = session.QueryOver<Stakeholders>()
                .Where(x => userIds.Contains(x.ExternalId)).List(); ;

            return stakeholdersList.ToList();
        }
    }
}