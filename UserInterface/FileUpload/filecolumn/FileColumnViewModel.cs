using System;
using System.Collections.Generic;
using System.Linq;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.FileUploader;
using ColloSys.DataLayer.Infra.SessionMgr;

namespace ColloSys.UserInterface.Areas.FileUploader.Models
{
    public class FileColumnViewModel
    {
        public FileColumnViewModel()
        {
            ColumnDataTypes = Enum.GetNames(typeof (ColloSysEnums.FileDataType));

            var session = SessionManager.GetCurrentSession();

            var data = session.QueryOver<FileDetail>()
                              .Where(x => x.ScbSystems == ScbEnums.ScbSystems.EBBS || x.ScbSystems == ScbEnums.ScbSystems.RLS)
                              .Select(x => x.AliasName)
                              .OrderBy(x=>x.AliasName).Asc
                              .List<ColloSysEnums.FileAliasName>();
            FileNames = data.Select(aliasName => Enum.GetName(typeof (ColloSysEnums.FileAliasName), aliasName))
                            .Distinct()
                            .ToList();

        }

        public IEnumerable<string> ColumnDataTypes;
        public IEnumerable<string> FileNames;
    }
}