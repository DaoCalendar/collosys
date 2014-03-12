#region references

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using AngularUI.Shared.apis;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.FileUploader;
using ColloSys.UserInterface.Shared.Attributes;
using NLog;

#endregion

namespace AngularUI.FileUpload.filemapping
{
    public class FileMappingApiController : BaseApiController<FileMapping>
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

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
                return (Session.QueryOver<FileDetail>()
                    .Where(c => c.ScbSystems == ScbEnums.ScbSystems.EBBS || c.ScbSystems == ScbEnums.ScbSystems.RLS).List());
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
                return (Session.QueryOver<FileColumn>().Where(c => c.FileDetail.Id == fileDetailId).List());
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