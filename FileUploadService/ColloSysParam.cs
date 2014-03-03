using ColloSys.NLog.AppConfig;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Web;
using System.Web.Configuration;

namespace ColloSys.FileUploadService
{
    public class ColloSysParam : BaseColloSysParam
    {   
        #region Log Path

        private string _logPath;
        public string LogPath
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_logPath))
                    return _logPath;

                var param = AppParams.SingleOrDefault<ParamElement>(x => x.Name == "LogPath");
                if (string.IsNullOrWhiteSpace(param.Value))
                {
                    _logPath = "~/log/";
                }
                else
                {
                    _logPath = param.Value;
                }

                if (!Directory.Exists(_logPath))
                {
                    try
                    {
                        Directory.CreateDirectory(_logPath);
                    }
                    catch (Exception)
                    {
                        throw new InvalidDataException("Please provider proper log path. Log path should have write access for collosys system.");
                    }
                }

                return _logPath;
            }
        }

        #endregion

        #region Use Inmemory

        private bool _useInmemory;
        public bool UseInmemory
        {
            get
            {
                var param = AppParams.SingleOrDefault<ParamElement>(x => x.Name == "UseInmemory");
                if (string.IsNullOrWhiteSpace(param.Value))
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
    }   
}