using System.ServiceModel;

namespace FileUploadServicev2
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
