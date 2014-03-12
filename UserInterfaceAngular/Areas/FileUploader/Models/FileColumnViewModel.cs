#region references

using System;
using System.Collections.Generic;
using System.Linq;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.FileUploader;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.QueryBuilder.FileUploadBuilder;

#endregion


namespace ColloSys.UserInterface.Areas.FileUploader.Models
{
    public class FileColumnViewModel
    {
        private static readonly FileDetailBuilder FileDetailBuilder=new FileDetailBuilder();

        public FileColumnViewModel()
        {
            ColumnDataTypes = Enum.GetNames(typeof (ColloSysEnums.FileDataType));
            var data = FileDetailBuilder.ForROrEAliasNames();
            FileNames = data.Select(aliasName => Enum.GetName(typeof (ColloSysEnums.FileAliasName), aliasName))
                            .Distinct()
                            .ToList();
        }

        public IEnumerable<string> ColumnDataTypes;
        public IEnumerable<string> FileNames;
    }
}