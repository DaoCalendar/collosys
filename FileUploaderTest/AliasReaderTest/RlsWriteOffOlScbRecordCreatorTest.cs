using ColloSys.DataLayer.Domain;
using ColloSys.FileUploader.AliasReader;
using ColloSys.FileUploader.AliasRecordCreator;
using ColloSys.FileUploader.RowCounter;
using ColloSys.FileUploaderService.AliasRecordCreator;
using ColloSys.FileUploaderService.AliasRecordCreator.RwriteOff;
using NUnit.Framework;
using ReflectionExtension.ExcelReader;
using ReflectionExtension.Tests.DataCreator.FileUploader;

namespace ReflectionExtension.Tests.AliasReaderTest
{
    [TestFixture]
    class RlsWriteOffPlScbRecordCreatorTest : SetUpAssemblies
    {

        private IExcelReader _excelReader;
        private AliasRWriteOffRecordCreator _record;
        private FileScheduler _fileScheduler;
        private FileMappingData _mappingData;
        private ICounter _counter;

        [SetUp]
        public void Init()
        {
            _mappingData = new FileMappingData();
            _excelReader = new NpOiExcelReader(FileInfo);
            //_fileScheduler = _mappingData.GetUploadedFile();
            _counter = new ExcelRecordCounter();
            _record = new RlsRWriteOffPlScbRecordCreator(_fileScheduler);
        }


        [Test]
        public void Test_CheckBasicField_Assigning_Valid_CycleString()
        {
            //Arrange
            var reader = _excelReader;

            //Act
            reader.Skip(2);
         //   var isValidField = _record.GetCheckBasicField(reader, _counter);

            //Assert
            //Assert.AreEqual(isValidField, true);
        }
    }
}
