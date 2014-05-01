using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.FileUploader;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.QueryBuilder.BaseTypes;
using ColloSys.QueryBuilder.TransAttributes;

namespace ColloSys.QueryBuilder.ClientDataBuilder
{
   public class FilterConditionBuilder:Repository<FilterCondition>
    {

       [Transaction]
       public IEnumerable<FilterCondition> OnAliasNameChange(Guid fileDetail)
       {
           return SessionManager.GetCurrentSession()
               .QueryOver<FilterCondition>()
               //.Fetch(x => x.Fconditions).Eager
               //.Fetch(x => x.AliasConditionName).Eager
               .Where(x => x.FileDetail.Id == fileDetail)
               .List();

       }
    }
}
