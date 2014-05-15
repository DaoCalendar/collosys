using System.Collections.Generic;

namespace ColloSys.FileUploader.FileReader
{
    public interface IFileReader<T>
    {
        void ReadAndSaveBatch();
        void ProcessFile();
        IList<T> GetNextBatch();
    }

}
