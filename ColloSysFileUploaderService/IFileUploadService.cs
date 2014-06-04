using System.ServiceModel;

namespace ColloSys.FileUploaderServiceInstaller
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
