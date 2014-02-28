using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColloSys.Shared.SharedUtils
{
    public static class StringUtils
    {
        public static IList<string> GetDuplicates(IList<string> valueArrary)
        {
            return (from lname in valueArrary
                    let count = (from name in valueArrary
                                 where name == lname
                                 select name).Count()
                    where count > 1
                    select lname).ToList();
        }
    }
}
