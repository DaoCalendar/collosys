using System;

namespace ColloSys.QueryBuilder.Generic
{
   public static class Util
    {
        public static DateTime GetTodayDate()
        {
            var today = DateTime.Today;
            //GetDebugDate(ref today);
            return today.Date;
        }
    }
}
