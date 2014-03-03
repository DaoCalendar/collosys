using System.Data;
using System.Data.OleDb;
using System.IO;
using ColloSys.DataLayer.Domain;

namespace ColloSys.FileUploadService.Excel2DT
{
    public static class OLEDBExcelReader
    {
        public static DataTable GenerateDataTable(FileScheduler fileScheduler, FileInfo fileInfo)
        {
            var connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileInfo.FullName
                + ";Extended Properties=\"Excel 12.0;HDR=NO;IMEX=1;\"";//TypeGuessRows=0;


            //var testingData = ReadExcel(InputFile.FullName);

            var sheetData = new DataTable();
            using (var conn = new OleDbConnection(connectionString))
            {
                conn.Open();

                var cmd = new OleDbCommand(string.Format("select * from [{0}]", fileScheduler.FileDetail.SheetName), conn);

                var dr = cmd.ExecuteReader();
                if (dr != null) sheetData.Load(dr);
                conn.Close();
            }

            sheetData.Rows[0].Delete();
            sheetData.AcceptChanges();

            return sheetData;
        }
    }
}
