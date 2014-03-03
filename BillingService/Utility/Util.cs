using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColloSys.DataLayer.Enumerations;

namespace BillingService.Utility
{
    public class Util
    {
        public static ScbEnums.ScbSystems GetSystemOnProduct(ScbEnums.Products products)
        {
            switch (products)
            {
                case ScbEnums.Products.SME_BIL:
                case ScbEnums.Products.SME_LAP:
                case ScbEnums.Products.SME_ME:
                case ScbEnums.Products.MORT:
                case ScbEnums.Products.AUTO:
                case ScbEnums.Products.PL:
                    return ScbEnums.ScbSystems.RLS;

                case ScbEnums.Products.CC:
                    return ScbEnums.ScbSystems.CCMS;

                case ScbEnums.Products.SME_LAP_OD:
                case ScbEnums.Products.AUTO_OD:
                case ScbEnums.Products.SMC:
                    return ScbEnums.ScbSystems.EBBS;
                default:
                    throw new ArgumentOutOfRangeException("products");
            }

        }

        public static uint GetVintage(uint mobWriteoff, ScbEnums.Products products)
        {
            switch (products)
            {
                case ScbEnums.Products.CC:
                    // MOB 0-1
                    if (mobWriteoff < 1)
                        return 1;
                    // MOB 1-2
                    if (mobWriteoff < 2)
                        return 2;
                    // MOB 3-12
                    if (mobWriteoff <= 12)
                        return 3;
                    // MOB 13-24
                    if (mobWriteoff <= 24)
                        return 4;
                    // MOB 25-48
                    if (mobWriteoff <= 48)
                        return 5;
                    // MOB >48
                    return 6;
                case ScbEnums.Products.SME_BIL:
                case ScbEnums.Products.SME_ME:
                case ScbEnums.Products.SME_LAP:
                case ScbEnums.Products.MORT:
                case ScbEnums.Products.AUTO:
                case ScbEnums.Products.SME_LAP_OD:
                case ScbEnums.Products.PL:
                case ScbEnums.Products.AUTO_OD:
                case ScbEnums.Products.SMC:
                    // MOB 0-1
                    if (mobWriteoff <= 1)
                        return 1;
                    // MOB 2-3
                    if (mobWriteoff <= 3)
                        return 2;
                    // MOB 4-12
                    if (mobWriteoff <= 12)
                        return 3;
                    // MOB 13-24
                    if (mobWriteoff <= 24)
                        return 4;
                    // MOB >25
                    return 5;
                default:
                    throw new ArgumentOutOfRangeException("products");
            }
        }

        public static uint GetMobWriteoff(DateTime? chargeoffDate)
        {
            if (chargeoffDate == null)
                return 0;

            var monthDiff = (DateTime.Now.Year * 12 + DateTime.Now.Month) -
                            (chargeoffDate.Value.Year * 12 + chargeoffDate.Value.Month);

            if (monthDiff <= 0)
                return 0;

            if (chargeoffDate.Value.Day > DateTime.Now.Day)
            {
                monthDiff--;
            }

            return (uint)monthDiff;
        }
    }
}
