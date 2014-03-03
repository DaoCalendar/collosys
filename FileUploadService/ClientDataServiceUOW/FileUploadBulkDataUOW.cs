using System;
using System.Collections.Generic;
using System.IO;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Infra.IComponents;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.SessionMgr;
using ColloSys.DataLayer.Shared;

namespace ColloSys.FileUploadService.ClientDataServiceUOW
{
    #region references

    #endregion

    public abstract class FileUploadBulkDataUOW<TSingle> : IDisposable
        where TSingle : Entity, IFileUploadable
    {
        #region ctor

        private readonly FileScheduler _fileScheduler;
        private readonly uint _flushCount;
        private IList<TSingle> _objectListInfo;
        private uint _rowCount;
        protected UnitOfWork _unit;

        protected FileUploadBulkDataUOW(FileScheduler fileScheduler, uint flushCount = 2000)
        {
            if ((fileScheduler == null))
                throw new InvalidDataException("Please make sure to pass not null list and file scheduler objects.");
            if (flushCount == 0)
                throw new InvalidDataException("Please make sure that flush count is more than zero.");

            _fileScheduler = fileScheduler;
            _flushCount = flushCount;

            _unit = SessionManager.GetCurrentUnitOfWork();
            //_unit.CurrentSession.Merge<FileScheduler>(fileScheduler);
        }

        protected void InitObjectList(ref IList<TSingle> listInfo)
        {
            //listInfo.Clear();
            _objectListInfo = listInfo;
        }

        #endregion

        #region save n merge

        protected virtual void Save(TSingle newVersion)
        {
            var oldVersion = GetByUniqueKey(newVersion);
            //oldInfo = ((oldInfo != null) && PerformUpdates(oldInfo)) ? oldInfo : null;

            if (oldVersion != null)
            {
                newVersion.CloneUniqueProperties(oldVersion);
                _unit.CurrentSession.Evict(oldVersion);
            }

            var updateHelper = ((oldVersion == null) || PerformUpdates(oldVersion));

            if (!updateHelper)
            {
                return;
            }

            newVersion.FileScheduler = _fileScheduler;

            _unit.CurrentSession.SaveOrUpdate(newVersion);

            _fileScheduler.ValidRows = ++_rowCount;

            if (_rowCount % _flushCount == 0)
            {
                Save();
            }
        }

        public virtual void Save()
        {
            SaveFileStatus();

            _unit.Commit();
        }

        private void SaveFileStatus()
        {
            FileStatus status = new FileStatus();
            status.TotalRows = _fileScheduler.TotalRows;
            status.ValidRows = _fileScheduler.ValidRows;
            status.UploadedRows = _rowCount;
            status.UploadStatus = _fileScheduler.UploadStatus;
            status.EntryTime = DateTime.Now;
            status.FileScheduler = _fileScheduler;

            _unit.CurrentSession.SaveOrUpdate(status);

        }
        #endregion

        #region 2be implemented in base

        protected abstract TSingle GetByUniqueKey(TSingle obj);

        protected virtual bool PerformUpdates(TSingle obj)
        {
            return true;
        }

        #endregion

        #region dispose

        public void Dispose()
        {
        }

        #endregion
    }
}




//// old info
//if (oldVersion != null && oldVersion.Id != Guid.Empty)
//{
//    //_unit.CurrentSession.Get<TInfo>(oldInfo.Id);
//    _unit.CurrentSession.Merge<TSingle>(newVersion);
//}
//else
//{
//    _unit.CurrentSession.SaveOrUpdate(newVersion);
//}