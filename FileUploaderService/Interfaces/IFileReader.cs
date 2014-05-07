using System.IO;
using ColloSys.DataLayer.Domain;
using FileUploaderService.Interfaces;

namespace ColloSys.FileUploadService.Interfaces
{
    public interface IFileReader
    {
        //properties
        IDbLayer GetDataLayer { get; }
        int GetBatchSize { get; }
        FileInfo GetInputFile { get; }
        IRowCounter Counter { get; }
        FileScheduler UploadedFile { get; }
        FileReaderProperties Properties { get; }

        // methods
        void InitFileReader(FileScheduler file, FileReaderProperties properties, IFileReaderNeeds reader);
        void UploadFile();
    }
}

//bool IsFileValid(out string errorMessage);
//void SaveDoneStatus();

