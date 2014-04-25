﻿using System.Collections.Generic;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.Mapping;
using ColloSys.QueryBuilder.BaseTypes;
using ColloSys.QueryBuilder.Generic;
using ColloSys.QueryBuilder.TransAttributes;

namespace ColloSys.QueryBuilder.BillingBuilder
{
    public class HoldingPolicyBuilder : Repository<HoldingPolicy>
    {
        [Transaction]
        public IEnumerable<HoldingPolicy> OnProduct(ScbEnums.Products products)
        {
            return
                SessionManager.GetCurrentSession().
                QueryOver<HoldingPolicy>().
                Where(x => x.Products == products)
                .And(x => x.StartDate > Util.GetTodayDate())
                              .And(x => x.EndDate == null ||
                                         x.EndDate.Value < Util.GetTodayDate())
                .List();
        }
    }
}
