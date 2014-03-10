using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColloSys.DataLayer.Domain;
using ColloSys.QueryBuilder.BaseTypes;
using NHibernate.Criterion;

namespace ColloSys.QueryBuilder.BillingBuilder
{
    public class BConditionBuilder:QueryBuilder<BCondition>
    {
        public override QueryOver<BCondition, BCondition> DefaultQuery()
        {
            throw new NotImplementedException();
        }
    }

    public class BillAdhocBuilder:QueryBuilder<BillAdhoc>
    {
        public override QueryOver<BillAdhoc, BillAdhoc> DefaultQuery()
        {
            throw new NotImplementedException();
        }
    }

    public class BillAmountBuilder:QueryBuilder<BillAmount>
    {
        public override QueryOver<BillAmount, BillAmount> DefaultQuery()
        {
            throw new NotImplementedException();
        }
    }

    public class BillDetailBuilder:QueryBuilder<BillDetail>
    {
        public override QueryOver<BillDetail, BillDetail> DefaultQuery()
        {
            throw new NotImplementedException();
        }
    }

    public class BillingPolicyBuilder:QueryBuilder<BillingPolicy>
    {
        public override QueryOver<BillingPolicy, BillingPolicy> DefaultQuery()
        {
            throw new NotImplementedException();
        }
    }

    public class BillingRelationBuilder:QueryBuilder<BillingRelation>
    {
        public override QueryOver<BillingRelation, BillingRelation> DefaultQuery()
        {
            throw new NotImplementedException();
        }
    }

    public class BillingSubpolicyBuilder:QueryBuilder<BillingSubpolicy>
    {
        public override QueryOver<BillingSubpolicy, BillingSubpolicy> DefaultQuery()
        {
            throw new NotImplementedException();
        }
    }

    public class BillStatusBuilder:QueryBuilder<BillStatus>
    {
        public override QueryOver<BillStatus, BillStatus> DefaultQuery()
        {
            throw new NotImplementedException();
        }
    }

    public class BMatrixBuilder:QueryBuilder<BMatrix>
    {
        public override QueryOver<BMatrix, BMatrix> DefaultQuery()
        {
            throw new NotImplementedException();
        }
    }

    public class BMatrixValueBuilder:QueryBuilder<BMatrixValue>
    {
        public override QueryOver<BMatrixValue, BMatrixValue> DefaultQuery()
        {
            throw new NotImplementedException();
        }
    }
}
