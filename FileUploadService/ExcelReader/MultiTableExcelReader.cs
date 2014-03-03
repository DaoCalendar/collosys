#region references

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Infra.SessionMgr;
using NLog;

#endregion

namespace ColloSys.FileUploadService.ExcelReader
{
    internal abstract class MultiTableExcelReader<TInfo, THelper> : ExcelReader<THelper>
        where TInfo : Entity, IFileUploadable, new()
        where THelper : Entity, IFileUploadable, new()
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        #region ctor
        protected MultiTableExcelReader(FileScheduler file)
            : base(file)
        {
        }
        #endregion

        #region abstract member

        protected abstract bool PopulateComputedValue(THelper record, DataRow dr, out string errorDescription);

        protected abstract bool PopulateComputedValue(TInfo record, DataRow dr, out string errorDescription);

        protected abstract void MergeRecord(TInfo info, THelper helper);

        protected abstract bool IsRecordValid(THelper helper);

        protected abstract TInfo GetInfo(THelper helper);

        protected abstract TInfo GetByUniqueKey(TInfo info);

        protected abstract THelper GetByUniqueKey(THelper helper);

        protected abstract bool PerformUpdates(THelper helper);

        #endregion

        #region Get Next Batch

        protected override List<THelper> GetNextBatch()
        {
            var recordList = new List<THelper>();
            var rows = GetNextRows().ToArray();
            _log.Info(string.Format("Rows return by GetNextRows() : {0}", rows.Count()));

            foreach (var dr in rows)
            {
                string errorDescription;
                var helperRecord = GetTRecord(dr, out errorDescription);

                if (helperRecord == null)
                {
                    AddInErrorTable(dr, errorDescription);
                    continue;
                }

                recordList.Add(helperRecord);
            }

            _log.Info(string.Format("Total Valid Record Created : {0}, by Total Rows : {1}", recordList.Count, rows.Count()));

            return recordList;
        }

        public override THelper GetTRecord(DataRow dr, out string errorDescription)
        {
            var infoRecord = new TInfo();
            var helperRecord = new THelper();

            if (!GetRecord(dr, infoRecord, out errorDescription))
                return null;

            if (!GetRecord(dr, helperRecord, out errorDescription))
                return null;

            if (!PopulateComputedValue(infoRecord, dr, out errorDescription))
                return null;

            if (!PopulateComputedValue(helperRecord, dr, out errorDescription))
                return null;

            MergeRecord(infoRecord, helperRecord);

            return helperRecord;
        }

        #endregion

        #region Save

        protected override void SaveRowList()
        {
            try
            {
                var rowList = DequeueRowList();
                if (rowList.Count == 0)
                {
                    return;
                }

                var hasValidRows = false;

                using (var session = SessionManager.GetNewSession())
                {
                    using (var tx = session.BeginTransaction())
                    {
                        ulong savedData = 0;
                        ulong validData = 0;
                        var fileScheduler = session.Load<FileScheduler>(CurrentFile.Id);

                        foreach (var newVersion in rowList)
                        {
                            if (!IsRecordValid(newVersion))
                                continue;

                            var info = GetInfo(newVersion);
                            var helper = newVersion;

                            // info
                            var oldInfo = GetByUniqueKey(info);

                            if (oldInfo != null)
                            {
                                info.CloneUniqueProperties(oldInfo);
                                session.Evict(oldInfo);
                            }

                            // helper
                            var oldHelper = (oldInfo == null ? null : GetByUniqueKey(helper));

                            if (oldHelper != null)
                            {
                                helper.CloneUniqueProperties(oldHelper);
                                session.Evict(oldHelper);
                            }

                            var updateHelper = ((oldHelper == null) || PerformUpdates(oldHelper));

                            if (!updateHelper)
                            {
                                validData++;
                                continue;
                            }

                            info.FileDate = fileScheduler.FileDate;
                            helper.FileDate = fileScheduler.FileDate;

                            info.FileScheduler = fileScheduler;
                            helper.FileScheduler = fileScheduler;

                            // save or update
                            session.SaveOrUpdate(helper);
                            validData++;
                            savedData++;
                            hasValidRows = true;
                        }

                        if ((rowList.Count > 0) && (hasValidRows))
                        {
                            var status = GetFileStatus(validData, savedData, fileScheduler);
                            session.SaveOrUpdate(status);
                        }

                        tx.Commit();
                    }
                }

                _log.Debug("ActualUpload: saved the batch.");
                rowList.Clear();
            }
            catch (Exception ex)
            {
                _log.Error("SaveRowList : " + ex);
            }
        }

        #endregion

        #region Save Single Row

        public override bool SaveTRecord(THelper newVersion, out string errorMessage)
        {
            try
            {
                using (var unit = SessionManager.GetNewSession())
                {
                    using (var tx = unit.BeginTransaction())
                    {
                        if (!IsRecordValid(newVersion))
                        {
                            errorMessage = "Record Not Valid";
                            return false;
                        }

                        var info = GetInfo(newVersion);
                        var helper = newVersion;

                        // info
                        var oldInfo = GetByUniqueKey(info);

                        if (oldInfo != null)
                        {
                            info.CloneUniqueProperties(oldInfo);
                            unit.Evict(oldInfo);
                        }

                        // helper
                        var oldHelper = (oldInfo == null ? null : GetByUniqueKey(helper));

                        if (oldHelper != null)
                        {
                            helper.CloneUniqueProperties(oldHelper);
                            unit.Evict(oldHelper);
                        }

                        var updateHelper = ((oldHelper == null) || PerformUpdates(oldHelper));

                        if (!updateHelper)
                        {
                            errorMessage = string.Empty;
                            return true;
                        }

                        info.FileDate = CurrentFile.FileDate;
                        helper.FileDate = CurrentFile.FileDate;

                        var fileScheduler = unit.Load<FileScheduler>(CurrentFile.Id);
                        info.FileScheduler = fileScheduler;
                        helper.FileScheduler = fileScheduler;

                        // save or update
                        unit.SaveOrUpdate(helper);

                        tx.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error("SaveTRecord : " + ex);
                errorMessage = ex.Message;
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }
        #endregion
    }
}
