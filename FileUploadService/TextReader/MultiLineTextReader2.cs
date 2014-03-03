#region references

using System;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Infra.IComponents;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.Shared;
using NLog;

#endregion

namespace ColloSys.FileUploadService.TextReader
{
    internal abstract class MultiLineTextReader<TInfo, THelper> : TextReader<string[]>
        where TInfo : Entity, IFileUploadable, new()
        where THelper : Entity, IFileUploadable, new()
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        #region constructor

        protected MultiLineTextReader(FileScheduler file)
            : base(file)
        {

        }

        #endregion

        #region abstract member

        protected abstract THelper GetRecord(string[] row);

        protected abstract bool IsRecordValid(THelper record);

        protected abstract void PopulateDefault(THelper record);

        protected abstract TInfo GetInfo(THelper helper);

        protected abstract TInfo GetByUniqueKey(TInfo record);

        protected abstract THelper GetByUniqueKey(THelper record);

        protected abstract bool PerformUpdates(THelper record);

        #endregion

        #region Save
        private THelper GetValidRecord(string[] row)
        {
            // ReSharper disable EmptyGeneralCatchClause
            try
            {
                THelper record = GetRecord(row);

                if (!IsRecordValid(record))
                    return null;

                PopulateDefault(record);

                return record;
            }
            catch
            {
            }

            return null;
            // ReSharper restore EmptyGeneralCatchClause
        }

        protected override void SaveRowList()
        {
            var rowList = DequeueRowList();

            try
            {
                using (var unit = SessionManager.GetCurrentUnitOfWork())
                {
                    ulong savedData = 0;
                    ulong validData = 0;
                    var fileScheduler = unit.CurrentSession.Load<FileScheduler>(FileScheduled.Id);

                    foreach (var row in rowList)
                    {
                        var newVersion = GetValidRecord(row);

                        if (newVersion == null)
                            continue;

                        var info = GetInfo(newVersion);
                        var helper = newVersion;

                        // info
                        var oldInfo = GetByUniqueKey(info);

                        if (oldInfo != null)
                        {
                            info.CloneUniqueProperties(oldInfo);
                            unit.CurrentSession.Evict(oldInfo);
                        }


                        // helper
                        var oldHelper = (oldInfo == null ? null : GetByUniqueKey(helper));

                        if (oldHelper != null)
                        {
                            helper.CloneUniqueProperties(oldHelper);
                            unit.CurrentSession.Evict(oldHelper);
                        }

                        var updateHelper = ((oldHelper == null) || PerformUpdates(oldHelper));

                        if (!updateHelper)
                        {
                            validData++;
                            continue;
                        }

                        info.FileDate = FileDate;
                        helper.FileDate = FileDate;


                        info.FileScheduler = fileScheduler;
                        helper.FileScheduler = fileScheduler;

                        // save or update
                        unit.CurrentSession.SaveOrUpdate(helper);
                        validData++;
                        savedData++;
                    }

                    if (rowList.Count > 0)
                    {
                        FileStatus status = GetFileStatus(validData, savedData, fileScheduler);
                        unit.CurrentSession.SaveOrUpdate(status);
                    }

                    unit.Commit();
                }
            }
            catch (Exception ex)
            {
                _log.Error("SaveRowList : " + ex.ToString());
                //Log.Warn(string.Format("SaveRowList Gives Error : {0}", ex.Message));
            }
        }

        #endregion
    }
}
