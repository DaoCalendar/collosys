using ColloSys.FileUploadServiceInstaller;
using ColloSys.FileUploaderService;

namespace FileUploadServiceInstallerv2
{
    public class FileUploadService : IFileUploadService
    {
        public void UploadFiles()
        {
            FileUploaderService1.UploadFiles();
        }

        public void ResetFiles()
        {
            FileUploaderService1.ResetFiles();
        }
    }
}