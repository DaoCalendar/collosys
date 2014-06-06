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
