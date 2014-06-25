
namespace ColloSys.FileUploader.RowCounter
{
    public class ExcelRecordCounter : ICounter
    {
        public uint CurrentRow { get; private set; }
        public ulong TotalRecords { get; private set; }
        public uint ErrorRecords { get; private set; }
        public uint ValidRecords { get; private set; }
        public uint Duplicate { get; private set; }
        public uint InsertRecord { get; private set; }
        public uint IgnoreRecord { get; private set; }

        public void IncrementErrorRecords()
        {
            ErrorRecords++;
            CurrentRow++;
        }

        public void IncrementValidRecords()
        {
            ValidRecords++;
            CurrentRow++;
        }

        public void IncrementIgnoreRecord()
        {
            IgnoreRecord++;
            CurrentRow++;
        }

        public void IncrementInsertRecords()
        {
            InsertRecord++;
        }

        public void IncrementDuplicateRecords()
        {
            Duplicate++;
        }

        public void IncrementTotalRecords()
        {
            TotalRecords++;
        }

        public void IncrementLineNo()
        {
            CurrentRow++;
        }
    }
}
