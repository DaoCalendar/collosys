#region references

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

#endregion

namespace ColloSys.Shared.ConfigSectionReader
{
    public class BaseColloSysParam
    {
        private static ColloSysParamsSection _config = ConfigurationManager.GetSection("ColloSysParams") as ColloSysParamsSection;
        private ApplicationMode _appMode = ApplicationMode.Release;
        protected readonly IEnumerable<ParamElement> AppParams;
        protected readonly Configuration Configuration;

        #region ctor

        protected BaseColloSysParam()
        {
            if (_config == null)
                _config = ReadColloSysSection();

            _appMode = ApplicationMode.Release;
            IsDebugMode();
            IsTestingMode();

            Configuration = ReadConfiguration();

            switch (_appMode)
            {
                case ApplicationMode.Release:
                    AppParams = _config.ReleaseParams.Cast<ParamElement>();
                    break;
                case ApplicationMode.Testing:
                    AppParams = _config.TestingParams.Cast<ParamElement>();
                    break;
                case ApplicationMode.Debug:
                    AppParams = _config.DebugParams.Cast<ParamElement>();
                    break;
                default:
                    throw new InvalidProgramException("Invalid ColloSys Param in web Config");
            }
        }

        [Conditional("DEBUG")]
        private void IsDebugMode()
        {
            _appMode = ApplicationMode.Debug;
        }

        [Conditional("TESTING")]
        private void IsTestingMode()
        {
            _appMode = ApplicationMode.Testing;
        }

        private ColloSysParamsSection ReadColloSysSection()
        {
            var configuration = ReadConfiguration();

            var section = configuration.GetSection("ColloSysParams") as ColloSysParamsSection;
            return section;
        }

        private static Configuration ReadConfiguration()
        {
            var appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
            if (appPath == null)
                throw new InvalidDataException("Could not get the config path!!!");
            appPath = appPath.Replace("file:\\", "");
            //var currentDomainPath = AppDomain.CurrentDomain.RelativeSearchPath + "\\" + "ColloSys.config";
            var currentDomainPath = appPath + "\\" + "ColloSys.config";
            var fileMap = new ConfigurationFileMap(currentDomainPath); //Path to your config file
            var configuration = ConfigurationManager.OpenMappedMachineConfiguration(fileMap);
            return configuration;
        }

        #endregion

        #region connection string

        private ConnectionStringSettings _connectionString;
        public ConnectionStringSettings ConnectionString
        {
            get
            {
                if (_connectionString != null)
                {
                    return _connectionString;
                }

                var connStringName = SharedUtils.WindowsAuth.GetLoggedInUserName();
                if (connStringName == "SYSTEM" || _appMode == ApplicationMode.Release)
                {
                    var paramElement = AppParams.SingleOrDefault(x => x.Name == "ConnectionStringName");
                    if (paramElement != null)
                        connStringName = paramElement.Value;
                }

                if (string.IsNullOrWhiteSpace(connStringName))
                {
                    throw new InvalidDataException("Please provider name of ConnectionString for collosys system.");
                }

                try
                {
                    var config = ReadConfiguration().GetSection("ConnStrings") as ConnectionStringsSection;
                    if (config == null)
                        throw new InvalidDataException("Connection String section is not provided.");
                    _connectionString = config.ConnectionStrings[connStringName];
                }
                catch (Exception)
                {
                    throw new InvalidDataException(string.Format("Please provider ConnectionString for name \"{0}\" collosys system.", connStringName));
                }

                return _connectionString;
            }
        }

        #endregion
    }
}