using ColloSys.FileUploadServiceInstaller;
using ColloSys.FileUploaderService;

namespace FileUploadServiceInstallerv2
{
    public class FileUploadService : IFileUploadService
    {
        public void UploadFiles()
        {
            FileUploaderService.UploadFiles();
        }

        public void ResetFiles()
        {
            FileUploaderService.ResetFiles();
        }
    }
}