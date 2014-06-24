namespace ColloSys.FileUploaderService.ExcelReader
{
    public interface IExcelReader
    {
        uint TotalRows { get; }
        uint TotalColumns { get; }
        uint CurrentRow { get;  }
        void NextRow();
        bool EndOfFile();
        string GetValue(uint pos);
        string GetValue(uint rowPos, uint pos);
        void Skip(uint count);
    }
}
