#region references

using System.Collections.Generic;
using System.Linq;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.FileUploader;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.Shared.ExcelWriter;

#endregion

namespace ColloSys.DataLayer.Services.FileUpload
{
    public static class FileColumnService
    {
        public static IList<ColumnPositionInfo> GetColumnDetails(ColloSysEnums.FileAliasName aliasName)
        {
            var session = SessionManager.GetCurrentSession();
            var filedetail = session.QueryOver<FileDetail>()
                                     .Where(x => x.AliasName == aliasName)
                                     .Fetch(x => x.FileMappings).Eager
                                     .SingleOrDefault<FileDetail>();

            return filedetail.FileMappings
                            .Where(x => x.OutputPosition != 0)
                             .OrderBy(x => x.OutputPosition)
                             .Select(mapping => new ColumnPositionInfo
                                 {
                                     FieldName = mapping.ActualColumn,
                                     DisplayName = mapping.OutputColumnName,
                                     Position = mapping.OutputPosition,
                                     WriteInExcel = true,
                                     IsFreezed = false,
                                     UseFieldNameForDisplay = true
                                 })
                             .ToList();
        }
    }
}
