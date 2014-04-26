using System;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using NLog;

namespace BillingService.DBLayer
{
    internal static class BMatrixDbLayer
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        // get matrix based on matrix name
        public static BMatrix GetMatrix(ScbEnums.Products products, string matrixName)
        {
            try
            {
                var session = SessionManager.GetCurrentSession();

                var bMatrix = session.QueryOver<BMatrix>()
                                     .Fetch(x => x.BMatricesValues).Eager
                                     .Where(x => x.Products == products && x.Name == matrixName)
                                     .SingleOrDefault();
                return bMatrix;
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("BillingPolicyDbLayer.GetSubpolicies() : {0}", ex.Message));
                return null;
            }
        }
    }
}