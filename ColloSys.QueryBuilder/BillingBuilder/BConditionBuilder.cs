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
using NHibernate.Linq;
using NHibernate.Transform;

namespace ColloSys.QueryBuilder.BillingBuilder
{
    public class BConditionBuilder : QueryBuilder<BCondition>
    {
        public override QueryOver<BCondition, BCondition> DefaultQuery()
        {
            return QueryOver.Of<BCondition>();
        }

        [Transaction]
        public IEnumerable<BCondition> OnSubpolicyId(Guid subpolicyId)
        {
           return SessionManager.GetCurrentSession().QueryOver<BCondition>()
                              .Where(c => c.BillingSubpolicy.Id == subpolicyId)
                              .List();
        }
    }

    public class BillAdhocBuilder : QueryBuilder<BillAdhoc>
    {
        public override QueryOver<BillAdhoc, BillAdhoc> DefaultQuery()
        {
            return QueryOver.Of<BillAdhoc>()
                            .Fetch(x => x.Stakeholder).Eager
                            .Fetch(x => x.BillDetails).Eager;
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
        [Transaction]
        public BillAmount OnStakeProductMonth(ScbEnums.Products products, Guid stakeId, int month)
        {
            return SessionManager.GetCurrentSession().QueryOver<BillAmount>()
               .Where(x => x.Stakeholder.Id == stakeId)
               .And(x => x.Products == products)
               .And(x => x.Month == month)
               .SingleOrDefault();
        }
    }

    public class BillDetailBuilder : QueryBuilder<BillDetail>
    {
        public override QueryOver<BillDetail, BillDetail> DefaultQuery()
        {
            return QueryOver.Of<BillDetail>();
        }

        [Transaction]
        public IEnumerable<BillDetail> OnStakeProductMonth(ScbEnums.Products products, Guid stakeId, int month)
        {
          return SessionManager.GetCurrentSession().QueryOver<BillDetail>()
                                    .Fetch(x => x.BillingPolicy).Eager
                                    .Fetch(x => x.BillingSubpolicy).Eager
                                    .Fetch(x => x.PaymentSource).Eager
                                    .Where(x => x.Stakeholder.Id == stakeId)
                                    .And(x => x.Products == products)
                                    .And(x => x.BillMonth == month)
                                    .List();
        }
    }

    public class BillingPolicyBuilder : QueryBuilder<BillingPolicy>
    {
        public override QueryOver<BillingPolicy, BillingPolicy> DefaultQuery()
        {
            return QueryOver.Of<BillingPolicy>();
        }

        [Transaction]
        public BillingPolicy OnProductCategory(ScbEnums.Products products, ScbEnums.Category category)
        {
           return SessionManager.GetCurrentSession().Query<BillingPolicy>()
                                     .Where(x => x.Products == products && x.Category == category)
                                     .FetchMany(x => x.BillingRelations)
                                     .ThenFetch(r => r.BillingSubpolicy)
                                     .ThenFetch(s => s.BConditions)
                                     .SingleOrDefault();
        }

        [Transaction]
        public IEnumerable<BillingPolicy> LinePolicies(ScbEnums.Products products)
        {
            return SessionManager.GetCurrentSession().QueryOver<BillingPolicy>()
                                 .Where(x => x.Products == products && x.Category == ScbEnums.Category.Liner)
                                 .List();
        }

        [Transaction]
        public IEnumerable<BillingPolicy> WriteoffPolicies(ScbEnums.Products products)
        {
            return SessionManager.GetCurrentSession().QueryOver<BillingPolicy>()
                                 .Where(x => x.Products == products && x.Category == ScbEnums.Category.WriteOff)
                                 .List();
        }
    }

    public class BillingRelationBuilder : QueryBuilder<BillingRelation>
    {
        public override QueryOver<BillingRelation, BillingRelation> DefaultQuery()
        {
            return QueryOver.Of<BillingRelation>();
        }

        [Transaction]
        public BillingRelation OnSubpolicyId(Guid subpolicyId)
        {
           return SessionManager.GetCurrentSession().QueryOver<BillingRelation>().Where(x => x.BillingSubpolicy.Id == subpolicyId).SingleOrDefault();
        }
    }

    public class BillingSubpolicyBuilder : QueryBuilder<BillingSubpolicy>
    {
        public override QueryOver<BillingSubpolicy, BillingSubpolicy> DefaultQuery()
        {
            return QueryOver.Of<BillingSubpolicy>();
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
        public IEnumerable<BillingSubpolicy> FormulaOnProductCategory(ScbEnums.Products product,
                                                                      ScbEnums.Category category)
        {
            return SessionManager.GetCurrentSession().QueryOver<BillingSubpolicy>()
                                 .Where(c => c.Products == product && c.Category == category
                                             && c.PayoutSubpolicyType == ColloSysEnums.PayoutSubpolicyType.Formula)
                                 .List();
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

        [Transaction]
        public IEnumerable<BillingSubpolicy> SubPoliciesInDb(ScbEnums.Products products,ScbEnums.Category category,List<Guid> savedSubnpoliciesIds)
        {
          return SessionManager.GetCurrentSession().QueryOver<BillingSubpolicy>()
                            .Where(x => x.PayoutSubpolicyType == ColloSysEnums.PayoutSubpolicyType.Subpolicy 
                                && x.Products == products && x.Category == category)
                            .WhereRestrictionOn(x => x.Id)
                            .Not.IsIn(savedSubnpoliciesIds)
                            .Fetch(x => x.BConditions).Eager
                            .Fetch(x => x.BillingRelations).Eager
                            .TransformUsing(Transformers.DistinctRootEntity)
                            .List();
        }

        [Transaction]
        public IEnumerable<BillingSubpolicy> OnProductCategory(ScbEnums.Products product, ScbEnums.Category category)
        {
            return SessionManager.GetCurrentSession()
                                 .QueryOver<BillingSubpolicy>()
                                 .Where(c => c.Products == product && c.Category == category)
                                 .List();
        }
    }

    public class BillStatusBuilder : QueryBuilder<BillStatus>
    {
        public override QueryOver<BillStatus, BillStatus> DefaultQuery()
        {
            return QueryOver.Of<BillStatus>();
        }

        [Transaction]
        public BillStatus OnProductMonth(ScbEnums.Products products, uint month)
        {
            return SessionManager.GetCurrentSession()
                                 .QueryOver<BillStatus>()
                                 .Where(x => x.Products == products && x.BillMonth == month)
                                 .SingleOrDefault();
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
        [Transaction]
        public IEnumerable<BMatrix> OnProductCategory(ScbEnums.Products product, ScbEnums.Category category)
        {
            return SessionManager.GetCurrentSession()
                                 .QueryOver<BMatrix>()
                                 .Where(c => c.Products == product && c.Category == category)
                                 .List();
        }
    }

    public class BMatrixValueBuilder : QueryBuilder<BMatrixValue>
    {
        public override QueryOver<BMatrixValue, BMatrixValue> DefaultQuery()
        {
            return QueryOver.Of<BMatrixValue>();
        }

        [Transaction]
        public IEnumerable<BMatrixValue> OnMatrixId(Guid matrixId)
        {
           return SessionManager.GetCurrentSession().QueryOver<BMatrixValue>()
                           .Where(c => c.BMatrix.Id == matrixId)
                           .List();
        }
    }
}
