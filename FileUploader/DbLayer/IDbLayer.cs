#region references

using System;
using System.Collections.Generic;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;

#endregion

namespace ColloSys.FileUploaderService.DbLayer
{
   public interface IDbLayer
   {
       FileScheduler GetNextFileForSchedule();

       IList<string> GetValuesFromKey(ColloSysEnums.Activities area, string key);

       List<string> GetAccountNosForDate<TEntity>(DateTime dateTime)  where TEntity : Entity, IUniqueKey;

       IList<TEntity> GetDataForPreviousDay<TEntity>(ColloSysEnums.FileAliasName aliasName,
           DateTime date, uint filecount)
           where TEntity : Entity, IFileUploadable;

       bool SaveOrUpdateData<TEntity>(IEnumerable<TEntity> data)
           where TEntity : Entity;

   }
}
