using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BillingService.DBLayer;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SharedDomain;
using NLog;

namespace BillingService.MoveProductToPayment
{
    public class MoveProductInfoToPayment
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static void Init()
        {
            foreach (ScbEnums.ScbSystems system in Enum.GetValues(typeof(ScbEnums.ScbSystems)))
            {
                if (system == ScbEnums.ScbSystems.CACS)
                    continue;
                Logger.Info("Moving Product from Info to Payment for System :" + system);
                switch (system)
                {
                    case ScbEnums.ScbSystems.CCMS:
                        var cData = MoveProductDbLayer.GetpaymentData();
                        if (!cData.Any())
                            break;
                        cData.ToList().ForEach(x => x.Products = ScbEnums.Products.CC);
                        MoveProductDbLayer.SaveList(cData);
                        break;
                    case ScbEnums.ScbSystems.EBBS:
                        var eData = MoveProductDbLayer.GetpaymentData();
                        if (!eData.Any())
                            break;
                        var paymentData = MoveProductDbLayer.SetPaymentToProduct(eData);
                        if (paymentData.Any())
                            MoveProductDbLayer.SaveList(paymentData);
                        break;
                    case ScbEnums.ScbSystems.RLS:
                        var rData = MoveProductDbLayer.GetpaymentData();
                        if (!rData.Any())
                            break;
                        var paymentDataR = MoveProductDbLayer.SetPaymentToProduct(rData);
                        if (paymentDataR.Any())
                            MoveProductDbLayer.SaveList(paymentDataR);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

            }
        }
    }
}
