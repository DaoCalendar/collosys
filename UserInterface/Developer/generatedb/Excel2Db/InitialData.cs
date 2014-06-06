#region references

using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using ColloSys.DataLayer.Allocation;
using ColloSys.DataLayer.Billing;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.SessionMgr;
using ColloSys.Shared.Encryption;

#endregion

namespace ColloSys.UserInterface.Areas.Developer.Models.Excel2Db
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

                // billing policies
                var billigpolicies = (from string product in products
                                      select new BillingPolicy
                                      {
                                          Name = product,
                                          Products = (ScbEnums.Products)Enum.Parse(typeof(ScbEnums.Products), product),
                                          Category = ScbEnums.Category.Liner
                                      }).ToList();

                var billingpoliciesWriteOff = (from string product in products
                                               select new BillingPolicy
                                               {
                                                   Name = product + "_" + ScbEnums.Category.WriteOff.ToString(),
                                                   Products = (ScbEnums.Products)Enum.Parse(typeof(ScbEnums.Products), product),
                                                   Category = ScbEnums.Category.WriteOff
                                               }).ToList();

                using (var tx = session.BeginTransaction())
                {
                    foreach (var bpolicy in billigpolicies)
                    {
                        session.SaveOrUpdate(bpolicy);
                    }

                    foreach (var billingPolicy in billingpoliciesWriteOff)
                    {
                        session.SaveOrUpdate(billingPolicy);
                    }
                    tx.Commit();
                }
            }
        }

        //private static void InsertIntoGPermission()
        //{
        //    var session = SessionManager.GetCurrentSession();
        //        var roleHoc = session.QueryOver<StkhHierarchy>()
        //                         .Where(x => x.Designation == "HOC" && x.Hierarchy == "Field")
        //                         .SingleOrDefault();

        //    var perm = new GPermission()
        //        {
        //            Role = roleHoc,
        //            Activity = ColloSysEnums.Activities.Billing,

        //        };

        //}

        //private static void InsertIntoGPermission()
        //{
        //    var session = SessionManager.GetCurrentSession();

        //    #region Role For Field

        //    var roleHoc = session.QueryOver<StkhHierarchy>()
        //                         .Where(x => x.Designation == "HOC" && x.Hierarchy == "Field")
        //                         .SingleOrDefault();
        //    var roleNcm = session.QueryOver<StkhHierarchy>()
        //                         .Where(x => x.Designation == "NCM" && x.Hierarchy == "Field")
        //                         .SingleOrDefault();
        //    var roleRcm = session.QueryOver<StkhHierarchy>()
        //                         .Where(x => x.Designation == "RCM" && x.Hierarchy == "Field")
        //                         .SingleOrDefault();
        //    var roleDeveloper = session.QueryOver<StkhHierarchy>()
        //                               .Where(x => x.Designation == "Developer" && x.Hierarchy == "Developer")
        //                               .SingleOrDefault();
        //    var roleClusterManager = session.QueryOver<StkhHierarchy>()
        //                                    .Where(x => x.Designation == "ClusterManager" && x.Hierarchy == "Field")
        //                                    .SingleOrDefault();
        //    var roleLocationManager = session.QueryOver<StkhHierarchy>()
        //                                     .Where(x => x.Designation == "LocationManager" && x.Hierarchy == "Field")
        //                                     .SingleOrDefault();
        //    var roleFieldCollector = session.QueryOver<StkhHierarchy>()
        //                                    .Where(x => x.Designation == "Collector" && x.Hierarchy == "Field")
        //                                    .SingleOrDefault();
        //    var roleAgencySupervisor = session.QueryOver<StkhHierarchy>()
        //                                      .Where(x => x.Designation == "AgencySupervisor" && x.Hierarchy == "Field")
        //                                      .SingleOrDefault();

        //    #endregion

        //    #region Role for Telecalling
        //    var roleHubManager = session.QueryOver<StkhHierarchy>()
        //                                      .Where(x => x.Designation == "HubManager" && x.Hierarchy == "Telecalling")
        //                                      .SingleOrDefault();
        //    var roleTeleManager = session.QueryOver<StkhHierarchy>()
        //                                      .Where(x => x.Designation == "Manager" && x.Hierarchy == "Telecalling")
        //                                      .SingleOrDefault();
        //    var roleTelecallingSupervisor = session.QueryOver<StkhHierarchy>()
        //                                      .Where(x => x.Designation == "TelecallingSupervisor" && x.Hierarchy == "Telecalling")
        //                                      .SingleOrDefault();
        //    var roleTelecaller = session.QueryOver<StkhHierarchy>()
        //                                      .Where(x => x.Designation == "Telecaller" && x.Hierarchy == "Telecalling")
        //                                      .SingleOrDefault();
        //    #endregion

        //    #region Role For BackOffice
        //    var roleBackNcm = session.QueryOver<StkhHierarchy>()
        //                                      .Where(x => x.Designation == "NCM" && x.Hierarchy == "BackOffice")
        //                                      .SingleOrDefault();
        //    var rolebackManager = session.QueryOver<StkhHierarchy>()
        //                                      .Where(x => x.Designation == "Manager" && x.Hierarchy == "BackOffice")
        //                                      .SingleOrDefault();
        //    var roleOfficer = session.QueryOver<StkhHierarchy>()
        //                                      .Where(x => x.Designation == "Officer" && x.Hierarchy == "BackOffice")
        //                                      .SingleOrDefault();
        //    var roleAsstMnager = session.QueryOver<StkhHierarchy>()
        //                                      .Where(x => x.Designation == "AsstManager" && x.Hierarchy == "BackOffice")
        //                                      .SingleOrDefault();
        //    #endregion

        //    #region Permission for Field

        //    var permission1 = new GPermission
        //        {
        //            EscalationDays = 5,
        //            Activity = ColloSysEnums.Activities.Allocation,
        //            Permission = ColloSysEnums.Permissions.Approve,
        //            Role = roleHoc
        //        };
        //    var permission2 = new GPermission
        //        {
        //            EscalationDays = 5,
        //            Activity = ColloSysEnums.Activities.Stakeholder,
        //            Permission = ColloSysEnums.Permissions.Approve,
        //            Role = roleNcm
        //        };
        //    var permission3 = new GPermission
        //        {
        //            EscalationDays = 5,
        //            Activity = ColloSysEnums.Activities.Allocation,
        //            Permission = ColloSysEnums.Permissions.Approve,
        //            Role = roleRcm
        //        };
        //    var permission5 = new GPermission
        //        {
        //            EscalationDays = 5,
        //            Activity = ColloSysEnums.Activities.FileUploader,
        //            Permission = ColloSysEnums.Permissions.Approve,
        //            Role = roleNcm
        //        };
        //    var permission6 = new GPermission
        //        {
        //            EscalationDays = 5,
        //            Activity = ColloSysEnums.Activities.FileApproval,
        //            Permission = ColloSysEnums.Permissions.Approve,
        //            Role = roleNcm
        //        };
        //    var permission7 = new GPermission
        //        {
        //            EscalationDays = 5,
        //            Activity = ColloSysEnums.Activities.Development,
        //            Permission = ColloSysEnums.Permissions.Approve,
        //            Role = roleDeveloper
        //        };
        //    var permission8 = new GPermission
        //        {
        //            EscalationDays = 5,
        //            Activity = ColloSysEnums.Activities.FileUploader,
        //            Permission = ColloSysEnums.Permissions.Approve,
        //            Role = roleClusterManager
        //        };
        //    var permission9 = new GPermission
        //        {
        //            EscalationDays = 5,
        //            Activity = ColloSysEnums.Activities.FileUploader,
        //            Permission = ColloSysEnums.Permissions.Approve,
        //            Role = roleLocationManager
        //        };
        //    var permission10 = new GPermission
        //        {
        //            EscalationDays = 5,
        //            Activity = ColloSysEnums.Activities.FileUploader,
        //            Permission = ColloSysEnums.Permissions.Approve,
        //            Role = roleFieldCollector
        //        };
        //    var permission11 = new GPermission
        //        {
        //            EscalationDays = 5,
        //            Activity = ColloSysEnums.Activities.FileUploader,
        //            Permission = ColloSysEnums.Permissions.Approve,
        //            Role = roleAgencySupervisor
        //        };

        //    #endregion

        //    #region Permission for Telecalling

        //    var permission12 = new GPermission
        //        {
        //            EscalationDays = 5,
        //            Activity = ColloSysEnums.Activities.FileUploader,
        //            Permission = ColloSysEnums.Permissions.Approve,
        //            Role = roleHubManager
        //        };
        //    var permission13 = new GPermission
        //        {
        //            EscalationDays = 5,
        //            Activity = ColloSysEnums.Activities.FileUploader,
        //            Permission = ColloSysEnums.Permissions.Approve,
        //            Role = roleTeleManager
        //        };
        //    var permission14 = new GPermission
        //        {
        //            EscalationDays = 5,
        //            Activity = ColloSysEnums.Activities.FileUploader,
        //            Permission = ColloSysEnums.Permissions.Approve,
        //            Role = roleTelecallingSupervisor
        //        };
        //    var permission15 = new GPermission
        //        {
        //            EscalationDays = 5,
        //            Activity = ColloSysEnums.Activities.FileUploader,
        //            Permission = ColloSysEnums.Permissions.Approve,
        //            Role = roleTelecaller
        //        };
        //    #endregion

        //    #region Permission For Backoffice

        //    var permission16 = new GPermission
        //        {
        //            EscalationDays = 5,
        //            Activity = ColloSysEnums.Activities.FileUploader,
        //            Permission = ColloSysEnums.Permissions.Approve,
        //            Role = roleBackNcm
        //        };
        //    var permission17 = new GPermission
        //        {
        //            EscalationDays = 5,
        //            Activity = ColloSysEnums.Activities.FileUploader,
        //            Permission = ColloSysEnums.Permissions.Approve,
        //            Role = rolebackManager
        //        };
        //    var permission18 = new GPermission
        //        {
        //            EscalationDays = 5,
        //            Activity = ColloSysEnums.Activities.FileUploader,
        //            Permission = ColloSysEnums.Permissions.Approve,
        //            Role = roleOfficer
        //        };
        //    var permission19 = new GPermission
        //        {
        //            EscalationDays = 5,
        //            Activity = ColloSysEnums.Activities.FileUploader,
        //            Permission = ColloSysEnums.Permissions.Approve,
        //            Role = roleAsstMnager
        //        };
        //    #endregion

        //    using (var trans = session.BeginTransaction())
        //    {
        //        session.SaveOrUpdate(permission1);
        //        session.SaveOrUpdate(permission2);
        //        session.SaveOrUpdate(permission3);
        //        session.SaveOrUpdate(permission5);
        //        session.SaveOrUpdate(permission6);
        //        session.SaveOrUpdate(permission7);
        //        session.SaveOrUpdate(permission8);
        //        session.SaveOrUpdate(permission9);
        //        session.SaveOrUpdate(permission10);
        //        session.SaveOrUpdate(permission11);
        //        session.SaveOrUpdate(permission12);
        //        session.SaveOrUpdate(permission13);
        //        session.SaveOrUpdate(permission14);
        //        session.SaveOrUpdate(permission14);
        //        session.SaveOrUpdate(permission15);
        //        session.SaveOrUpdate(permission16);
        //        session.SaveOrUpdate(permission17);
        //        session.SaveOrUpdate(permission18);
        //        session.SaveOrUpdate(permission19);

        //        trans.Commit();
        //    }

        //}

        private static void InsertIntoUsers()
        {
            var session = SessionManager.GetCurrentSession();
            //var role1 = session.QueryOver<StakeHierarchy>()
            //           .Where(x => x.Designation == "HOC" && x.ApplicationName == "ColloSys")
            //           .SingleOrDefault();
            var role2 = session.QueryOver<StkhHierarchy>()
                               .Where(x => x.Designation == "Officer" && x.Hierarchy == "BackOffice")
                               .SingleOrDefault();
            //var role3 = session.QueryOver<StakeHierarchy>()
            //           .Where(x => x.Designation == "RCM" && x.ApplicationName == "ColloSys")
            //           .SingleOrDefault();
            var role4 = session.QueryOver<StkhHierarchy>()
                               .Where(x => x.Designation == "Developer")
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
                    PasswordAnswer = "20100101",
                    PasswordQuestion = "Joining Date?",
                    Username = "devadmin",
                };

            var user2 = new GUsers
                {
                    Role = role2,
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
                    Password = PasswordUtility.EncryptText("password"),
                    PasswordAnswer = "20100101",
                    PasswordQuestion = "Joining Date?",
                    Username = "scbuser",
                };

            var user2Stkh = new Stakeholders
            {
                Status = ColloSysEnums.ApproveStatus.Approved,
                ApprovedBy = "System",
                ApprovedOn = DateTime.Now,
                ExternalId = "devadmin",
                Password = PasswordUtility.EncryptText("p@55w0rld"),
                //BirthDate = DateTime.Today.AddYears(-20),
                EmailId = "collosys@sc.com",
                //Gender = ColloSysEnums.Gender.Male,
                JoiningDate = DateTime.Today,
                MobileNo = "9999999999",
                Name = "Developer",
                Hierarchy = role4,
            };

            using (var trans = session.BeginTransaction())
            {
                session.SaveOrUpdate(user1);
                session.SaveOrUpdate(user2);
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


//private static void InsertPincodes()
//{
//    var pincode1 = new GPincode()
//        {
//            Area = "Wakad",
//            City = "Pune",
//            Cluster = "West",
//            Country = "India",
//            District = "Pune",
//            Region = "West",
//            IsInUse = true,
//            Pincode = 411057,
//            State = "MH"
//        };

//    var pincode2 = new GPincode()
//    {
//        Area = "Hadapsar",
//        City = "Pune",
//        Cluster = "West",
//        Country = "India",
//        District = "Pune",
//        Region = "West",
//        IsInUse = true,
//        Pincode = 411028,
//        State = "MH"
//    };

//    var pincode3 = new GPincode()
//    {
//        Area = "Chembur",
//        City = "Mumbai",
//        Cluster = "West",
//        Country = "India",
//        District = "Mumbai",
//        Region = "West",
//        IsInUse = true,
//        Pincode = 400071,
//        State = "MH"
//    };

//    var uow = SessionManager.GetCurrentUnitOfWork();
//    uow.CurrentSession.SaveOrUpdate(pincode1);
//    uow.CurrentSession.SaveOrUpdate(pincode2);
//    uow.CurrentSession.SaveOrUpdate(pincode3);
//    uow.Commit();

//}
