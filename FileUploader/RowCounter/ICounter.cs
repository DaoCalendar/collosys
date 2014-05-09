namespace ColloSys.FileUploader.RowCounter
{
    public interface ICounter
    {
        ulong TotalRecords { get; }
       
        uint ErrorRecords { get; }
        uint ValidRecords { get; }
        uint Duplicate { get; }
        uint InsertRecord { get; }
        uint IgnoreRecord { get; }
        void IncrementErrorRecords();

        void IncrementValidRecords();
        void IncrementIgnoreRecord();
        void IncrementInsertRecords();
        void IncrementDuplicateRecords();
        void IncrementTotalRecords();
    }
}
