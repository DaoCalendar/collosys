using System;
using System.Collections.Generic;
using System.Linq;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SessionMgr;
using ColloSys.DataLayer.Stakeholder;

namespace ColloSys.UserInterface.Areas.Developer.Models.Excel2Db
{
    public class InitStakeholder
    {
        public void InsertIntoStakeholder()
        {
            //Insert Default 
            SaveStakeholder(GetDefaultStakeholder());
        }

        private void SaveStakeholder(Stakeholders stakeholder)
        {
            try
            {
                Save(stakeholder);
            }
            catch (Exception)
            {

            }
        }

        private Stakeholders GetDefaultStakeholder()
        {
            var stakeholder = new Stakeholders
                {
                    //BirthDate = new DateTime(1986, 2, 5),
                    JoiningDate = DateTime.Now,
                    //Designation = "HOC",
                    EmailId = "hoc@gmail.com",
                    ExternalId = "0000000",
                    //Gender = 0,
                    //Hierarchy = "Field",
                    MobileNo = "1236547891",
                    Name = "Hoc1",
                    StkhPayments = new List<StkhPayment> { GetDefaultPayment() },
                    StkhWorkings = new List<StkhWorking> { GetDefaultWorking() },
                    StkhRegistrations = new List<StkhRegistration> { GetDefaultRegistration() },
                    StkhAddress = new List<StkhAddress> { GetDefaultAddress() }
                };

            return stakeholder;
        }

        private StkhAddress GetDefaultAddress()
        {
            var address = new StkhAddress
                {
                    //AddressType = "Head Office",
                    Country = "India",
                    //IsOfficial = true,
                    LandlineNo = "12345678911",
                    Line1 = "line 1",
                    Line2 = "line 2",
                    Line3 = "line 3",
                    Pincode = 123654
                };
            return address;
        }

        private StkhWorking GetDefaultWorking()
        {
            var working = new StkhWorking
                {
                    Area = "area",
                    City = "Mumbai",
                    Cluster = "Mumbai",
                    Country = "India",
                    District = "Mumbai",
                    Products = ScbEnums.Products.CC,
                    Region = "West",
                    State = "Maharashtra"
                };
            return working;
        }

        private StkhRegistration GetDefaultRegistration()
        {
            var registration = new StkhRegistration
                {
                    //HasCollector = false,
                    PanNo = "",
                    RegistrationNo = "",
                    ServiceTaxno = "",
                    TanNo = ""
                };

            return registration;
        }

        private StkhPayment GetDefaultPayment()
        {
            var payment = new StkhPayment
                {
                    EndDate = DateTime.Now.AddMonths(6),
                    FixpayBasic = 1000,
                    //FixpayHra = 2000,
                    FixpayGross = 1000,
                    //FixpayTotal = 0,
                    MobileElig = 2000,
                    TravelElig = 1000,
                    StartDate = DateTime.Now
                };
            return payment;
        }

        #region StakeholderServices

        private static void Save(Stakeholders stakeholders)
        {
            stakeholders = SetStakeholder(stakeholders);

            using (var unit = SessionManager.GetNewSession())
            {
                using (var tx = unit.BeginTransaction())
                {
                    unit.SaveOrUpdate(stakeholders);
                    if (stakeholders.StkhAddress.Any())
                    {
                        var listOfAddresses = SetGAddress(stakeholders);
                        foreach (var gAddress in listOfAddresses)
                        {
                            unit.SaveOrUpdate(gAddress);
                        }
                    }
                    tx.Commit();
                }
            }
        }

        private static Stakeholders SetStakeholder(Stakeholders stakeholders)
        {
            //set working
            SetWorking(stakeholders);
            //set payment
            SetPayment(stakeholders);
            //set registration
            SetRegistration(stakeholders);
            return stakeholders;
        }

        private static IEnumerable<StkhAddress> SetGAddress(Stakeholders stakeholders)
        {
            var gAddresses = stakeholders.StkhAddress;
            foreach (var gAddress in gAddresses)
            {
                //gAddress.Source = "Stakeholder";
                gAddress.Country = "India";
                //gAddress.SourceId = stakeholders.Id;
            }
            return gAddresses;
        }

        private static void SetRegistration(Stakeholders stakeholders)
        {
            if (stakeholders.StkhRegistrations.Any())
            {
                foreach (var stkhRegistration in stakeholders.StkhRegistrations)
                {
                    stkhRegistration.Stakeholder = stakeholders;
                }
            }
        }

        private static void SetPayment(Stakeholders stakeholders)
        {
            if (stakeholders.StkhPayments.Any())
            {
                foreach (var stkhPayment in stakeholders.StkhPayments)
                {
                    stkhPayment.Stakeholder = stakeholders;
                }
            }
        }

        private static void SetWorking(Stakeholders stakeholders)
        {
            if (stakeholders.StkhWorkings.Any())
            {
                foreach (var gWorking in stakeholders.StkhWorkings)
                {
                    gWorking.Stakeholder = stakeholders;
                }
            }
        }
        #endregion
    }
}