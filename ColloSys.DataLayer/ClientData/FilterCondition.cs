using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Billing;
using ColloSys.DataLayer.FileUploader;
using System.Collections.Generic;

namespace ColloSys.DataLayer.ClientData
{
    public class FilterCondition : Entity
    {
        public virtual IList<BillTokens> BillTokens { get; set; }
        public virtual FileDetail FileDetail { get; set; }
        public virtual string AliasConditionName { get; set; }

    }
}
