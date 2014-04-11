using System.Collections.Generic;

namespace ReflectionExtension.Tests.DataCreator.Generic
{
    class GKeyValue
    {
        private IList<string> GetValuesFromKey()
        {
            var strList = new List<string> { "204@PARTIAL REPAYMENT - REVERSAL", "PARTIAL REPAYMENT - REVERSAL", "204" };

            return strList;
        }

        public IList<string> ComputedSetter()
        {
            var data = GetValuesFromKey();
            return data;
        }
    }
}
