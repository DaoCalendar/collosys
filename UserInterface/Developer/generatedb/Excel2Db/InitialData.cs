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
            InsertIntoUsers();
            InsertAllocPolicies();
            InsertNofications();
        }

        private static void InsertNofications()
        {
            var valueList = Enum.GetValues(typeof(ColloSysEnums.NotificationType));
            var valueArray = (ColloSysEnums.NotificationType[])(valueList);
            IList<GNotifyConfig> notifications = valueArray
                .Select(notify => new GNotifyConfig
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
    }
}

