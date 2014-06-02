using System.Collections.Generic;
using System.Linq;
using ColloSys.DataLayer.Billing;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SessionMgr;

namespace BillingService2.DBLayer
{
    internal static class DhflLinerDbLayer
    {
        public static List<DHFL_Liner> GetDhflLinerForStkholderDbData(BillStatus billStatus)
        {
            //ScbEnums.Products products, uint billMonth
            var session = SessionManager.GetCurrentSession();
            var dhflLiners = session.QueryOver<DHFL_Liner>()
                                    .Where(x =>
                                        //x.Product == billStatus.Products && 
                                        x.BillMonth == billStatus.BillMonth
                                        && x.DisbMonth == billStatus.OriginMonth
                                                && x.AgentId == billStatus.Stakeholder.ExternalId)
                                    .List<DHFL_Liner>();

            return dhflLiners.ToList();
        }
    }


    internal static class DhflInfoDbLayer
    {
        public static List<DHFL_Info> GetDhflInfo(List<uint> AppNos)
        {
            //ScbEnums.Products products, uint billMonth
            var session = SessionManager.GetCurrentSession();
            var dhflInfos = session.QueryOver<DHFL_Info>()
                                    .AndRestrictionOn(x => x.ApplNo).IsIn(AppNos)
                                    .List<DHFL_Info>();

            return dhflInfos.ToList();
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