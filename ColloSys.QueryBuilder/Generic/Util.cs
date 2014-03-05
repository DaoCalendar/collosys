using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
