#region references

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.FileUploader;
using ColloSys.QueryBuilder.FileUploadBuilder;
using ColloSys.UserInterface.Areas.FileUploader.Models;
using ColloSys.UserInterface.Shared;
using ColloSys.UserInterface.Shared.Attributes;
using NLog;

#endregion

namespace UserInterfaceAngular.app
{
    public class FileMappingApiController : BaseApiController<FileMapping>
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private static readonly FileDetailBuilder FileDetailBuilder=new FileDetailBuilder();
        private static readonly FileColumnBuilder FileColumnBuilder=new FileColumnBuilder();


        #region Get

        [HttpGet]
        public IEnumerable<string> GetValueTypes()
        {
            return Enum.GetNames(typeof(ColloSysEnums.FileMappingValueType));
        }

        [HttpGet]
        [HttpTransaction]
        public IEnumerable<FileDetail> GetFileDetails()
        {
            try
            {
                return FileDetailBuilder.ForROrE();
            }
            catch (Exception exception)
            {
                _log.ErrorException("GetFileDetails : ", exception);
                throw new Exception(exception.Message, exception);
            }
        }

        [HttpGet]
        [HttpTransaction]
        public IEnumerable<FileMapping> GetFileMappings(Guid fileDetailId)
        {
            try
            {
                return FileMappingViewModel.GetFileMappings(fileDetailId);
            }
            catch (Exception exception)
            {
                _log.ErrorException("GetFileMappings : ", exception);
                throw new Exception(exception.Message, exception);
            }
        }

        [HttpGet]
        [HttpTransaction]
        public IEnumerable<FileColumn> GetFileColumns(Guid fileDetailId)
        {
            try
            {
                return FileColumnBuilder.OnFileDetailId(fileDetailId);
            }
            catch (Exception exception)
            {
                _log.ErrorException("GetFileDetails : ", exception);
                throw new Exception(exception.Message, exception);
            }
        }

        #endregion

        #region Post

        [HttpPost]
        [HttpTransaction(Persist = true)]
        public HttpResponseMessage SaveMapping(FileMapping fileMapping)
        {
            return fileMapping.Id == Guid.Empty ? Post(fileMapping) : Put(fileMapping.Id, fileMapping);
        }

        #endregion
    }
}