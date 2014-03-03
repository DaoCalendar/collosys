using System.IO;

namespace ColloSys.FileUploadService.Interfaces
{
    public class FileValidator : IFileValidator
    {
        private readonly FileInfo _fileInfo;
        private readonly long _expectedSize;
        private readonly string _expectedExt;
        private string _errorMessage;

        private bool? _isFileValid;

        public FileValidator(FileInfo file, long size, string ext)
        {
            if (file == null || string.IsNullOrWhiteSpace(ext))
            {
                throw new InvalidDataException("Please provide valid file & extension");

            }
            _fileInfo = file;
            _expectedSize = size;
            _expectedExt = ext;
        }

        public bool IsFileValid(out string error)
        {
            if (_isFileValid.HasValue)
            {
                error = _errorMessage;
                return _isFileValid.Value;
            }

            _isFileValid = DoesFileExist(_fileInfo, out _errorMessage);
            _isFileValid = _isFileValid.Value && DoesSizeMatches(_fileInfo, _expectedSize, out _errorMessage);
            _isFileValid = _isFileValid.Value && DoesExtensionMatches(_fileInfo, _expectedExt, out _errorMessage);

            error = _errorMessage;
            return _isFileValid.Value;
        }

        public bool DoesFileExist(FileInfo file, out string message)
        {
            if (file == null || !file.Exists)
            {
                message = string.Format("File '{0}' Does Not Exist",
                                        file == null ? "unknown" : file.FullName);
                return false;
            }
            message = string.Empty;
            return true;
        }

        public bool DoesSizeMatches(FileInfo file, long expectedSize, out string message)
        {
            if (file == null || file.Length != expectedSize)
            {
                message = string.Format("Size of file '{0}' is different than it was during scheduling.",
                                        file == null ? "unknown" : file.FullName);
                return false;
            }
            message = string.Empty;
            return true;
        }

        public bool DoesExtensionMatches(FileInfo file, string expectedExt, out string message)
        {
            var fileExt = file == null ? string.Empty : file.Extension.ToUpper();

            if (file == null || fileExt != expectedExt.ToUpper())
            {
                message = string.Format("Extension of file '{0}' is different than it was during scheduling.",
                                        file == null ? "unknown" : file.FullName);
                return false;
            }
            message = string.Empty;
            return true;
        }
    }
}