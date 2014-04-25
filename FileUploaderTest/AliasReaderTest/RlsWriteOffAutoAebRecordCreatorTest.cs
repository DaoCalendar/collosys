using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColloSys.DataLayer.Domain;
using ColloSys.FileUploader.AliasReader;
using ColloSys.FileUploader.RowCounter;
using NUnit.Framework;
using ReflectionExtension.ExcelReader;
using ReflectionExtension.Tests.DataCreator.FileUploader;

namespace ReflectionExtension.Tests.AliasReaderTest
{
    [TestFixture]
    class RlsWriteOffAutoAebRecordCreatorTest:SetUpAssemblies
    {
        private IExcelReader _excelReader;
        private AliasWriteOffRecordCreator record;
        private FileScheduler _fileScheduler;
        private FileMappingData _mappingData;
        private ICounter _counter;
        
        [SetUp]
        public void Init()
        {
            _mappingData = new FileMappingData();
            _excelReader = new NpOiExcelReader(FileInfo);
            _fileScheduler = _mappingData.GetUploadedFile();
            _counter=new ExcelRecordCounter();
            record = new RlsWriteOffAutoAebrecordCreator(_fileScheduler);
        }

        [Test]
        public void Test_CheckbBasicField_Assigning_Valid_cycleString()
        {
            //Arrange
            var reader = _excelReader;

            //Act
            reader.Skip(2);
           var isFieldValid= record.GetCheckBasicField(reader, _counter);

            //Assert
           Assert.AreEqual(isFieldValid,true);
        }

    }
}
