#region references

using System;
using System.Collections.Generic;
using System.Configuration;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Generic;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.NhSetup;
using ColloSys.DataLayer.Services.Generic;
using ColloSys.DataLayer.Services.Shared;
using ColloSys.DataLayer.SessionMgr;
using ColloSys.Shared.ConfigSectionReader;
using NHibernate.Criterion;
using NLog;

#endregion

namespace ColloSys.EMailServices
{
    public class EmailService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #region nh init

        public static readonly ConnectionStringSettings ConnString;
        static EmailService()
        {
            try
            {
                ConnString = ColloSysParam.WebParams.ConnectionString;
                Logger.Info(string.Format("EmailService: Connection String : {0}", ConnString.ConnectionString));
                SessionManager.InitNhibernate(new NhInitParams
                    {
                        ConnectionString = ConnString,
                        DbType = ConfiguredDbTypes.MsSql,
                        IsWeb = false
                    });
            }
            catch (Exception ex)
            {
                Logger.Error("EmailService : " + ex);
            }
        }

        #endregion

        public void EmailReports()
        {
            try
            {
                SessionManager.BindNewSession();

                var session = SessionManager.GetCurrentSession();

                var gReports =
                    session.QueryOver<GReports>().Where(x => x.NextSendingDateTime < DateTime.Now).List<GReports>();

                foreach (var gReport in gReports)
                {
                    var report = ReportingService.FetchReport(gReport.Id);
                    var fileinfo = ClientDataDownloadHelper.DownloadGridData(report.Params, report.Report.SendOnlyIfData);
                    if (fileinfo != null)
                    {
                        var subject = string.Format("{0}_{1}_{2}", gReport.User, gReport.Frequency,
                            gReport.NextSendingDateTime.ToString("dd/MM/yy"));
                        var sendEmailList = GetStakeholderEmailIds(gReport);

                        //send mail
                        DataLayer.Services.Shared.EmailService.EmailReport(fileinfo, subject, sendEmailList);

                        //DataLayer.Services.Shared.EmailService.EmailReport(fileinfo, gReport.EmailId, subject);
                        Logger.Info("Send mail to " + gReport.User);
                    }

                    // set next sending datetime
                    gReport.NextSendingDateTime = CompuateNextDate(gReport.Frequency, gReport.FrequencyParam);
                }

                Logger.Info("Send mail to total User : " + gReports.Count);
                SaveOrUpdateData(gReports);
                SessionManager.UnbindSession();
                Logger.Info("Update next datetime in db of users report");
            }
            catch (Exception exception)
            {
                Logger.Error("Error : EmailReports " + exception);
            }
        }

        private DateTime CompuateNextDate(ColloSysEnums.FileFrequency frequency, uint param)
        {
            switch (frequency)
            {
                case ColloSysEnums.FileFrequency.Daily:
                    return DateTime.Today.AddDays(1).AddHours(param);
                case ColloSysEnums.FileFrequency.Monthly:
                    try
                    {
                        return new DateTime(DateTime.Today.Year, DateTime.Today.Month + 1, (int)param);
                    }
                    catch (Exception)
                    {
                        var date = new DateTime(DateTime.Today.Year, DateTime.Today.Month + 1, 1);
                        return date.AddDays(-1);
                    }
                case ColloSysEnums.FileFrequency.Weekly:
                    var tomorrow = DateTime.Today.AddDays(1);
                    var daysUntil = ((int)param - (int)tomorrow.DayOfWeek + 7) % 7;
                    return tomorrow.AddDays(daysUntil);
                default:
                    throw new ArgumentOutOfRangeException("frequency");
            }
        }

        public bool SaveOrUpdateData<TEntity>(IEnumerable<TEntity> data)
           where TEntity : Entity
        {
            using (var session = SessionManager.GetNewSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    foreach (var entity in data)
                    {
                        session.SaveOrUpdate(entity);
                    }

                    tx.Commit();
                }
                return true;
            }
        }

        public List<string> GetStakeholderEmailIds(GReports reports)
        {
            var stakeIds = reports.StakeholderIds.Split(',');
            SessionManager.BindNewSession();
            var session = SessionManager.GetCurrentSession();

            var emaillist = new List<string> {reports.EmailId};

            emaillist.AddRange(session.QueryOver<GReports>()
                                      .Where(Restrictions.In("StakeholdersId", stakeIds))
                                      .Select(x => x.EmailId.ToString())
                                      .List<string>());

            
            return emaillist;
        }
    }
}
