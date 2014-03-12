#region references

using System.Collections.Generic;
using System.Linq;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.SharedDomain;
using ColloSys.QueryBuilder.BaseTypes;
using ColloSys.QueryBuilder.Generic;
using ColloSys.QueryBuilder.TransAttributes;
using NHibernate.Criterion;
using NHibernate.Linq;

#endregion


namespace ColloSys.QueryBuilder.ClientDataBuilder
{
    public class InfoBuilder : QueryBuilder<Info>
    {
        public override QueryOver<Info, Info> WithRelation()
        {
            return QueryOver.Of<Info>();
        }

        [Transaction]
        public IEnumerable<Info> UnAllocatedCases(ScbEnums.Products products)
        {
            return SessionManager.GetCurrentSession().QueryOver<Info>()
                                 .Where(x => x.Product == products)
                                 .And(x => x.AllocEndDate == null)
                                 .List();
        }

        [Transaction]
        public IEnumerable<Info> OnAccNo(IList<string> accNoList)
        {
            return SessionManager.GetCurrentSession().Query<Info>()
                                 .Where(x => accNoList.Contains(x.AccountNo)
                                             && x.AllocStatus ==
                                             ColloSysEnums.AllocStatus.AllocateToTelecalling)
                                 .ToList();
        }

        [Transaction]
        public IEnumerable<Info> IgnoreAllocated(ScbEnums.Products products)
        {
            return SessionManager.GetCurrentSession().QueryOver<Info>()
                                 .Where(x => x.Product == products)
                                 .And(x => x.AllocEndDate != null)
                                 .And(x => x.AllocEndDate < Util.GetTodayDate())
                                 .List();
        }

        [Transaction]
        public IEnumerable<Info> NonPincodeEntries(ScbEnums.Products products)
        {
            return SessionManager.GetCurrentSession().QueryOver<Info>()
                                 .Where(x => x.GPincode == null)
                                 .And(x => x.Product == products)
                                 .And(x => x.Pincode > 0)
                                 .List();
        }

        [Transaction]
        public IEnumerable<Info> ForUnkownProduct(QueryOver<Payment, Payment> query)
        {
            return SessionManager.GetCurrentSession()
                                 .QueryOver<Info>()
                                 .WithSubquery
                                 .WhereExists(query)
                                 .List();
        }

        [Transaction]
        public IEnumerable<string> MissingPincodeId(string pincode)
        {
            return SessionManager.GetCurrentSession().Query<Info>()
                                 .Where(x => x.GPincode == null && x.Pincode.ToString().StartsWith(pincode))
                                 .Select(x => x.Pincode.ToString())
                                 .Distinct()
                                 .ToList();
        }

        [Transaction]
        public IEnumerable<Info> OnAccNoProduct(string accountNo, ScbEnums.Products products)
        {
            return SessionManager.GetCurrentSession().Query<Info>()
                                 .Where(x => x.AccountNo.ToString()
                                              .StartsWith(accountNo) && x.Product == products)
                                 .Take(10).ToList();
        }
    }
}