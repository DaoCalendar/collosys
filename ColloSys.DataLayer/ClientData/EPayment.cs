using System.Collections.Generic;
using ColloSys.DataLayer.SharedDomain;

// ReSharper disable CheckNamespace
namespace ColloSys.DataLayer.Domain
{
    public class EPayment : SharedPayment
    {
        public override IList<string> GetExcludeInExcelProperties()
        {
            var memberHelper = new MemberHelper<EPayment>();
            var list = GetExcludeInExcelPaymentProperties();
            list.Add(memberHelper.GetName(x => x.DebitAmount));
            return list;
        }


        #region DomainFields

        public virtual decimal DebitAmount { get; set; }

        #endregion
    }
}
// ReSharper restore CheckNamespace
