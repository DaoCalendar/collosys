namespace ColloSys.FileUploaderService.RowCounter
{
    public interface ICounter
    {
        ulong TotalRecords { get; }
        uint CurrentRow { get; }
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
        void IncrementLineNo();
        void CalculateTotalRecord();
    }
}
