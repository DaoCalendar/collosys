#region references

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Generic;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.Services.Generic;
using ColloSys.DataLayer.Services.Shared;
using ColloSys.Shared.NgGrid;
using ColloSys.UserInterface.Shared.Attributes;

#endregion

namespace ColloSys.UserInterface.Areas.Reporting.apiController
{
    public class GridApiController : ApiController
    {
        [HttpPost]
        [HttpTransaction]
        public HttpResponseMessage FetchGridData(GridQueryParams gridWrapper)
        {
            var data = ClientDataDownloadHelper.GetGridData(gridWrapper);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpPost]
        [HttpTransaction]
        public HttpResponseMessage DownloadGridData(GridQueryParams gridWrapper)
        {
            try
            {
                var fileinfo = ClientDataDownloadHelper.DownloadGridData(gridWrapper);
                return Request.CreateResponse(HttpStatusCode.OK, fileinfo.FullName);
            }
            catch (Exception exception)
            {
                return Request.CreateResponse(HttpStatusCode.ExpectationFailed, exception.Message);
            }
        }

        [HttpPost]
        [HttpTransaction]
        public HttpResponseMessage EmailGridData(GridQueryParams gridWrapper)
        {
            try
            {
                var fileinfo = ClientDataDownloadHelper.DownloadGridData(gridWrapper);
                EmailService.EmailReport(fileinfo, EmailService.GetCurrentUserEmail(), "Report");
                return Request.CreateResponse(HttpStatusCode.OK, string.Empty);
            }
            catch (Exception exception)
            {
                return Request.CreateResponse(HttpStatusCode.ExpectationFailed, exception.Message);
            }
        }

        [HttpPost]
        [HttpTransaction(Persist = true)]
        public HttpResponseMessage SaveReport(ReportsVM report)
        {
            var session = SessionManager.GetCurrentSession();
            report.Params.GridConfig.pagingOptions.currentPage = 1;
            report.Report.User = AuthService.CurrentUser;
            report.Report.EmailId = EmailService.GetCurrentUserEmail();
            report.Report.NextSendingDateTime = CompuateNextDate(report.Report.Frequency, report.Report.FrequencyParam);
            ReportingService.SerializeQueryParams(report);
            session.SaveOrUpdate(report.Report);
            return Request.CreateResponse(HttpStatusCode.Created, report.Report);
        }

        [HttpPost]
        [HttpTransaction]
        public HttpResponseMessage GetReportsContents(GetReportContentParams param)
        {
            var report = ReportingService.FetchReport(param.ReportId);
            var gridInit = new GridInitData(report.Params, report.Report.ScreenName);
            return Request.CreateResponse(HttpStatusCode.OK, gridInit);
        }

        [HttpPost]
        [HttpTransaction(Persist = true)]
        public HttpResponseMessage DeleteReport(GetReportContentParams param)
        {
            var session = SessionManager.GetCurrentSession();
            var entity = session.Load<GReports>(param.ReportId);
            session.Delete(entity);
            return Request.CreateResponse(HttpStatusCode.OK, string.Empty);
        }

        [HttpPost]
        [HttpTransaction]
        public HttpResponseMessage GetReportsList(GetReportListParams param)
        {
            var session = SessionManager.GetCurrentSession();
            var user = HttpContext.Current.User.Identity.Name;
            var reports = session.QueryOver<GReports>()
                                 .Where(x => x.ScreenName == param.ScreenName)
                                 .And(x => x.User == user)
                                 .List();
            return Request.CreateResponse(HttpStatusCode.OK, reports);
        }

        [HttpGet]
        public HttpResponseMessage GetEnumValues(string type, string property)
        {
            var type2 = Type.GetType(type);
            if (type2 == null)
            {
                throw new InvalidProgramException("Property does not exist on given type");
            }
            var prop = type2.GetProperty(property);
            if (!prop.PropertyType.IsEnum)
            {
                throw new InvalidProgramException("Property does not exist on given type");
            }
            IList<string> values = Enum.GetNames(Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            return Request.CreateResponse(HttpStatusCode.OK, values);
        }

        [HttpGet]
        [HttpTransaction]
        public HttpResponseMessage GetStakeholderList()
        {
            var session = SessionManager.GetCurrentSession();
            var stakeholders = session.QueryOver<Stakeholders>()
                                      .Fetch(x => x.Hierarchy).Eager
                                      .Where(x => x.EmailId != string.Empty)
                                      .List<Stakeholders>();

            return Request.CreateResponse(HttpStatusCode.OK, stakeholders);
        }

        #region random helpers

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

        public struct GetReportContentParams
        {
            // ReSharper disable UnusedAutoPropertyAccessor.Global
            public Guid ReportId { get; set; }
            // ReSharper restore UnusedAutoPropertyAccessor.Global
        }

        public struct GetReportListParams
        {
            // ReSharper disable UnusedAutoPropertyAccessor.Global
            public ColloSysEnums.GridScreenName ScreenName { get; set; }
            // ReSharper restore UnusedAutoPropertyAccessor.Global
        }

        #endregion
    }
}
