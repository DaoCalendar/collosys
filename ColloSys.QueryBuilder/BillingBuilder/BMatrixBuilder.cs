#region references

using System.Collections.Generic;
using ColloSys.DataLayer.Billing;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.SessionMgr;
using ColloSys.QueryBuilder.BaseTypes;
using ColloSys.QueryBuilder.TransAttributes;
using NHibernate.Criterion;

#endregion


namespace ColloSys.QueryBuilder.BillingBuilder
{
    public class BMatrixBuilder : Repository<BMatrix>
    {
        public override QueryOver<BMatrix, BMatrix> ApplyRelations()
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
        public IEnumerable<BMatrix> OnProductCategory(ScbEnums.Products product)
        {
            return SessionManager.GetCurrentSession()
                                 .QueryOver<BMatrix>()
                                 .Where(c => c.Products == product )
                                 .List();
        }
    }
}