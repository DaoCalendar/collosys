using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.FileUploader;
using ColloSys.UserInterface.Shared;

namespace ColloSys.UserInterface.BaseWebApi.FileUpload
{
    public class FileDetailApiController : BaseApiController<FileDetail>
    {
        protected override System.Collections.Generic.IEnumerable<FileDetail> BaseGet()
        {
            return Session.QueryOver<FileDetail>()
                          .Skip(0).Take(500)
                          .List();
        }
    }
}