#region references

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Web.Http;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.SessionMgr;
using ColloSys.Shared.ExcelWriter;
using ColloSys.Shared.NgGrid;
using NHibernate;
using NHibernate.Criterion;
using NLog;
using System.IO.Compression;

#endregion


namespace ColloSys.UserInterface.Areas.Developer.apiController
{
    public class DatabaseTablesApiController : ApiController
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        // GET api/<controller>
        [HttpGet]
        public IEnumerable<string> GetTableNames()
        {
            return from t in typeof(CLiner).Assembly.GetTypes()
                   where typeof(Entity).IsAssignableFrom(t) && !t.IsAbstract
                   select t.Name;
        }

        // GET api/<controller>/5
        [HttpGet]
        
        public HttpResponseMessage FetchPageData(string tableName)
        {
            var type = typeof(CLiner).Assembly.GetTypes().SingleOrDefault(x => x.Name == tableName);

            if (type == null)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            var detachedCriteria = DetachedCriteria.For(type, type.Name);

            var gridData = new GridInitData(detachedCriteria, type);

            return Request.CreateResponse(HttpStatusCode.Created, gridData);
        }

        [HttpPost]
        
        public HttpResponseMessage DownLoadTables(List<string> tableNames)
        {
            // create excel from data
            var outputDirectory = Path.GetTempPath() +
                                  string.Format("DbTables_Excel_{0}", DateTime.Now.ToString("yyyyMMddHHmm"));
            var directory = new DirectoryInfo(outputDirectory);
            if (!directory.Exists)
                directory.Create();

            var fileInfo = new List<FileInfo>();

            var tableTypes = typeof(CLiner).Assembly.GetTypes().Where(x => tableNames.Contains(x.Name));
            foreach (var tableType in tableTypes)
            {
                fileInfo.Add(DownloadFile(tableType, directory));
            }

            var zipFileInfo = new FileInfo(directory.FullName + ".zip");
            
             ZipFile.CreateFromDirectory(directory.FullName, zipFileInfo.FullName);

            return Request.CreateResponse(HttpStatusCode.Created, zipFileInfo.FullName);
        }


        public FileInfo DownloadFile(Type tableType, DirectoryInfo directory)
        {
            // get data for that filescheduler from db 

            var entityName = tableType.Name;

            IList result;
            try
            {
                var session = SessionManager.GetCurrentSession();
                var criteria = session.CreateCriteria(tableType, entityName);

                result = criteria.List();
            }
            catch (HibernateException exception)
            {
                _logger.ErrorException("Error occured while executing command : " + exception.Data, exception);
                throw new Exception("NHibernate Error : " + (exception.InnerException != null
                                                                 ? exception.InnerException.Message
                                                                 : exception.Message));
            }

            var outputfilename = string.Format("DbTable_{0}.xlsx", tableType.Name);
            var file = new FileInfo(directory +"\\"+ outputfilename);
            try
            {
                var includeList = GetColumnsToWrite(tableType);

                ClientDataWriter.ListToExcel(result, file, includeList);
            }
            catch (Exception exception)
            {
                _logger.ErrorException("FileStatus : could not generate excel. ", exception);
                throw new ExternalException("Could not generate excel. " + exception.Message);
            }

            return file;
        }

        private static IList<ColumnPositionInfo> GetColumnsToWrite(Type tableType)
        {
            var includeList = new List<ColumnPositionInfo>();
            var props = tableType.GetProperties();
            uint position = 1;
            foreach (var info in props)
            {
                var isForeignKey = typeof(Entity).IsAssignableFrom(info.PropertyType);

                includeList.Add(new ColumnPositionInfo
                {
                    FieldName = isForeignKey ? info.Name + ".Id" : info.Name,
                    DisplayName = info.Name,
                    Position = (++position),
                    WriteInExcel = true,
                    IsFreezed = false,
                    UseFieldNameForDisplay = true
                });
            }

            return includeList;
        }
    }
}