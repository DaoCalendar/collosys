namespace ReflectionExtension.ExcelReader
{
    public interface ICounter
    {
        ulong TotalRecords { get; }
        uint ErrorRecords { get; }
        ulong ValidRecords { get; }
        uint Duplicate { get; }
        ulong InsertRecord { get; }
        uint IgnoreRecord { get; }
        void IncrementErrorRecords();

        void IncrementValidRecords();
        void IncrementIgnoreRecord();
        void IncrementInsertRecords();
        void IncrementDuplicateRecords();
        void IncrementTotalRecords();
    }
}
