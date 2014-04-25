﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.FileUploader.AliasFileReader;
using ColloSys.FileUploader.FileReader;
using NUnit.Framework;
using ReflectionExtension.Tests.DataCreator.FileUploader;

namespace ReflectionExtension.Tests.FileReaderTest
{
    [TestFixture]
    internal class RlsPaymentWoPlpcFileReaderTest
    {
        private IFileReader<Payment> _fileReader;
        private FileScheduler _uploadedFile;
        private FileMappingData _data;

        [SetUp]
        public void Init()
        {
            _data = new FileMappingData();
            _uploadedFile = _data.GetUploadedFile();
            _fileReader = new RlsPaymentWoPlpcFileReader(_uploadedFile);
        }

        [Test]
        public void Test_ReadAndSaveBatch_Assigning_Valid_ExcelFile()
        {
            //Arrange
            var payment = _data.GetPayment();
            var mappings = _data.GetMappings();

            //Act
            _fileReader.ReadAndSaveBatch(payment, mappings, (uint) 20);

            //Assert
            Assert.AreEqual(payment.AccountNo, "");
        }

        [Test]
        public void Test_ReadAndSaveBatch_Check_ListCount()
        {
            //Arrange
            var payment = _data.GetPayment();
            var mappings = _data.GetMappings();

            //Act
            _fileReader.ReadAndSaveBatch(payment, mappings, (uint) 20);

            //Assert
            Assert.AreEqual(_fileReader.List.Count, 16);

        }
    }
}
