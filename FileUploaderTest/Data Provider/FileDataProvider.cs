using System;
using System.Linq;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.SessionMgr;
using ReflectionExtension.Tests.DataCreator.FileUploader;

namespace ReflectionExtension.Tests
{
    class FileDataProvider
    {
        FileMappingData data = new FileMappingData();

        public FileScheduler GetUploadedFile()
        {
            FileScheduler file;
            var session = SessionManager.GetNewSession();
            data.SetFileScheduler();
            file = session
                .QueryOver<FileScheduler>()
                .Where(c => c.FileDetail.Id == new Guid("f11e47b8-8c7b-430e-9e29-d0246a54af2d"))
                .List<FileScheduler>().FirstOrDefault();

            return file;



        }

        public FileScheduler GetUploadedFileForRlsPaymentManualReserval()
        {
            var data =
                SessionManager.GetCurrentSession()
                    .QueryOver<FileScheduler>()
                    .Where(c => c.FileDetail.Id == new Guid("910696CC-A1D7-4035-9449-59F96D31D099"))
                    .And(c => c.FileName == "20140429_184726_REVERSAL MIS.xls")
                    .List<FileScheduler>().FirstOrDefault();

            return data;
        }
        public FileScheduler GetUploadedFileForRlsPaymentWoAeb()
        {

            var data =
                SessionManager.GetCurrentSession()
                    .QueryOver<FileScheduler>()
                    .Where(c => c.FileDetail.Id == new Guid("755fb655-d76d-46ab-9c4f-714a49dd7e90"))
                    .And(c => c.FileName == "20140430_180100_Final GB, SCB & AEB PL Flash - July 2013-14ColumnAuto.xls")
                    .List<FileScheduler>().FirstOrDefault();

            return data;
        }
        public FileScheduler GetUploadedFileForWoSmc()
        {

            var data =
                SessionManager.GetCurrentSession()
                    .QueryOver<FileScheduler>()
                    .Where(c => c.FileDetail.Id == new Guid("c89db479-6534-432c-ad81-7e43c5591eec"))
                    .And(c => c.FileName == "20140429_160810_SMC Final Flash - April 2013 - Copy (2).xls")
                    .List<FileScheduler>().FirstOrDefault();

            return data;
        }

        public FileScheduler GetUploadedFileForEbbsAutoOd()
        {
            var data =
                SessionManager.GetCurrentSession()
                    .QueryOver<FileScheduler>()
                    .Where(c => c.FileDetail.Id == new Guid("4CBEF63A-6965-4FA0-8601-B5A98D5BDD21"))
                    .And(c => c.FileName == "20140429_161823_Final Auto OD Flash - July 2013.xls")
                    .List<FileScheduler>().FirstOrDefault();

            return data;
        }
        public FileScheduler GetUploadedFileForEbbs()
        {
            var data =
                SessionManager.GetCurrentSession()
                    .QueryOver<FileScheduler>()
                    .Where(c => c.FileDetail.Id == new Guid("26DB346F-0ACD-4FB6-AF25-A0EB5A28BFAE"))
                    .And(c => c.FileName == "20140429_182525_product code_501N.xls")
                    .List<FileScheduler>().FirstOrDefault();

            return data;
        }

    }
}
