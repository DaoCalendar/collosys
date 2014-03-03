using System;
using System.Collections.Generic;
using System.IO;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Infra.IComponents;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.SessionMgr;
using ColloSys.DataLayer.Shared;

namespace ColloSys.DataLayer.ClientDataServices
{
    #region references

    #endregion

    public abstract class FileUpload2BulkDataUOW<TInfo, THelper> : IDisposable
        where TInfo : Entity, IFileUploadable
        where THelper : Entity, IFileUploadable
    {
        #region ctor

        private readonly FileScheduler _fileScheduler;
        private readonly uint _flushCount;
        private IList<TInfo> _objectListInfo;
        private uint _rowCount;
        protected UnitOfWork _unit;

        protected FileUpload2BulkDataUOW(FileScheduler fileScheduler, uint flushCount = 2000)
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

        protected void InitObjectList(ref IList<TInfo> listInfo)
        {
            //listInfo.Clear();
            _objectListInfo = listInfo;
        }

        #endregion

        #region save n merge

        protected virtual void Save(TInfo info, THelper helper)
        {
            // info
            var oldInfo = GetByUniqueKey(info);

            if (oldInfo != null)
            {
                info.CloneUniqueProperties(oldInfo);
                _unit.CurrentSession.Evict(oldInfo);
            }

            // helper
            var oldHelper = (oldInfo == null ? null : GetByUniqueKey(oldInfo, helper));

            if (oldHelper != null)
            {
                helper.CloneUniqueProperties(oldHelper);
                _unit.CurrentSession.Evict(oldHelper);
            }

            var updateHelper = ((oldHelper == null) || PerformUpdates(oldHelper));


            if (!updateHelper)
            {
                return;
            }

            info.FileScheduler = _fileScheduler;
            helper.FileScheduler = _fileScheduler;           

            // save or update
            _unit.CurrentSession.SaveOrUpdate(helper);

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

        protected abstract TInfo GetByUniqueKey(TInfo obj);

        protected abstract THelper GetByUniqueKey(TInfo objt, THelper obju);

        protected abstract bool PerformUpdates(TInfo obj);

        protected abstract bool PerformUpdates(THelper obj);

        #endregion

        #region dispose

        public void Dispose()
        {
        }

        #endregion
    }
}