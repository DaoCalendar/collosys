#region references

using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using ColloSys.DataLayer.Allocation;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Generic;
using ColloSys.DataLayer.SessionMgr;
using ColloSys.DataLayer.Stakeholder;
using ColloSys.Shared.Encryption;

#endregion

namespace AngularUI.Developer.generatedb.Excel2Db
{
    public static class InitialData
    {
        public static void InsertData()
        {
            //InsertIntoStakeHierarchy2();
            //InsertIntoGPermission();
            InsertIntoUsers();
            InsertProducts();
            // InsertPincodes();
            InsertAllocPolicies();
            InsertNofications();
        }

        private static void InsertNofications()
        {
            var valueList = Enum.GetValues(typeof (ColloSysEnums.NotificationType));
            var valueArray = (ColloSysEnums.NotificationType[]) (valueList);
            IList<GNotification> notifications = valueArray
                .Select(notify => new GNotification
                {
                    NotificationType = notify,
                    NotifyHierarchy = ColloSysEnums.NotifyHierarchy.Creator,
                    EsclationDays = 15
                }).ToList();

            using (var session = SessionManager.GetNewSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    foreach (var notification in notifications)
                    {
                        session.Save(notification);
                    }
                    tx.Commit();
                }
            }
        }

        private static void InsertAllocPolicies()
        {
            var products = Enum.GetNames(typeof(ScbEnums.Products));
            var removeList = new List<string> { "UNKNOWN", "ANY", "ALL" };
            products = products.Where(x => !removeList.Contains(x)).ToArray();

            using (var session = SessionManager.GetNewSession())
            {
                // alloc policies
                var allocPolicies = (from string product in products
                                     select new AllocPolicy
                                     {
                                         Name = product,
                                         Products = (ScbEnums.Products)Enum.Parse(typeof(ScbEnums.Products), product)
                                     }).ToList();
                using (var tx = session.BeginTransaction())
                {
                    foreach (var allocPolicy in allocPolicies)
                    {
                        session.SaveOrUpdate(allocPolicy);
                    }
                    tx.Commit();
                }
            }
        }
        
        private static void InsertIntoUsers()
        {
            var session = SessionManager.GetCurrentSession();
            var role4 = session.QueryOver<StkhHierarchy>()
                               .Where(x => x.Designation == "SuperAdmin" && x.Hierarchy == "Admin")
                               .SingleOrDefault();

            var user1 = new GUsers
                {
                    Role = role4,
                    ApplicationName = "ColloSys",
                    Email = "collosys@sc.com",
                    FailedPasswordAnswerAttemptCount = 0,
                    FailedPasswordAnswerAttemptWindowStart = SqlDateTime.MinValue.Value,
                    FailedPasswordAttemptCount = 0,
                    FailedPasswordAttemptWindowStart = SqlDateTime.MinValue.Value,
                    IsApproved = true,
                    IsLockedOut = false,
                    LastActivityDate = SqlDateTime.MinValue.Value,
                    LastLockedOutDate = SqlDateTime.MinValue.Value,
                    Password = PasswordUtility.EncryptText("p@55w0rld"),
                    PasswordAnswer = "20140101",
                    PasswordQuestion = "Joining Date?",
                    Username = "1469319",
                    LastLoginDate = DateTime.Now,
                    LastPasswordChangedDate = DateTime.Now
                };

            var user2Stkh = new Stakeholders
            {
                ApprovalStatus = ColloSysEnums.ApproveStatus.Approved,
                ExternalId = "1469319",
                Password = PasswordUtility.EncryptText("p@55w0rld"),
                EmailId = "collosys@sc.com",
                JoiningDate = DateTime.Today,
                MobileNo = "9819696687",
                Name = "Vendor Admin",
                Hierarchy = role4,
            };

            using (var trans = session.BeginTransaction())
            {
                session.SaveOrUpdate(user1);
                //session.SaveOrUpdate(user2);
                session.SaveOrUpdate(user2Stkh);
                trans.Commit();
            }
        }

        private static void InsertProducts()
        {
            var creaditCard = new ProductConfig();
            var autoOd = new ProductConfig();
            var smartCredit = new ProductConfig();
            var personalLoan = new ProductConfig();
            var auto = new ProductConfig();
            var mortgage = new ProductConfig();
            var smelapod = new ProductConfig();
            var bil = new ProductConfig();
            var lap = new ProductConfig();
            var smeMe = new ProductConfig();

            creaditCard.Product = ScbEnums.Products.CC;
            autoOd.Product = ScbEnums.Products.AUTO_OD;
            smartCredit.Product = ScbEnums.Products.SMC;
            personalLoan.Product = ScbEnums.Products.PL;
            auto.Product = ScbEnums.Products.AUTO;
            mortgage.Product = ScbEnums.Products.MORT;
            smelapod.Product = ScbEnums.Products.SME_LAP_OD;
            bil.Product = ScbEnums.Products.SME_BIL;
            lap.Product = ScbEnums.Products.SME_LAP;
            smeMe.Product = ScbEnums.Products.SME_ME;

            var getProductGroup = new Dictionary<ScbEnums.Products, ScbEnums.ScbSystems>
                {
                    {ScbEnums.Products.CC, ScbEnums.ScbSystems.CCMS},
                    {ScbEnums.Products.AUTO_OD, ScbEnums.ScbSystems.EBBS},
                    {ScbEnums.Products.SMC, ScbEnums.ScbSystems.EBBS},
                    {ScbEnums.Products.SME_LAP_OD, ScbEnums.ScbSystems.RLS},
                    {ScbEnums.Products.AUTO, ScbEnums.ScbSystems.RLS},
                    {ScbEnums.Products.PL, ScbEnums.ScbSystems.RLS},
                    {ScbEnums.Products.MORT, ScbEnums.ScbSystems.RLS},
                    {ScbEnums.Products.SME_BIL, ScbEnums.ScbSystems.RLS},
                    {ScbEnums.Products.SME_ME, ScbEnums.ScbSystems.RLS},
                    {ScbEnums.Products.SME_LAP, ScbEnums.ScbSystems.RLS}
                };


            creaditCard.ProductGroup = getProductGroup[creaditCard.Product];
            autoOd.ProductGroup = getProductGroup[autoOd.Product];
            smartCredit.ProductGroup = getProductGroup[smartCredit.Product];
            personalLoan.ProductGroup = getProductGroup[personalLoan.Product];
            auto.ProductGroup = getProductGroup[auto.Product];
            mortgage.ProductGroup = getProductGroup[mortgage.Product];
            smelapod.ProductGroup = getProductGroup[smelapod.Product];
            bil.ProductGroup = getProductGroup[bil.Product];
            lap.ProductGroup = getProductGroup[lap.Product];
            smeMe.ProductGroup = getProductGroup[smeMe.Product];

            creaditCard.AllocationResetStrategy = ColloSysEnums.AllocationPolicy.Cyclewise;
            autoOd.AllocationResetStrategy = ColloSysEnums.AllocationPolicy.Monthly;
            smartCredit.AllocationResetStrategy = ColloSysEnums.AllocationPolicy.Monthly;
            personalLoan.AllocationResetStrategy = ColloSysEnums.AllocationPolicy.Monthly;
            auto.AllocationResetStrategy = ColloSysEnums.AllocationPolicy.Monthly;
            mortgage.AllocationResetStrategy = ColloSysEnums.AllocationPolicy.Monthly;
            smelapod.AllocationResetStrategy = ColloSysEnums.AllocationPolicy.Monthly;
            bil.AllocationResetStrategy = ColloSysEnums.AllocationPolicy.Monthly;
            lap.AllocationResetStrategy = ColloSysEnums.AllocationPolicy.Monthly;
            smeMe.AllocationResetStrategy = ColloSysEnums.AllocationPolicy.Monthly;

            creaditCard.BillingStrategy = ColloSysEnums.BillingPolicy.Monthly;
            autoOd.BillingStrategy = ColloSysEnums.BillingPolicy.Monthly;
            smartCredit.BillingStrategy = ColloSysEnums.BillingPolicy.Monthly;
            personalLoan.BillingStrategy = ColloSysEnums.BillingPolicy.Monthly;
            auto.BillingStrategy = ColloSysEnums.BillingPolicy.Monthly;
            mortgage.BillingStrategy = ColloSysEnums.BillingPolicy.Monthly;
            smelapod.BillingStrategy = ColloSysEnums.BillingPolicy.Monthly;
            lap.BillingStrategy = ColloSysEnums.BillingPolicy.Monthly;
            bil.BillingStrategy = ColloSysEnums.BillingPolicy.Monthly;
            smeMe.BillingStrategy = ColloSysEnums.BillingPolicy.Monthly;

            creaditCard.HasTelecalling = true;
            autoOd.HasTelecalling = true;
            smartCredit.HasTelecalling = true;
            personalLoan.HasTelecalling = true;
            auto.HasTelecalling = true;
            mortgage.HasTelecalling = true;
            smelapod.HasTelecalling = false;
            lap.HasTelecalling = true;
            bil.HasTelecalling = true;
            smeMe.HasTelecalling = false;

            creaditCard.CycleCodes = "[\"1\",\"5\",\"15\",\"25\"]";
            autoOd.CycleCodes = "[\"1\",\"5\",\"15\",\"25\"]";
            smartCredit.CycleCodes = "[\"1\",\"5\",\"15\",\"25\"]";
            personalLoan.CycleCodes = "[\"1\",\"5\",\"15\",\"25\"]";
            auto.CycleCodes = "[\"1\",\"5\",\"15\",\"25\"]";
            mortgage.CycleCodes = "[\"1\",\"5\",\"15\",\"25\"]";
            smelapod.CycleCodes = "[\"1\",\"5\",\"15\",\"25\"]";
            lap.CycleCodes = "[\"1\",\"5\",\"15\",\"25\"]";
            bil.CycleCodes = "[\"1\",\"5\",\"15\",\"25\"]";
            smeMe.CycleCodes = "[\"1\",\"5\",\"15\",\"25\"]";

            creaditCard.FrCutOffDaysCycle = 5;
            autoOd.FrCutOffDaysCycle = 5;
            smartCredit.FrCutOffDaysCycle = 5;
            personalLoan.FrCutOffDaysCycle = 5;
            auto.FrCutOffDaysCycle = 5;
            mortgage.FrCutOffDaysCycle = 5;
            smelapod.FrCutOffDaysCycle = 5;
            lap.FrCutOffDaysCycle = 5;
            bil.FrCutOffDaysCycle = 5;
            smeMe.FrCutOffDaysCycle = 5;

            creaditCard.FrCutOffDaysMonth = 7;
            autoOd.FrCutOffDaysMonth = 7;
            smartCredit.FrCutOffDaysMonth = 7;
            personalLoan.FrCutOffDaysMonth = 7;
            auto.FrCutOffDaysMonth = 7;
            mortgage.FrCutOffDaysMonth = 7;
            smelapod.FrCutOffDaysMonth = 7;
            lap.FrCutOffDaysMonth = 7;
            bil.FrCutOffDaysMonth = 7;
            smeMe.FrCutOffDaysMonth = 7;

            creaditCard.HasWriteOff = true;
            autoOd.HasWriteOff = true;
            smartCredit.HasWriteOff = true;
            smelapod.HasWriteOff = true;
            personalLoan.HasWriteOff = true;
            auto.HasWriteOff = true;
            mortgage.HasWriteOff = false;
            lap.HasWriteOff = false;
            bil.HasWriteOff = false;
            smeMe.HasWriteOff = false;

            creaditCard.LinerTable = ScbEnums.ClientDataTables.CLiner;
            autoOd.LinerTable = ScbEnums.ClientDataTables.ELiner;
            smartCredit.LinerTable = ScbEnums.ClientDataTables.ELiner;
            smelapod.LinerTable = ScbEnums.ClientDataTables.ELiner;
            personalLoan.LinerTable = ScbEnums.ClientDataTables.RLiner;
            auto.LinerTable = ScbEnums.ClientDataTables.RLiner;
            mortgage.LinerTable = ScbEnums.ClientDataTables.RLiner;
            lap.LinerTable = ScbEnums.ClientDataTables.RLiner;
            bil.LinerTable = ScbEnums.ClientDataTables.RLiner;
            smeMe.LinerTable = ScbEnums.ClientDataTables.RLiner;

            creaditCard.WriteoffTable = ScbEnums.ClientDataTables.CWriteoff;
            autoOd.WriteoffTable = ScbEnums.ClientDataTables.EWriteoff;
            smartCredit.WriteoffTable = ScbEnums.ClientDataTables.EWriteoff;
            smelapod.WriteoffTable = ScbEnums.ClientDataTables.EWriteoff;
            personalLoan.WriteoffTable = ScbEnums.ClientDataTables.RWriteoff;
            auto.WriteoffTable = ScbEnums.ClientDataTables.RWriteoff;
            mortgage.WriteoffTable = null;
            lap.WriteoffTable = null;
            bil.WriteoffTable = null;
            smeMe.WriteoffTable = null;

            creaditCard.PaymentTable = ScbEnums.ClientDataTables.Payment;
            autoOd.PaymentTable = ScbEnums.ClientDataTables.Payment;
            smartCredit.PaymentTable = ScbEnums.ClientDataTables.Payment;
            smelapod.PaymentTable = ScbEnums.ClientDataTables.Payment;
            personalLoan.PaymentTable = ScbEnums.ClientDataTables.Payment;
            auto.PaymentTable = ScbEnums.ClientDataTables.Payment;
            mortgage.PaymentTable = ScbEnums.ClientDataTables.Payment;
            lap.PaymentTable = ScbEnums.ClientDataTables.Payment;
            bil.PaymentTable = ScbEnums.ClientDataTables.Payment;
            smeMe.PaymentTable = ScbEnums.ClientDataTables.Payment;

            using (var session = SessionManager.GetCurrentSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    session.SaveOrUpdate(creaditCard);
                    session.SaveOrUpdate(autoOd);
                    session.SaveOrUpdate(smartCredit);
                    session.SaveOrUpdate(personalLoan);
                    session.SaveOrUpdate(auto);
                    session.SaveOrUpdate(mortgage);
                    session.SaveOrUpdate(smelapod);
                    session.SaveOrUpdate(lap);
                    session.SaveOrUpdate(bil);
                    session.SaveOrUpdate(smeMe);

                    tx.Commit();
                }
            }

        }
    }
}

