#region references

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using ColloSys.DataLayer.Enumerations;
using ColloSys.Shared.ExcelWriter;
using ColloSys.Shared.NHPagination;
using ColloSys.Shared.NgGrid;
using NHibernate;
using NLog;

#endregion

namespace ColloSys.DataLayer.Services.Shared
{
    public static class ClientDataDownloadHelper
    {
        #region client data apis

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static GridInitData GetPageData(DownloadParams downloadParams)
        {
            // get data for that filescheduler from db 
            downloadParams.SelectedDate = downloadParams.SelectedDate.Date;
            Logger.Info(string.Format("ClientPageData: DownloadFile: download data for product {0}, category {1} " +
                                      " from table {2} for date.",
                                      downloadParams.SelectedProduct, downloadParams.SelectedCategory,
                                      downloadParams.SelectedDate));

            try
            {
                var criteria = downloadParams.GetCriteria();
                Logger.Info("ClientPageData: DownloadFile: criteria =>" + criteria);
                var init = new GridInitData(criteria, downloadParams.GetTypeForCriteria(),
                    ColloSysEnums.GridScreenName.ClientDataDownload);
                init.QueryParams.GridConfig.primaryKey = "FileRowNo";
                return init;
            }
            catch (HibernateException exception)
            {
                Logger.ErrorException("ClientPageData : Error occured while executing command : " + exception.Data, exception);
                throw new Exception("NHibernate Error : " + (exception.InnerException != null
                                                                 ? exception.InnerException.Message
                                                                 : exception.Message));
            }
        }

        #endregion

        #region grid apis

        public static GridQueryResult GetGridData(GridQueryParams param)
        {
            IPagedList page = new PagedList();
            page.FetchPageData(param.GetCriteria(), param);
            var result = new GridQueryResult { PageData = page.PageData, TotalRowCount = page.TotalCount };
            return result;
        }

        public static FileInfo DownloadGridData(GridQueryParams param, bool writeOnlyIfData = false)
        {
            IPagedList page = new PagedList();
            page.FetchAllData(param.GetCriteria(), param);
            if (page.PageData.Count == 0 && writeOnlyIfData)
            {
                Logger.Info("No data to write!!!");
                return null;
            }
            var outputfilename = string.Format("{0}_{1}.xlsx", "result", DateTime.Now.ToString("_yyyyMMdd_HHmmssfff"));
            var file = new FileInfo(Path.GetTempPath() + outputfilename);
            try
            {
                uint i = 0;
                IList<ColumnPositionInfo> columns = param.GridConfig.columnDefs
                                                         .Select(gridColumn => new ColumnPositionInfo
                                                             {
                                                                 FieldName = gridColumn.field,
                                                                 DisplayName = gridColumn.displayName,
                                                                 Position = (++i),
                                                                 WriteInExcel = gridColumn.visible,
                                                                 UseFieldNameForDisplay = false,
                                                                 IsFreezed = gridColumn.pinned
                                                             })
                                                         .ToList();
                ClientDataWriter.ListToExcel(page.PageData, file, columns);
            }
            catch (Exception exception)
            {
                Logger.ErrorException("ClietnDataDownload : could not generate excel. ", exception);
                throw new ExternalException("Could not generate excel. " + exception.Message);
            }

            return file;
        }

        #endregion
    }
}