#region references

using System;
using System.Collections.Generic;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.QueryBuilder.BaseTypes;
using ColloSys.QueryBuilder.TransAttributes;
using NHibernate.Criterion;

#endregion


namespace ColloSys.QueryBuilder.ClientDataBuilder
{
    public class CLinerBuilder : Repository<CLiner>
    {
        public override QueryOver<CLiner, CLiner> ApplyRelations()
        {
            return QueryOver.Of<CLiner>();
        }

        [Transaction]
        public List<object[]> OnFileDate(DateTime fileDate)
        {
            return (List<object[]>) SessionManager.GetCurrentSession().QueryOver<CLiner>()
                                                  .Where(x => x.FileDate == fileDate)
                                                  .SelectList(
                                                      list =>
                                                      list.SelectGroup(m => m.GlobalCustId)
                                                          .SelectSum(m => m.OutStandingBalance))
                                                  .List<object[]>();
        }

        [Transaction]
        public IEnumerable<string> GlobelCustIdList(DateTime fileDate)
        {
            return SessionManager.GetCurrentSession().QueryOver<CLiner>()
                                 .Where(x => x.FileDate == fileDate && x.Flag == ColloSysEnums.DelqFlag.Z)
                                 .Select(x => x.GlobalCustId).List<string>();
        }
    }
}