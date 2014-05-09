#region references

using System.Collections.Generic;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.FileUploader.AliasReader;
using ColloSys.FileUploader.RecordCreator;
using ColloSys.FileUploader.RowCounter;
using NUnit.Framework;
using ReflectionExtension.ExcelReader;
using ReflectionExtension.Tests.DataCreator.FileUploader;

#endregion


namespace ReflectionExtension.Tests.RecordCreatorTest
{
    [TestFixture]
    class RecordCreatorTest : SetUpAssemblies
    {
        #region octor

        private FileMappingData _mappingData;
        private IRecord<Payment> _record;
        private IExcelReader _reader;
        private ICounter _counter;
        // ReSharper disable once UnassignedField.Compiler
        private IAliasRecordCreator<Payment> _aliasRecordCreator;
        private Payment _payment;
        private readonly IList<string> _strlist = new List<string>();
        private readonly List<string> _eWriteoff = new List<string>();
        private FileScheduler _uploadedFile;

        [SetUp]
        public void Init()
        {
            _uploadedFile = new FileScheduler();
           _aliasRecordCreator = new RlsPaymentLinerRecordCreator(_uploadedFile);
            _payment = new Payment();
            _mappingData = new FileMappingData();
            _reader = new NpOiExcelReader(FileInfo);
            _counter = new ExcelRecordCounter();
            _record = new RecordCreator<Payment>(_aliasRecordCreator,_reader,_counter);
        }

        #endregion

        #region ::ExcelMapper() Test Cases::
        [TestCase()]
        public void Test_ExcelMapper_Assigning_Valid_Dummy_Mappings()
        {
            //Arrange
            var mappings = _mappingData.ExcelMapper();

            //Act
            _reader.Skip(3);
            _record.ExcelMapper(_payment, mappings);

            //Assert
            Assert.AreEqual(_payment.AccountNo, "42297532");
        }

        [Test]
        public void Test_ExcelMapper_Check_TransCode()
        {
            //Arrange
            var mappings = _mappingData.ExcelMapper_PassingTransCodeAndDesc();

            //Act
            _reader.Skip(3);
            _record.ExcelMapper(_payment, mappings);

            //Assert
            Assert.AreEqual(_payment.TransCode, 204);
        }

        [Test]
        public void Test_ExcelMapper_Check_TransDesc()
        {
            //Arrange
            var mappings = _mappingData.ExcelMapper_PassingTransCodeAndDesc();

            //Act
            _reader.Skip(3);
            _record.ExcelMapper(_payment, mappings);

            //Assert
            Assert.AreEqual(_payment.TransDesc, "PARTIAL REPAYMENT - REVERSAL");
        }

        [Test]
        public void Test_ExcelMapper_Assigning_Mappings_on_InValid_Row()
        {
            //Arrange
            var mappings = _mappingData.ExcelMapper();

            //Act
            _reader.Skip(2);
            _record.ExcelMapper(_payment, mappings);

            //Assert
            Assert.AreNotEqual(_payment.AccountNo, "42297532");
        }

        [Test]
        public void Test_ExcelMapper_Assigning_Mappings_OnInvlid_Row_Check_ErrorCount()
        {
            //Arrange
            var mappings = _mappingData.ExcelMapper();

            //Act
            _reader.Skip(1);
            _record.ExcelMapper(_payment, mappings);

            //Assert
            Assert.AreEqual(_counter.ErrorRecords, 1);
        }

        [Test]
        public void Test_ExcelMapper_Assigning_Mappings_Check_DebitAmount()
        {
            //Arrange
            var mappings = _mappingData.ExcelMapper();

            //Act
            _reader.Skip(3);
            _record.ExcelMapper(_payment, mappings);

            //Assert
            Assert.AreEqual(_payment.DebitAmount, 6725);
        }

        [Test]
        public void Test_ExcelMapper_Assigning_InValid_Position()
        {
            //Arrange
            var mappings = _mappingData.ExcelMapper_PassingInvlidPosition();

            //Act
            _reader.Skip(3);
            _record.ExcelMapper(_payment, mappings);

            //Assert
            Assert.AreEqual(_counter.ErrorRecords, 1);
        }
        #endregion

        #region :: DefaultMapper ::

        [Test]
        public void Test_DefaultMapper_Assigning_Valid_Mappings()
        {
            //Arrange
            var mapping = _mappingData.DefaultMapper();

            //Act
            _reader.Skip(3);
            _record.DefaultMapper(_payment, mapping);

            //Assert
            Assert.AreEqual(_payment.BillStatus, ColloSysEnums.BillStatus.Unbilled);
        }

        [Test]
        public void Test_DefaultMapper_Assigning_InValid_Mappings()
        {
            //Arrange
            var mapping = _mappingData.DefaultMapper();

            //Act
            _reader.Skip(2);
            _record.DefaultMapper(_payment, mapping);

            //Assert
            Assert.AreEqual(_payment.IsExcluded, true);
        }

        #endregion

        #region ::CreateRecord() Test ::

        [Test]
        public void Test_CreateRecord_Assigning_ValidMapping_Check_AccNo()
        {
            //Arrange
            var mapping = _mappingData.CreateRecord();

            //Act
            _reader.Skip(3);
            _record.CreateRecord(_payment, mapping,_counter);

            //Assert
            Assert.AreEqual(_payment.AccountNo, "42297532");

        }

        [Test]
        public void Test_CreateRecord_Assigning_Valid_Mapping()
        {
            //Arrange
            var mapping = _mappingData.CreateRecord();

            //Act
            _reader.Skip(3);
            _record.CreateRecord(_payment, mapping,_counter);

            //Assert
            Assert.AreEqual(_counter.InsertRecord, 1);
        }

        [Test]
        public void Test_CreateRecord_Assigning_InValid_Record_Mapping()
        {
            //Arrange
            var mapping = _mappingData.CreateRecord();

            //Act
            _reader.Skip(2);
            _record.CreateRecord(_payment, mapping,_counter  );

            //Assert
            Assert.AreEqual(_counter.IgnoreRecord, 1);

        }

        [Test]
        public void Test_CreateRecord_Assigning_InValid_Mapping_ErrorCount()
        {
            //Arrange
            var mapping = _mappingData.CreateRecord();

            //Act
            _reader.Skip(9);
            _record.CreateRecord(_payment, mapping,_counter);

            //Assert
            Assert.AreEqual(_counter.ErrorRecords, 1);

        }
        #endregion

        #region :: GetMapping ::

        [Test]
        public void Test_GetMappings_Assigning_Mapping()
        {
            //Arrange
            var data = _mappingData.GetMappings();

            //Act
            var mappings = _record.GetMappings(ColloSysEnums.FileMappingValueType.ExcelValue, data);

            //Assert
            Assert.AreEqual(mappings.Count, 3);
        }

        [Test]
        public void Test_GetMappings_Assigning_Mapping_Check_DefaultMapping_Count()
        {
            //Arrange
            var data = _mappingData.GetMappings();

            //Act
            var mappings = _record.GetMappings(ColloSysEnums.FileMappingValueType.DefaultValue, data);

            //Assert
            Assert.AreEqual(mappings.Count, 1);
        }

        [Test]
        public void Test_GetMappings_Assigning_Mapping_Check_ComputedMapping_Count()
        {
            //Arrange
            var data = _mappingData.GetMappings();

            //Act
            var mappings = _record.GetMappings(ColloSysEnums.FileMappingValueType.ComputedValue, data);

            //Assert
            Assert.AreEqual(mappings.Count, 1);
        }

        #endregion

    }
}
