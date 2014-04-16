using System;
using ColloSys.DataLayer.Domain;
using ColloSys.FileUploader.AliasReader;
using ColloSys.FileUploader.RowCounter;
using NSubstitute;
using NUnit.Framework;
using ReflectionExtension.ExcelReader;
using ReflectionExtension.Tests.DataCreator.FileUploader;

namespace ReflectionExtension.Tests.AliasReaderTest
{
    [TestFixture]
    class AliasPaymentRecordCreatorTest : SetUpAssemblies
    {
        private FileMappingData _mappingData;
        private FileScheduler _fileScheduler;
        private AliasPaymentRecordCreator _objCreator;
        private IExcelReader _reader;
        private ICounter _counter;

        [SetUp]
        public void Init()
        {
            //Arrange
            _mappingData = new FileMappingData();
            _fileScheduler = _mappingData.GetUploadedFile();
            _reader = new NpOiExcelReader(FileInfo);
            _counter = new ExcelRecordCounter();
            _objCreator = Substitute.For<AliasPaymentRecordCreator>(_fileScheduler, (uint)3, (uint)8);
        }

        #region Computted

        [Test]
        public void Test_ComputtedSetter_Assigning_FileDate_To_FileShedular()
        {
            //Arrange
            var payment = _mappingData.GetPayment();

            //Act
            _reader.Skip(3);
            _objCreator.ComputedSetter(payment, _reader, _counter);

            //Assert
            Assert.AreEqual(payment.FileDate, new DateTime(2014, 4, 15));
        }

        [Test]
        public void Test_ComputtedSetter_Assigning_InValid_Position()
        {
            //Arrange
            var payment = _mappingData.GetPayment();

            //Act
            _reader.Skip(2);
            bool isComputtedSetter = _objCreator.ComputedSetter(payment, _reader, _counter);

            //Assert
            Assert.AreEqual(isComputtedSetter, false);

        }

        [Test]
        public void Test_ComputtedSetter_Assigning_ValidPosition_For_AccountNo()
        {
            //Arrange
            var payment = _mappingData.GetPayment();

            //Act
            _reader.Skip(3);
            _objCreator.ComputedSetter(payment, _reader, _counter);

            //Assert
            Assert.AreEqual(payment.AccountNo, "42297532");

        }

        #endregion

        #region checkBasicField Test

        [Test]
        public void Test_CheckBasicField_Assigning_Valid_AccountNo()
        {
            //Arrange


            //Act
            _reader.Skip(3);
            var isValidBasicField = _objCreator.CheckBasicField(_reader, _counter);

            //Assert
            Assert.AreEqual(isValidBasicField, true);
        }

        [Test]
        public void Test_CheckBasicField_Assigning_InValid_AccountNo()
        {
            //Arrange

            //Act
            _reader.Skip(7);
            var isValidBasicField = _objCreator.CheckBasicField(_reader, _counter);

            //Assert
            Assert.AreEqual(isValidBasicField, false);
        }

        [Test]
        public void Test_CheckBasicField_Assigning_InValidAccountNo_Check_IgnoreCount()
        {
            //Arrange

            //Act
            _reader.Skip(7);
            _objCreator.CheckBasicField(_reader, _counter);

            //Assert
            Assert.AreEqual(_counter.IgnoreRecord, 1);
        }

        #endregion

    }

}
