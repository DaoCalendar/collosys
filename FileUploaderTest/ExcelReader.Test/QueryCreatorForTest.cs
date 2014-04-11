using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Hql.Ast.ANTLR;

namespace ReflectionExtension.Tests.ExcelReader.Test
{
   public class QueryTest
    {
        public IList<string> GetValuesFromKey()
        {
            var strList = new List<string>();
            strList.Add("204@PARTIAL REPAYMENT - REVERSAL");
            strList.Add("PARTIAL REPAYMENT - REVERSAL");
            strList.Add("204");

            return strList;
        }
    }
}
