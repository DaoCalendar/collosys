using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.QueryBuilder.BaseTypes;
using ColloSys.QueryBuilder.TransAttributes;
using NHibernate.Criterion;

namespace ColloSys.QueryBuilder.BillingBuilder
{
    public class BConditionBuilder : QueryBuilder<BCondition>
    {
        public override QueryOver<BCondition, BCondition> DefaultQuery()
        {
            throw new NotImplementedException();
        }
    }

    public class BillAdhocBuilder : QueryBuilder<BillAdhoc>
    {
        public override QueryOver<BillAdhoc, BillAdhoc> DefaultQuery()
        {
            throw new NotImplementedException();
        }

        [Transaction]
        public IEnumerable<BillAdhoc> ForStakeholder(Stakeholders stakeholders, ScbEnums.Products products, uint month)
        {
            return SessionManager.GetCurrentSession().QueryOver<BillAdhoc>()
                                         .Where(x => x.Stakeholder.Id == stakeholders.Id
                                                     && x.Products == products
                                                     && x.StartMonth <= month
                                                     && x.EndMonth >= month)
                                         .List<BillAdhoc>();
        }
    }

    public class BillAmountBuilder : QueryBuilder<BillAmount>
    {
        public override QueryOver<BillAmount, BillAmount> DefaultQuery()
        {
            return QueryOver.Of<BillAmount>();
        }
    }

    public class BillDetailBuilder : QueryBuilder<BillDetail>
    {
        public override QueryOver<BillDetail, BillDetail> DefaultQuery()
        {
            return QueryOver.Of<BillDetail>();
        }
    }

    public class BillingPolicyBuilder : QueryBuilder<BillingPolicy>
    {
        public override QueryOver<BillingPolicy, BillingPolicy> DefaultQuery()
        {
            return QueryOver.Of<BillingPolicy>();
        }
    }

    public class BillingRelationBuilder : QueryBuilder<BillingRelation>
    {
        public override QueryOver<BillingRelation, BillingRelation> DefaultQuery()
        {
            return QueryOver.Of<BillingRelation>();
        }
    }

    public class BillingSubpolicyBuilder : QueryBuilder<BillingSubpolicy>
    {
        public override QueryOver<BillingSubpolicy, BillingSubpolicy> DefaultQuery()
        {
            throw new NotImplementedException();
        }

        [Transaction]
        public IEnumerable<BillingSubpolicy> SubpolicyOnPolicy(BillingPolicy billingPolicy, uint billMonth)
        {
            var startDate = DateTime.ParseExact(string.Format("{0}01", billMonth), "yyyyMMdd",
                                               CultureInfo.InvariantCulture);

            if (billingPolicy == null)
                return new List<BillingSubpolicy>();

            return SessionManager.GetCurrentSession().QueryOver<BillingRelation>()
                                          .Fetch(x => x.BillingSubpolicy).Eager
                                          .Fetch(x => x.BillingSubpolicy.BConditions).Eager
                                          .Where(x => x.BillingPolicy.Id == billingPolicy.Id)
                                          .And(x => (x.EndDate == null || x.EndDate >= startDate))
                                          .Select(x => x.BillingSubpolicy)
                                          .OrderBy(x => x.Priority).Asc
                                          .List<BillingSubpolicy>();
        }

        [Transaction]
        public BillingSubpolicy FormulaOnProductAndName(ScbEnums.Products products, string formulaName)
        {

            return SessionManager.GetCurrentSession().QueryOver<BillingSubpolicy>()
                                 .Fetch(x => x.BConditions).Eager
                                 .Where(x => x.Products == products
                                     && x.PayoutSubpolicyType == ColloSysEnums.PayoutSubpolicyType.Formula)
                                 .And(x => x.Name == formulaName)
                                 .SingleOrDefault();
        }
    }

    public class BillStatusBuilder : QueryBuilder<BillStatus>
    {
        public override QueryOver<BillStatus, BillStatus> DefaultQuery()
        {
            return QueryOver.Of<BillStatus>();
        }
    }

    public class BMatrixBuilder : QueryBuilder<BMatrix>
    {
        public override QueryOver<BMatrix, BMatrix> DefaultQuery()
        {
            return QueryOver.Of<BMatrix>();
        }

        [Transaction]
        public BMatrix OnProductAndName(ScbEnums.Products products, string matrixName)
        {

            return SessionManager.GetCurrentSession().QueryOver<BMatrix>()
                                 .Fetch(x => x.BMatricesValues).Eager
                                 .Where(x => x.Products == products && x.Name == matrixName)
                                 .SingleOrDefault();
        }
    }

    public class BMatrixValueBuilder : QueryBuilder<BMatrixValue>
    {
        public override QueryOver<BMatrixValue, BMatrixValue> DefaultQuery()
        {
            return QueryOver.Of<BMatrixValue>();
        }
    }
}
