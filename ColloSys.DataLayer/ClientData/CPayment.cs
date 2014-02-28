using System.Collections.Generic;
using ColloSys.DataLayer.SharedDomain;

// ReSharper disable CheckNamespace
namespace ColloSys.DataLayer.Domain
{
    public class CPayment : Payment
    {
        public override IList<string> GetExcludeInExcelProperties()
        {
            return GetExcludeInExcelPaymentProperties();
        }
    }
}
// ReSharper restore CheckNamespace
