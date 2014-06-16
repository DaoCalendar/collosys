using AngularUI.Stakeholder.addedit.Working;
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
    }
}
