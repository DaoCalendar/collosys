using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColloSys.FileUploadService.Interfaces
{
    public interface IFileReaderNeeds
    {
        bool ReadFile(out string errorMessage);

        bool HasEofReached();

        void EnqueueRowList();

        void SaveRowList(out string errorMessage);

        void SaveErrorTable();

        bool PostProcesing();

        bool RetryErrorRows();

    }
}
