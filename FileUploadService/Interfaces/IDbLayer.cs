using System;
using System.Collections.Generic;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.FileUploader;

namespace ColloSys.FileUploadService.Interfaces
{
    public interface IDBLayer
    {
        Dictionary<string, Decimal> GetUnbilledAmounts(DateTime date);
        Dictionary<string, Decimal> GetCustTotalDue(DateTime date);
        ISet<string> GetZGlobalCustId(DateTime date);

        void ChangeStatus(FileScheduler currentFile);
        void SetDoneStatus(FileScheduler currentFile, IRowCounter counter);

        List<TEntity> GetDataForDate<TEntity>(DateTime dateTime)
            where TEntity : Entity, IFileUploadable;

        List<string> GetAccountNosForDate<TEntity>(DateTime dateTime)
            where TEntity : Entity, IUniqueKey;

        IList<TEntity> GetTableData<TEntity>()
            where TEntity : Entity;

        IList<TEntity> GetDataForPreviousDay<TEntity>(ColloSysEnums.FileAliasName aliasName, DateTime date,
                                                      uint fileCount)
            where TEntity : Entity, IFileUploadable;

        void ResetFileStatus();

        FileScheduler GetNextFileForSchedule();

        void CommitData<TEntity>(IList<TEntity> data, FileScheduler file, IRowCounter counter) where TEntity : Entity;

        IEnumerable<FileValueMapping> GetFieldValueMappings(Guid id);

        IList<string> GetValuesFromKey(ColloSysEnums.Activities area, string key);

        bool SaveOrUpdateData<TEntity>(IEnumerable<TEntity> data)
            where TEntity : Entity;

        bool MergeData<TEntity>(IEnumerable<TEntity> data)
          where TEntity : Entity, IFileUploadable;

        int GetDoneFile(ColloSysEnums.FileAliasName aliasName, DateTime date);
    }
}