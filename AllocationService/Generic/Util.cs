using System;
using System.Diagnostics;
using ColloSys.DataLayer.Enumerations;
using ColloSys.Shared.ConfigSectionReader;

namespace ColloSys.AllocationService.Generic
{
    public static class Util
    {
        public static DateTime GetTodayDate()
        {
            var today = DateTime.Today;
            //GetDebugDate(ref today);
            return today.Date;
        }

        [Conditional("DEBUG")]
        private static  void GetDebugDate(ref DateTime today)
        {
            try
            {
                var dateString = ColloSysParam.WebParams.AllocationDate;
                today=Convert.ToDateTime(dateString);
            }
            catch (Exception)
            {
                today= DateTime.Today;
            }
        }

        public static DateTime ComputeStartDate(int cycleCode)
        {
            var today = GetTodayDate();

            if (cycleCode > DateTime.DaysInMonth(today.Year, today.Month))
            {
                return new DateTime(today.Year, today.AddMonths(1).Month, 1);
            }
            var startDate = new DateTime(today.Year, today.Month, cycleCode);

            if (cycleCode < today.Day)
            {
                startDate = startDate.AddMonths(1);

            }
            else
            {
                startDate = startDate.AddMonths(-1);
            }
            return startDate;
        }

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
    }
}
