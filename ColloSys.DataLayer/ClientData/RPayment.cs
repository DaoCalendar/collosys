using System.Collections.Generic;
using ColloSys.DataLayer.SharedDomain;

// ReSharper disable CheckNamespace
namespace ColloSys.DataLayer.Domain
{
    public class RPayment : SharedPayment
    {
        public override IList<string> GetExcludeInExcelProperties()
        {
            var memberHelper = new MemberHelper<RPayment>();
            var list = GetExcludeInExcelPaymentProperties();
            list.Add(memberHelper.GetName(x => x.DebitAmount));
            list.Add(memberHelper.GetName(x => x.CreditAmount));
            return list;
        }

        #region NotMapped Fields
        public virtual decimal DebitAmount { get; set; }

        public virtual decimal CreditAmount { get; set; }
        #endregion
    }
}
// ReSharper restore CheckNamespace

