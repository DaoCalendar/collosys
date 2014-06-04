namespace ColloSys.FileUploaderServiceInstaller
{
    public class FileUploadService : IFileUploadService
    {
        public void UploadFiles()
        {
           FileUploaderService.v2.FileUploaderService.UploadFiles();
        }

        public void ResetFiles()
        {
            FileUploaderService.v2.FileUploaderService.ResetFiles();
        }
    }
}