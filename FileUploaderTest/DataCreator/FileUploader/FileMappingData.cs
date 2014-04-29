#region References

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.FileUploader;
using ColloSys.DataLayer.Infra.SessionMgr;

#endregion

namespace ReflectionExtension.Tests.DataCreator.FileUploader
{
    public class FileMappingData
    {
        SetUpAssemblies setUp=new SetUpAssemblies();
        private FileMapping GetDefaultMapping()
        {
            return new FileMapping
            {
                CreatedOn = DateTime.Now,
                CreatedBy = "testproject",
                CreateAction = "Insert",
                Version = 0,
                Id = default(Guid),
                EndDate = DateTime.Today.AddMonths(1),
                FileDetail = null,
                FileValueMappings = null,
                OutputColumnName = string.Empty,
                OutputPosition = 0,
                Position = 0,
                StartDate = DateTime.Today.AddMonths(-1),
                TempColumn = string.Empty,
                DefaultValue = string.Empty,
                ActualColumn = string.Empty,
                ValueType = default(ColloSysEnums.FileMappingValueType),
                ActualTable = string.Empty,
                TempTable = string.Empty
            };
        }

        public IEnumerable<FileMapping> ExcelMapper()
        {
            IList<FileMapping> mappings = new List<FileMapping>();

            var mapping1 = GetDefaultMapping();
            mapping1.ActualColumn = "AccountNo";
            mapping1.Position = 3;
            mappings.Add(mapping1);

            var mapping2 = GetDefaultMapping();
            mapping2.ActualColumn = "DebitAmount";
            mapping2.Position = 4;
            mappings.Add(mapping2);

            var mapping3 = GetDefaultMapping();
            mapping3.ActualColumn = "CreditAmount";
            mapping3.Position = 5;
            mappings.Add(mapping3);

            return mappings;
        }

        #region Dummy Mapping For RecordCreator

        public IEnumerable<FileMapping> ExcelMapper_PassingTransCodeAndDesc()
        {
            IList<FileMapping> mappings = new List<FileMapping>();

            var mapping1 = GetDefaultMapping();
            mapping1.ActualColumn = "TransCode";
            mapping1.Position = 1;
            mappings.Add(mapping1);

            var mapping2 = GetDefaultMapping();
            mapping2.ActualColumn = "TransDesc";
            mapping2.Position = 2;
            mappings.Add(mapping2);

            return mappings;
        }

        public IEnumerable<FileMapping> ExcelMapper_PassingInvlidPosition()
        {
            IList<FileMapping> mappings = new List<FileMapping>();

            var mapping1 = GetDefaultMapping();
            mapping1.ActualColumn = "DebitAmount";
            mapping1.Position = 2;
            mappings.Add(mapping1);

            return mappings;
        }


        public IEnumerable<FileMapping> DefaultMapper()
        {
            IList<FileMapping> mappings = new List<FileMapping>();

            var mapping1 = GetDefaultMapping();
            mapping1.ActualColumn = "BillStatus";
            mapping1.DefaultValue = "Unbilled";
            mappings.Add(mapping1);

            var mapping2 = GetDefaultMapping();
            mapping2.ActualColumn = "IsExcluded";
            mapping2.DefaultValue = "True";
            mappings.Add(mapping2);

            return mappings;
        }

        public IList<FileMapping> CreateRecord()
        {
            IList<FileMapping> mappings = new List<FileMapping>();
            var mapping1 = GetDefaultMapping();
            mapping1.ActualColumn = "AccountNo";
            mapping1.Position = 3;
            mapping1.ValueType = ColloSysEnums.FileMappingValueType.ExcelValue;
            mappings.Add(mapping1);

            var mapping2 = GetDefaultMapping();
            mapping2.ActualColumn = "BillStatus";
            mapping2.ValueType = ColloSysEnums.FileMappingValueType.DefaultValue;
            mapping2.DefaultValue = "Unbilled";
            mappings.Add(mapping2);

            var mapping3 = GetDefaultMapping();
            mapping3.ActualColumn = "DebitAmount";
            mapping3.ValueType = ColloSysEnums.FileMappingValueType.ExcelValue;
            mapping3.Position = 4;
            mappings.Add(mapping3);

            var mapping4 = GetDefaultMapping();
            mapping4.ActualColumn = "CreditAmount";
            mapping4.ValueType = ColloSysEnums.FileMappingValueType.ExcelValue;
            mapping4.Position = 5;
            mappings.Add(mapping4);

            var mapping5 = GetDefaultMapping();
            mapping5.ActualColumn = "IsExcluded";
            mapping5.ValueType = ColloSysEnums.FileMappingValueType.ComputedValue;
            mappings.Add(mapping5);

            return mappings;
        }

        public IEnumerable<FileMapping> GetMappings()
        {
            IList<FileMapping> mappings = new List<FileMapping>();
            var mapping1 = GetDefaultMapping();
            mapping1.ActualColumn = "AccountNo";
            mapping1.ValueType = ColloSysEnums.FileMappingValueType.ExcelValue;
            mapping1.Position = 3;
            mappings.Add(mapping1);

            var mapping2 = GetDefaultMapping();
            mapping2.ActualColumn = "BillStatus";
            mapping2.ValueType = ColloSysEnums.FileMappingValueType.DefaultValue;
            mapping2.DefaultValue = "Billed";
            mappings.Add(mapping2);

            var mapping3 = GetDefaultMapping();
            mapping3.ActualColumn = "DebitAmount";
            mapping3.ValueType = ColloSysEnums.FileMappingValueType.ExcelValue;
            mapping3.Position = 4;
            mappings.Add(mapping3);

            var mapping4 = GetDefaultMapping();
            mapping4.ActualColumn = "CreditAmount";
            mapping4.ValueType = ColloSysEnums.FileMappingValueType.ExcelValue;
            mapping4.Position = 5;
            mappings.Add(mapping4);

            var mapping5 = GetDefaultMapping();
            mapping5.ActualColumn = "IsExcluded";
            mapping5.ValueType = ColloSysEnums.FileMappingValueType.ComputedValue;
            mappings.Add(mapping5);

            return mappings;
        }

        #endregion


        public Payment GetPayment()
        {
            var objPayment=new Payment {TransCode = 204, TransDesc = "PARTIAL REPAYMENT - REVERSAL" ,CreditAmount = 400,DebitAmount = 500};

            return objPayment;
        }

        public Payment GetPaymentForTransAmount()
        {
            var objPayment = new Payment { TransCode = 204, TransDesc = "PARTIAL REPAYMENT - REVERSAL", CreditAmount = 400, DebitAmount = 500, TransAmount = 205 };

            return objPayment;
        }

        public FileScheduler GetUploadedFile()
        {
             setUp.InitNhibernate();
            var data =
                SessionManager.GetCurrentSession()
                    .QueryOver<FileScheduler>()
                    .Where(c => c.FileDetail.Id == new Guid("A42EF611-808D-4CC2-9F6F-D15069664D4C"))
                    .And(c => c.FileName == "20140429_160719_DrillDown_Txn_1.xls")
                    .List<FileScheduler>().FirstOrDefault();

            return data;
        }
        public FileScheduler GetUploadedFileForRlsPaymentManualReserval()
        {
            setUp.InitNhibernate();
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
            setUp.InitNhibernate();
            var data =
                SessionManager.GetCurrentSession()
                    .QueryOver<FileScheduler>()
                    .Where(c => c.FileDetail.Id == new Guid("755FB655-D76D-46AB-9C4F-714A49DD7E90"))
                    .And(c => c.FileName == "20140429_190243_Final GB, SCB & AEB PL Flash - July 2013.xls")
                    .List<FileScheduler>().FirstOrDefault();

            return data;
        }
        public FileScheduler GetUploadedFileForWoSmc()
        {
            setUp.InitNhibernate();
            var data =
                SessionManager.GetCurrentSession()
                    .QueryOver<FileScheduler>()
                    .Where(c => c.FileDetail.Id == new Guid("C89DB479-6534-432C-AD81-7E43C5591EEC"))
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


        public List<string> GetTransactionList()
        {
            var strList=new List<string> {"204@PARTIAL REPAYMENT - REVERSAL"};

            return strList;
        }
       

        public IEnumerable<FileMapping> GetMappingForCheckbasicField()
        {
            IList<FileMapping> mappings = new List<FileMapping>();
            var mapping1 = GetDefaultMapping();
            mapping1.ActualColumn = "AccountNo";
            mapping1.ValueType = ColloSysEnums.FileMappingValueType.ExcelValue;
            mapping1.Position = 3;
            mappings.Add(mapping1);


            return mappings;
        }

        public Payment GetPaymentForIsRecordValid()
        {
            var objPayment = new Payment { TransDate = Convert.ToDateTime("10/10/2001") };

            return objPayment;
        }
    }
}
