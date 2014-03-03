namespace ColloSys.UserInterface.Areas.Developer.Models.Excel2Db
{
    public class Excel2Db
    {
        private readonly string _connectionString;

        public Excel2Db(string connString)
        {
            _connectionString = connString;
        }

        public void UploadExcel2Db(string fileName)
        {
            var excelData = ExcelReader.ReadExcel(fileName);
            var bulkCopy = new BulkCopy(_connectionString);
            bulkCopy.UploadData(excelData);
        }
    }
}
