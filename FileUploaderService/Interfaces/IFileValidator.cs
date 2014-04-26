using System.IO;

namespace ColloSys.FileUploadService.Interfaces
{
    internal interface IFileValidator
    {
        bool IsFileValid(out string error);
        bool DoesFileExist(FileInfo file, out string error);
        bool DoesSizeMatches(FileInfo file, long expectedSize, out string error);
        bool DoesExtensionMatches(FileInfo file, string expectedExt, out string message);
    }
}
