#region references

using System;
using System.Collections.Generic;
using System.Linq;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.FileUploader;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.QueryBuilder.BaseTypes;
using ColloSys.QueryBuilder.TransAttributes;
using NHibernate.Criterion;

#endregion

namespace ColloSys.QueryBuilder.FileUploadBuilder
{
    public class FileValueMappingBuilder : Repository<FileValueMapping>
    {
        public override QueryOver<FileValueMapping, FileValueMapping> ApplyRelations()
        {
            return QueryOver.Of<FileValueMapping>();
        }

        [Transaction]
        public IEnumerable<FileValueMapping> OnFileMappingId(Guid fileMappingId)
        {
            return SessionManager.GetCurrentSession()
                                 .QueryOver<FileValueMapping>()
                                 .Where(x => x.FileMapping.Id == fileMappingId)
                                 .List();
        }
    }

    public class FileUploadableBuilder
    {
        [Transaction]
        public IEnumerable<TEntity> GetDataForDate<TEntity>(DateTime date) where TEntity : Entity, IFileUploadable
        {
            return SessionManager.GetCurrentSession().QueryOver<TEntity>()
                                                  .Where(x => x.FileDate == date)
                                                  .List();
        }

        [Transaction]
        public List<string> GetAccountNosForDate<TEntity>(DateTime dateTime) where TEntity : Entity, IUniqueKey
         {
             return (List<string>) SessionManager.GetCurrentSession().QueryOver<TEntity>()
                                         .Where(x => x.FileDate == dateTime)
                                         .Select(x => x.AccountNo)
                                         .List<string>();
         }

        [Transaction]
        public IList<TEntity> GetTableData<TEntity>() where TEntity:Entity
        {
            return SessionManager.GetCurrentSession().QueryOver<TEntity>()
                                 .List();
        }

        [Transaction]
        public IList<TEntity> GetDataForPreviousDay<TEntity>(IEnumerable<Guid> lastuploadedfileid)
             where TEntity : Entity, IFileUploadable
        {
           return SessionManager.GetCurrentSession().QueryOver<TEntity>()
                                      .WhereRestrictionOn(x => x.FileScheduler.Id)
                                      .IsIn(lastuploadedfileid.ToArray())
                                      .List();
        }
    }
}