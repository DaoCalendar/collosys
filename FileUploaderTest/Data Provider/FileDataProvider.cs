using System;
using System.Collections.Generic;
using System.Linq;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.FileUploader;
using ColloSys.DataLayer.SessionMgr;
using NHibernate.Linq;
using NHibernate.Transform;
using ReflectionExtension.Tests.DataCreator.FileUploader;

namespace ReflectionExtension.Tests
{
    public class FileDataProvider
    {
        FileMappingData data = new FileMappingData();

        private DateTime _fileDate;
        private string _dirPath;

        private FileDataProvider()
        {
            _fileDate=new DateTime(2013,06,12);
            _dirPath = "";
        }

        public FileDataProvider(DateTime fileDate,string dirPath)
        {
            _fileDate = fileDate;
            _dirPath = dirPath;
        }

        public FileScheduler GetUploadedFile(ColloSysEnums.FileAliasName alias)
        {
            var filename = string.Empty;
            switch (alias)
            {
                case ColloSysEnums.FileAliasName.CACS_ACTIVITY:
                    filename = "DrillDown_Txn_1.xls";
                    break;
                case ColloSysEnums.FileAliasName.C_LINER_COLLAGE:
                    break;
                case ColloSysEnums.FileAliasName.C_LINER_UNBILLED:
                    break;
                case ColloSysEnums.FileAliasName.C_PAYMENT_LIT:
                    break;
                case ColloSysEnums.FileAliasName.C_PAYMENT_UIT:
                    break;
                case ColloSysEnums.FileAliasName.C_PAYMENT_VISA:
                    break;
                case ColloSysEnums.FileAliasName.C_WRITEOFF:
                    break;
                case ColloSysEnums.FileAliasName.E_LINER_AUTO:
                    break;
                case ColloSysEnums.FileAliasName.E_LINER_OD_SME:
                    break;
                case ColloSysEnums.FileAliasName.E_PAYMENT_LINER:
                    filename = "./FileUploaderTest/ExcelData/product code_501N.xls";
                    break;
                case ColloSysEnums.FileAliasName.E_PAYMENT_WO_AUTO:
                    filename = "./FileUploaderTest/ExcelData/AEB Auto Charge Off Base - 28.01.2014.xls";

                    break;
                case ColloSysEnums.FileAliasName.E_PAYMENT_WO_SMC:
                    filename = "./FileUploaderTest/ExcelData/SMC Final Flash - April 2013.xls";
                    break;
                case ColloSysEnums.FileAliasName.E_WRITEOFF_AUTO:
                    filename = "./FileUploaderTest/ExcelData/Dummy AUTO OD charge off base.xls";
                    break;
                case ColloSysEnums.FileAliasName.E_WRITEOFF_SMC:
                    filename = "./FileUploaderTest/ExcelData/SMC Charge Off base - 17.08.2013.xls";
                    break;
                case ColloSysEnums.FileAliasName.R_WRITEOFF_SME:
                    break;
                case ColloSysEnums.FileAliasName.R_LINER_BFS_LOAN:
                    filename = "./FileUploaderTest/ExcelData/BFS DELQ 13-08-13n.xls";
                    break;
                case ColloSysEnums.FileAliasName.R_LINER_MORT_LOAN:
                    break;
                case ColloSysEnums.FileAliasName.R_LINER_PL:
                    break;
                case ColloSysEnums.FileAliasName.R_MANUAL_REVERSAL:
                    break;
                case ColloSysEnums.FileAliasName.R_PAYMENT_LINER:
                    filename = "./FileUploaderTest/ExcelData/DrillDown_Txn_1.xls";
                    break;
                case ColloSysEnums.FileAliasName.R_PAYMENT_WO_AEB:
                    filename = "./FileUploaderTest/ExcelData/Final GB, SCB & AEB PL Flash - July 2013.xls";
                    break;
                case ColloSysEnums.FileAliasName.R_PAYMENT_WO_PLPC:
                    filename = "./FileUploaderTest/ExcelData/Final GB, SCB & AEB PL Flash - July 2013.xls";
                    break;
                case ColloSysEnums.FileAliasName.R_WRITEOFF_PL_AEB:
                    filename = "./FileUploaderTest/ExcelData/AEB PL Charge off base - 10.12.13.xls";
                    break;
                case ColloSysEnums.FileAliasName.R_WRITEOFF_PL_SCB:
                    break;
                case ColloSysEnums.FileAliasName.R_WRITEOFF_PL_GB:
                    break;
                case ColloSysEnums.FileAliasName.R_WRITEOFF_PL_LORDS:
                    break;
                case ColloSysEnums.FileAliasName.R_WRITEOFF_AUTO_AEB:
                    filename = "./FileUploaderTest/ExcelData/AEB Auto Charge Off Base - 28.01.2014.xls";
                    break;
                case ColloSysEnums.FileAliasName.R_WRITEOFF_AUTO_GB:
                    filename = "./FileUploaderTest/ExcelData/GB Auto Charge Off Base - 28.01.2014.xls";
                    break;
                case ColloSysEnums.FileAliasName.R_WRITEOFF_AUTO_SCB:
                    filename = "./FileUploaderTest/ExcelData/SCB Auto Charge Off Base - 28.01.2014.xls";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("alias");
            }
            return GetFileScheduler(filename, alias);
        }

        public FileDetail GetFileDetail(ColloSysEnums.FileAliasName name)
        {

            var session = SessionManager.GetNewSession();
            var file = session
                  .QueryOver<FileDetail>()
                  .Where(c => c.AliasName == name)
                  .SingleOrDefault();

            return file;

        }

        public FileScheduler GetUploadedFileForWoAuto()
        {
            FileScheduler file;
            var session = SessionManager.GetNewSession();
            //data.SetFileScheduler();
            file = session
                .Query<FileScheduler>()
                .Where(c => c.FileDetail.Id == new Guid("4CBEF63A-6965-4FA0-8601-B5A98D5BDD21"))
                .Fetch(x => x.FileDetail)
                .ThenFetchMany(x => x.FileMappings)
                .FirstOrDefault();

            return file;



        }

        public FileScheduler GetFileScheduler(string fileName, ColloSysEnums.FileAliasName aliasName)
        {

            var fs = new FileScheduler()
            {
                FileName = fileName,
                FileDetail = GetFileDetail(aliasName),
                FileDate = _fileDate,
                FileDirectory = _dirPath,
                StartDateTime = new DateTime(2014, 06,11),
                EndDateTime = new DateTime(2014,06,10),
                IsImmediate = true,
                ScbSystems = ScbEnums.ScbSystems.RLS,
            };
            return fs;
        }

        //public IEnumerable<FileMapping> ExcelMapper()
        //{
        //    IList<FileMapping> mappings = new List<FileMapping>();

        //    var mapping1 = GetDefaultMapping();
        //    mapping1.ActualColumn = "AccountNo";
        //    mapping1.Position = 3;
        //    mappings.Add(mapping1);

        //    var mapping2 = GetDefaultMapping();
        //    mapping2.ActualColumn = "DebitAmount";
        //    mapping2.Position = 4;
        //    mappings.Add(mapping2);

        //    var mapping3 = GetDefaultMapping();
        //    mapping3.ActualColumn = "CreditAmount";
        //    mapping3.Position = 5;
        //    mappings.Add(mapping3);

        //    return mappings;
        //}


        //public FileScheduler GetUploadedFileForRlsPaymentManualReserval()
        //{
        //    var data =
        //        SessionManager.GetCurrentSession()
        //            .QueryOver<FileScheduler>()
        //            .Where(c => c.FileDetail.Id == new Guid("910696CC-A1D7-4035-9449-59F96D31D099"))
        //            .And(c => c.FileName == "20140429_184726_REVERSAL MIS.xls")
        //            .List<FileScheduler>().FirstOrDefault();

        //    return data;
        //}
        //public FileScheduler GetUploadedFileForRlsPaymentWoAeb()
        //{

        //    var data =
        //        SessionManager.GetCurrentSession()
        //            .QueryOver<FileScheduler>()
        //            .Where(c => c.FileDetail.Id == new Guid("755fb655-d76d-46ab-9c4f-714a49dd7e90"))
        //            .And(c => c.FileName == "20140430_180100_Final GB, SCB & AEB PL Flash - July 2013-14ColumnAuto.xls")
        //            .List<FileScheduler>().FirstOrDefault();

        //    return data;
        //}
        //public FileScheduler GetUploadedFileForWoSmc()
        //{

        //    var data =
        //        SessionManager.GetCurrentSession()
        //            .QueryOver<FileScheduler>()
        //            .Where(c => c.FileDetail.Id == new Guid("c89db479-6534-432c-ad81-7e43c5591eec"))
        //            .And(c => c.FileName == "20140429_160810_SMC Final Flash - April 2013 - Copy (2).xls")
        //            .List<FileScheduler>().FirstOrDefault();

        //    return data;
        //}

        //public FileScheduler GetUploadedFileForEbbsAutoOd()
        //{
        //    var data =
        //        SessionManager.GetCurrentSession()
        //            .QueryOver<FileScheduler>()
        //            .Where(c => c.FileDetail.Id == new Guid("4CBEF63A-6965-4FA0-8601-B5A98D5BDD21"))
        //            .And(c => c.FileName == "20140429_161823_Final Auto OD Flash - July 2013.xls")
        //            .List<FileScheduler>().FirstOrDefault();

        //    return data;
        //}
        //public FileScheduler GetUploadedFileForEbbs()
        //{
        //    var data =
        //        SessionManager.GetCurrentSession()
        //            .QueryOver<FileScheduler>()
        //            .Where(c => c.FileDetail.Id == new Guid("26DB346F-0ACD-4FB6-AF25-A0EB5A28BFAE"))
        //            .And(c => c.FileName == "20140429_182525_product code_501N.xls")
        //            .List<FileScheduler>().FirstOrDefault();

        //    return data;
        //}

    }
}
