using System;
using System.Collections.Generic;
using System.Linq;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.SharedDomain;
using ColloSys.QueryBuilder.BaseTypes;
using ColloSys.QueryBuilder.Generic;
using ColloSys.QueryBuilder.TransAttributes;
using NHibernate.Criterion;
using NHibernate.Linq;

namespace ColloSys.QueryBuilder.ClientDataBuilder
{
    public class InfoBuilder : QueryBuilder<Info>
    {
        public override QueryOver<Info, Info> DefaultQuery()
        {
            throw new NotImplementedException();
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
    }

    public class PaymentBuilder : QueryBuilder<Payment>
    {
        public override QueryOver<Payment, Payment> DefaultQuery()
        {
            throw new NotImplementedException();
        }
    }

    public class CacsActivityBuilder : QueryBuilder<CacsActivity>
    {
        public override QueryOver<CacsActivity, CacsActivity> DefaultQuery()
        {
            throw new NotImplementedException();
        }

        [Transaction]
        public IEnumerable<CacsActivity> DataOnFileSchedular(FileScheduler fileScheduler)
        {
            return SessionManager.GetCurrentSession().QueryOver<CacsActivity>()
                              .Where(x => x.FileScheduler.Id == fileScheduler.Id)
                              .And(x => x.ConsiderInAllocation)
                              .List();
        }
    }

    public class CLinerBuilder : QueryBuilder<CLiner>
    {
        public override QueryOver<CLiner, CLiner> DefaultQuery()
        {
            throw new NotImplementedException();
        }
    }

    public class CUnbilledBuilder : QueryBuilder<CUnbilled>
    {
        public override QueryOver<CUnbilled, CUnbilled> DefaultQuery()
        {
            throw new NotImplementedException();
        }
    }

    public class CWriteoffBuilder : QueryBuilder<CWriteoff>
    {
        public override QueryOver<CWriteoff, CWriteoff> DefaultQuery()
        {
            throw new NotImplementedException();
        }
    }

    public class ELinerBuilder : QueryBuilder<ELiner>
    {
        public override QueryOver<ELiner, ELiner> DefaultQuery()
        {
            throw new NotImplementedException();
        }
    }

    public class EWriteoffBuilder : QueryBuilder<EWriteoff>
    {
        public override QueryOver<EWriteoff, EWriteoff> DefaultQuery()
        {
            throw new NotImplementedException();
        }
    }



    public class RLinerBuilder : QueryBuilder<RLiner>
    {
        public override QueryOver<RLiner, RLiner> DefaultQuery()
        {
            throw new NotImplementedException();
        }
    }

    public class RWriteoffBuilder : QueryBuilder<RWriteoff>
    {
        public override QueryOver<RWriteoff, RWriteoff> DefaultQuery()
        {
            throw new NotImplementedException();
        }
    }
}
