#region References

using System;
using System.Collections.Generic;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.FileUploader;
using ColloSys.DataLayer.SessionMgr;
using NHibernate.Cfg;

#endregion

namespace ReflectionExtension.Tests.DataCreator.FileUploader
{
    public class FileMappingData
    {
      //  SetUpAssemblies obj1 = new SetUpAssemblies();
       // SetUpAssembliesForTest obj=new SetUpAssembliesForTest();
        private Configuration d;
       public FileMappingData()
        {
            //obj.InitNhibernate();
        }

        //public ISession BindNewSession()
        //{
            
        //  //return  obj1.BindNewSession();
        //    //var session = SessionFactory.OpenSession();
        //    //session.FlushMode = FlushMode.Commit;
        //    ////CurrentSessionContext.Bind(session);
        //    //return session;
        //}

     
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

        public IEnumerable<FileMapping> CreateRecord()
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
            var objPayment = new Payment { TransCode = 204, TransDesc = "PARTIAL REPAYMENT - REVERSAL", CreditAmount = 400, DebitAmount = 500};

            return objPayment;
        }

        public Payment GetPaymentForTransAmount()
        {
            var objPayment = new Payment { TransCode = 204, TransDesc = "PARTIAL REPAYMENT - REVERSAL", CreditAmount = 400, DebitAmount = 500, TransAmount = 205 };

            return objPayment;
        }

        public void SetFileScheduler()
        {
            using (var session = SessionManager.GetNewSession())
            {
                using (var tx=session.BeginTransaction())
                {
                    var data = new FileScheduler() { ErrorRows = 0,AllocBillDone = true,CreateAction = "insert",
                                                     CreatedBy = "abhijeet",
                                                     CreatedOn = DateTime.Now,
                                                     FileDetail = new FileDetail() { Id = new Guid("58f60a6b-edd8-4cc2-a796-c5698eff45c6"), FileName = ""},
                        FileName = "somthing",FileDirectory = "",EndDateTime = DateTime.Now,
                        FileDate = DateTime.Now,FileStatuss = new FileStatus[]{},FileSize = 123,Category = new ScbEnums.Category(){},
                        TotalRows = 11,ValidRows = 11,UploadStatus = ColloSysEnums.UploadStatus.UploadRequest,StatusDescription = "", Version = 1,
                        CLiners = new List<CLiner>(){},CUnbilleds = new CUnbilled[]{},CWriteoffs = new CWriteoff[]{},
                        Id = new Guid("f11e47b8-8c7b-430e-9e29-d0246a54af2d"), StartDateTime = DateTime.Now};

                    session.Save(data);
                    tx.Commit();
                }
            }
        }


        public List<string> GetTransactionList()
        {
            var strList = new List<string> { "204@PARTIAL REPAYMENT - REVERSAL" };

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
