using System.ServiceModel;

namespace FileUploaderService
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
