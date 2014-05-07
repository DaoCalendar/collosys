#region references

using System.Collections.Generic;
using System.Linq;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.SessionMgr;
using ColloSys.DataLayer.SharedDomain;
using ColloSys.QueryBuilder.BaseTypes;
using ColloSys.QueryBuilder.Generic;
using ColloSys.QueryBuilder.TransAttributes;
using NHibernate.Criterion;
using NHibernate.Linq;

#endregion


namespace ColloSys.QueryBuilder.ClientDataBuilder
{
    public class InfoBuilder : Repository<CustomerInfo>
    {
        public override QueryOver<CustomerInfo, CustomerInfo> ApplyRelations()
        {
            return QueryOver.Of<CustomerInfo>().Fetch(x=>x.Allocs).Eager;
        }

        [Transaction]
        public IEnumerable<CustomerInfo> UnAllocatedCases(ScbEnums.Products products)
        {
            return SessionManager.GetCurrentSession().QueryOver<CustomerInfo>()
                                 .Where(x => x.Product == products)
                                 .And(x => x.AllocEndDate == null)
                                 .List();
        }

        [Transaction]
        public IEnumerable<CustomerInfo> OnAccNo(IList<string> accNoList)
        {
            return SessionManager.GetCurrentSession().Query<CustomerInfo>()
                                 .Where(x => accNoList.Contains(x.AccountNo)
                                             && x.AllocStatus ==
                                             ColloSysEnums.AllocStatus.AllocateToTelecalling)
                                 .ToList();
        }

        [Transaction]
        public IEnumerable<CustomerInfo> IgnoreAllocated(ScbEnums.Products products)
        {
            return SessionManager.GetCurrentSession().QueryOver<CustomerInfo>()
                                 .Where(x => x.Product == products)
                                 .And(x => x.AllocEndDate != null)
                                 .And(x => x.AllocEndDate < Util.GetTodayDate())
                                 .List();
        }

        [Transaction]
        public IEnumerable<CustomerInfo> NonPincodeEntries(ScbEnums.Products products)
        {
            return SessionManager.GetCurrentSession().QueryOver<CustomerInfo>()
                                 .Where(x => x.GPincode == null)
                                 .And(x => x.Product == products)
                                 .And(x => x.Pincode > 0)
                                 .List();
        }

        [Transaction]
        public IEnumerable<CustomerInfo> ForUnkownProduct(QueryOver<Payment, Payment> query)
        {
            return SessionManager.GetCurrentSession()
                                 .QueryOver<CustomerInfo>()
                                 .WithSubquery
                                 .WhereExists(query)
                                 .List();
        }

        [Transaction]
        public IEnumerable<string> MissingPincodeId(string pincode)
        {
            return SessionManager.GetCurrentSession().Query<CustomerInfo>()
                                 .Where(x => x.GPincode == null && x.Pincode.ToString().StartsWith(pincode))
                                 .Select(x => x.Pincode.ToString())
                                 .Distinct()
                                 .ToList();
        }

        [Transaction]
        public IEnumerable<CustomerInfo> OnAccNoProduct(string accountNo, ScbEnums.Products products)
        {
            return SessionManager.GetCurrentSession().Query<CustomerInfo>()
                                 .Where(x => x.AccountNo.ToString()
                                              .StartsWith(accountNo) && x.Product == products)
                                 .Take(10).ToList();
        }
    }
}