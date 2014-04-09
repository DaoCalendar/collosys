#region references

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using AngularUI.Shared.apis;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Generic;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.Services.Generic;
using ColloSys.DataLayer.Services.Shared;
using ColloSys.Shared.NgGrid;

#endregion

namespace ColloSys.UserInterface.Areas.Reporting.apiController
{
    public class GridApiController : BaseApiController<GReports>
    {
        [HttpPost]
        public HttpResponseMessage FetchGridData(GridQueryParams gridWrapper)
        {
            var data = ClientDataDownloadHelper.GetGridData(gridWrapper);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpPost]
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
        public HttpResponseMessage EmailGridData(GridQueryParams gridWrapper)
        {
            try
            {
                var fileinfo = ClientDataDownloadHelper.DownloadGridData(gridWrapper);
                EmailService.EmailReport(fileinfo, EmailService.GetUserEmail(GetUsername()), "Report");
                return Request.CreateResponse(HttpStatusCode.OK, string.Empty);
            }
            catch (Exception exception)
            {
                return Request.CreateResponse(HttpStatusCode.ExpectationFailed, exception.Message);
            }
        }

        [HttpPost]        
        public HttpResponseMessage SaveReport(ReportsVM report)
        {
            var session = SessionManager.GetCurrentSession();
            report.Params.GridConfig.pagingOptions.currentPage = 1;
            report.Report.User = GetUsername();
            report.Report.EmailId = EmailService.GetUserEmail(report.Report.User);
            report.Report.NextSendingDateTime = CompuateNextDate(report.Report.Frequency, report.Report.FrequencyParam);
            ReportingService.SerializeQueryParams(report);
            using (var tx = session.BeginTransaction())
            {
                session.SaveOrUpdate(report.Report);
                tx.Commit();                
            }
            return Request.CreateResponse(HttpStatusCode.Created, report.Report);
        }

        [HttpPost]
        public HttpResponseMessage GetReportsContents(GetReportContentParams param)
        {
            var report = ReportingService.FetchReport(param.ReportId);
            var gridInit = new GridInitData(report.Params, report.Report.ScreenName);
            return Request.CreateResponse(HttpStatusCode.OK, gridInit);
        }

        [HttpPost]
        public HttpResponseMessage DeleteReport(GetReportContentParams param)
        {
            var session = SessionManager.GetCurrentSession();
            var entity = session.Load<GReports>(param.ReportId);
            session.Delete(entity);
            return Request.CreateResponse(HttpStatusCode.OK, string.Empty);
        }

        [HttpPost]
        public HttpResponseMessage GetReportsList(GetReportListParams param)
        {
            var session = SessionManager.GetCurrentSession();
            var reports = session.QueryOver<GReports>()
                                 .Where(x => x.ScreenName == param.ScreenName)
                                 .And(x => x.User == GetUsername())
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
        public HttpResponseMessage GetStakeholderList()
        {
            var session = SessionManager.GetCurrentSession();
            var stakeholders = session.QueryOver<Stakeholders>()
                                      .Fetch(x => x.Hierarchy).Eager
                                      .Where(x => x.EmailId != string.Empty)
                                      .List<Stakeholders>();

            return Request.CreateResponse(HttpStatusCode.OK, stakeholders);
        }

        [HttpGet]
        public HttpResponseMessage DownloadFile(string filename)
        {
            filename = filename.Replace("\\\\", "\\").Replace("'", "").Replace("\"","");
            if (!char.IsLetter(filename[0]))
            {
                filename = filename.Substring(2);
            }

            var fileinfo = new FileInfo(filename);
            if (!fileinfo.Exists)
            {
                throw new FileNotFoundException(fileinfo.Name);
            }

            try
            {
                var excelData = File.ReadAllBytes(filename);
                var result = new HttpResponseMessage(HttpStatusCode.OK);
                var stream = new MemoryStream(excelData);
                result.Content = new StreamContent(stream);
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = fileinfo.Name
                };
                return result;
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.ExpectationFailed, ex);
            }
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
