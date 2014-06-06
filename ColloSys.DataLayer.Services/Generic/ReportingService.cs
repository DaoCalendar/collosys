using System;
using ColloSys.DataLayer.Generic;
using ColloSys.DataLayer.SessionMgr;
using ColloSys.Shared.NgGrid;
using Newtonsoft.Json;

namespace ColloSys.DataLayer.Services.Generic
{
    public static class ReportingService
    {
        public static void SerializeQueryParams(ReportsVM report)
        {
            report.Report.ReportJson = JsonConvert.SerializeObject(report.Params);
        }

        public static void DesrializeQueryParams(ReportsVM report)
        {
            report.Params = JsonConvert.DeserializeObject<GridQueryParams>(report.Report.ReportJson);
        }

        public static ReportsVM FetchReport(Guid reportId)
        {
            var session = SessionManager.GetCurrentSession();
            var report = session.QueryOver<GReports>()
                                 .Where(x => x.Id == reportId)
                                 .SingleOrDefault();
            var reportVm = new ReportsVM { Report = report };
            DesrializeQueryParams(reportVm);
            return reportVm;
        }
    }

    public class ReportsVM
    {
        public GReports Report { get; set; }
        public GridQueryParams Params { get; set; }
    }
}
