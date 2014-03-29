#region references

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Configuration;
using System.Net.Mail;
using NLog;

#endregion

namespace ColloSys.Shared.ConfigSectionReader
{
    public class ColloSysParam : BaseColloSysParam
    {
        private static ColloSysParam _webParams;
        public static ColloSysParam WebParams
        {
            get { return _webParams ?? (_webParams = new ColloSysParam()); }
        }

        #region Log Path

        private string _logPath;
        public string LogPath
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_logPath))
                    return _logPath;

                var param = AppParams.SingleOrDefault(x => x.Name == "LogPath");
                if (param == null || string.IsNullOrWhiteSpace(param.Value))
                {
                    _logPath = "~/log/";
                }
                else
                {
                    _logPath = param.Value;
                }

                _logPath = CreateDirectory(_logPath);

                return _logPath;
            }
        }

        private string CreateDirectory(string dirPath)
        {
            if (!Path.IsPathRooted(dirPath))
            {
                dirPath = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + dirPath);
            }

            if (!Directory.Exists(dirPath))
            {
                try
                {
                    Directory.CreateDirectory(dirPath);
                }
                catch (Exception)
                {
                    throw new InvalidDataException("Please provider path with write access. Path : " + dirPath);
                }
            }

            return dirPath;
        }

        #endregion

        #region Upload Path

        private string _uploadPath;
        public string UploadPath
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_uploadPath))
                    return _uploadPath;

                var param = AppParams.SingleOrDefault(x => x.Name == "UploadPath");
                if (param == null || string.IsNullOrWhiteSpace(param.Value))
                {
                    _uploadPath = "~/upload/";
                }
                else
                {
                    _uploadPath = param.Value;
                }

                _uploadPath = CreateDirectory(_uploadPath);

                return _uploadPath;
            }
        }

        #endregion

        #region Upload Start Time

        private string _uploadStartTime;
        public DateTime UploadStartTime
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_uploadStartTime))
                {
                    var param = AppParams.SingleOrDefault(x => x.Name == "UploadStartTime");
                    if (param == null || string.IsNullOrWhiteSpace(param.Value))
                    {
                        _uploadStartTime = "19:30";
                    }
                    else
                    {
                        _uploadStartTime = param.Value;
                    }
                }

                var time = _uploadStartTime.Split(':');
                var date = DateTime.Today;
                date = date.AddHours(date.Hour * -1);
                date = date.AddMinutes(date.Minute * -1);
                date = date.AddSeconds(date.Second * -1);

                int hours;
                int mins;
                try
                {
                    hours = Convert.ToUInt16(time[0]);
                    mins = Convert.ToUInt16(time[1]);
                    if (hours > 23 || hours < 0 || mins < 0 || mins > 60)
                        throw new InvalidDataException("Please provide valid date time.");
                }
                catch
                {
                    hours = 19;
                    mins = 30;
                }

                date = date.AddHours(hours);
                date = date.AddMinutes(mins);
                return date;
            }
        }

        #endregion

        #region logging level

        private bool? _showDebugLogs;
        public LogLevel LogLevel
        {
            get
            {
                if (_showDebugLogs != null)
                {
                    return _showDebugLogs.Value ? LogLevel.Debug : LogLevel.Info;
                }

                var param = AppParams.SingleOrDefault(x => x.Name == "ShowDebugLogs");
                if (param == null || string.IsNullOrWhiteSpace(param.Value))
                {
                    _showDebugLogs = true;
                }
                else
                {
                    try
                    {
                        _showDebugLogs = Convert.ToBoolean(param.Value);
                    }
                    catch (Exception)
                    {
                        _showDebugLogs = true;
                    }
                }

                return _showDebugLogs.Value ? LogLevel.Debug : LogLevel.Info;
            }
        }

        #endregion

        #region Use Inmemory

        private bool _useInmemory;
        public bool UseInmemory
        {
            get
            {
                var param = AppParams.SingleOrDefault(x => x.Name == "UseInmemory");
                if (param == null || string.IsNullOrWhiteSpace(param.Value))
                {
                    _useInmemory = false;
                }
                else
                {
                    try
                    {
                        _useInmemory = Convert.ToBoolean(param.Value);
                    }
                    catch (Exception)
                    {
                        _useInmemory = false;
                    }
                }

                return _useInmemory;
            }
        }

        #endregion

        #region Allocation Date

        private DateTime? _allocDate;

        public DateTime AllocationDate
        {
            get
            {
                if (_allocDate != null)
                {
                    return _allocDate.Value;
                }
                var param = AppParams.SingleOrDefault(x => x.Name == "AllocationDate");
                if (param == null || string.IsNullOrWhiteSpace(param.Value))
                {
                    _allocDate = DateTime.Today;
                }
                else
                {
                    DateTime dateTime;
                    _allocDate = DateTime.TryParseExact(param.Value, "yyyy/MM/dd", null, DateTimeStyles.None, out dateTime) ? dateTime : DateTime.Today;
                }
                return _allocDate.Value;
            }
        }

        #endregion

        #region Smtp Setting
        private void InitSmtpFromConfig()
        {
            SmtpSection smtpSection;
            var param = AppParams.SingleOrDefault(x => x.Name == "Smtp");
            if (param == null || string.IsNullOrWhiteSpace(param.Value))
            {
                smtpSection = (SmtpSection)Configuration.GetSection("mailSettings/smtp_release");
            }
            else
            {
                smtpSection = (SmtpSection)Configuration
                                                 .GetSection(string.Format("mailSettings/{0}", param.Value));
            }

            _smtpClient = new SmtpClient
            {
                Port = 25,
                Host = smtpSection.Network.Host,
                EnableSsl = false,
                Timeout = 5 * 60 * 1000,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false
            };
        }

        [Conditional("DEBUG")]
        private void InitGmailSmtp()
        {
            _smtpClient = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("algosys.server@gmail.com", "p@55w0rld"),
                EnableSsl = true
            };
        }

        private SmtpClient _smtpClient;
        public SmtpClient SmtpClient
        {
            get
            {
                if (_smtpClient != null)
                {
                    return _smtpClient;
                }

                //debug mode
                InitGmailSmtp();
                if (_smtpClient != null) 
                    return _smtpClient;

                //release mode
                InitSmtpFromConfig();
                return _smtpClient;
            }
        }
        #endregion

        #region SMTP Send Mail max person

        private uint? _emailBatchSize;

        public uint EmailBatchSize
        {
            get
            {
                if (_emailBatchSize != null)
                {
                    return _emailBatchSize.Value;
                }
                var param = AppParams.SingleOrDefault(x => x.Name == "EmailBatchSize");
                if (param == null || string.IsNullOrWhiteSpace(param.Value))
                {
                    _emailBatchSize = 100;
                }
                else
                {
                    uint number;
                    _emailBatchSize = uint.TryParse(param.Value, NumberStyles.Any, null, out number) ? number : 100;
                }
                return _emailBatchSize.Value;
            }
        }

        #endregion
    }
}

