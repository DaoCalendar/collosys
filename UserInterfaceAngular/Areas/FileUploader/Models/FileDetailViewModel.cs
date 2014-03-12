#region references

using System;
using System.Collections.Generic;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.FileUploader;
using ColloSys.QueryBuilder.FileUploadBuilder;

#endregion

namespace ColloSys.UserInterface.Areas.FileUploader.Models
{
    public class FileDetailViewModel
    {
        private static readonly FileDetailBuilder FileDetailBuilder=new FileDetailBuilder();
        public FileDetailViewModel()
        {
            FileDetails = FileDetailBuilder.GetAll();
            FileTypes = Enum.GetNames(typeof(ColloSysEnums.FileType));
            FileCategories = Enum.GetNames(typeof(ScbEnums.Category));
            FileAliasNames = Enum.GetNames(typeof(ColloSysEnums.FileAliasName));
            FileFrequencies = Enum.GetNames(typeof(ColloSysEnums.FileFrequency));
            FileSystems = Enum.GetNames(typeof(ScbEnums.ScbSystems));
        }

        // ReSharper disable MemberCanBePrivate.Global
        public IEnumerable<FileDetail> FileDetails;
        public IEnumerable<string> FileTypes;
        public IEnumerable<string> FileCategories;
        public IEnumerable<string> FileAliasNames;
        public IEnumerable<string> FileFrequencies;
        public IEnumerable<string> FileSystems;
        // ReSharper restore MemberCanBePrivate.Global

    }
}