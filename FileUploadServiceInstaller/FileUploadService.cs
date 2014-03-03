using ColloSys.FileUploadService;

namespace ColloSys.FileUploadServiceInstaller
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