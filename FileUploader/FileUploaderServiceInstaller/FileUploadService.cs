namespace ColloSys.FileUploaderService.v2.FileUploaderServiceInstaller
{
    public class FileUploadService : IFileUploadService
    {
        public void UploadFiles()
        {
            ColloSys.FileUploaderService.v2.FileUploaderService.UploadFiles();
        }

        public void ResetFiles()
        {
            ColloSys.FileUploaderService.v2.FileUploaderService.ResetFiles();
        }
    }
}