using System.ServiceModel;

namespace ColloSys.FileUploaderService.v2.FileUploaderServiceInstaller
{
    [ServiceContract]
    public interface IFileUploadService
    {
        [OperationContract]
        void UploadFiles();

        [OperationContract]
        void ResetFiles();
    }
}
