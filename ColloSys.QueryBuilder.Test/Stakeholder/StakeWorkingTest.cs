using System.Linq;
using System.Net;
using AngularUI.Stakeholder.addedit.Working;
using ColloSys.DataLayer.Stakeholder;
using ColloSys.QueryBuilder.GenericBuilder;
using NUnit.Framework;

namespace ColloSys.QueryBuilder.Test.Stakeholder
{
    [TestFixture]
    class StakeWorkingTest
    {
        [Test]
        public void GetPincodeData()
        {
            var workModel = new WorkingModel
                {
                    SelectedPincodeData = { Country = "India" },
                    QueryFor = LocationLevels.State,
                    DisplayManager =
                    {
                        ShowState = true,
                        ShowDistrict = true
                    }
                };

            var pincodeData = workModel.SetWorkingList(workModel);

            workModel.SelectedPincodeData.State = "Maharashtra";
            workModel.QueryFor = LocationLevels.District;
            pincodeData = workModel.SetWorkingList(workModel);

            workModel.SelectedPincodeData.District = "Pune";
            workModel.QueryFor = LocationLevels.Area;
            pincodeData = workModel.SetWorkingList(workModel);

            workModel.SelectedPincodeData.State = "Assam";
            workModel.QueryFor = LocationLevels.District;
            pincodeData = workModel.SetWorkingList(workModel);

            Assert.AreEqual(0, pincodeData.ListOfAreas.Count);
            Assert.AreEqual(5, pincodeData.ListOfDistricts.Count);

        }

        [Test]
        public void SalCalculation()
        {
            var GKeyValueBuilder = new GKeyValueBuilder();
            StkhPayment sp = new StkhPayment();
            sp.FixpayTotal = 9350;
            sp.FixpayBasic = 5610;
            var gKeyValue = GKeyValueBuilder.ForStakeholders();
            var fixPay = gKeyValue.ToDictionary(keyValue => keyValue.ParamName, keyValue => decimal.Parse(keyValue.Value));
            var data = WorkingPaymentHelper.GetSalaryDetails(sp, fixPay);

            Assert.AreEqual(163.625, data.EmployeeESI);
            Assert.AreEqual(673.2, data.EmployeePF);
            Assert.AreEqual(444.125, data.EmployerESI);
            Assert.AreEqual(763.52, data.EmployeePF);
        }
    }
}
