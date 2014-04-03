namespace ReflectionExtension.ExcelReader
{
  public  class ExcelRecordCounter:ICounter
    {
        public  ulong TotalRecords { get; private set; }
        public uint ErrorRecords { get; private set; }
        public ulong ValidRecords { get; private set; }
        public uint Duplicate { get; private set; }
        public ulong InsertRecord { get; private set; }
        public uint IgnoreRecord { get; private set; }

      public void IncrementErrorRecords()
        {
            ErrorRecords++;
        }

       
        public void IncrementValidRecords()
        {
            ValidRecords++;
        }

      public void IncrementIgnoreRecord()
      {
          IgnoreRecord++;
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

    }
}
