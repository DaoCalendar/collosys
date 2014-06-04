using System.ServiceModel;

namespace ColloSys.FileUploadServiceInstaller
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
