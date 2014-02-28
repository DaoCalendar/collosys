#region References
using System;
using System.ComponentModel.DataAnnotations;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.FileUploader;
using Iesi.Collections.Generic;
using NHibernate;

#endregion

namespace ColloSys.DataLayer.Domain
{
    public class FileMapping : Entity
    {
        #region Relationship None
        //public override void MakeEmpty(bool forceEmpty = false)
        //{
        //    if (!NHibernateUtil.IsInitialized(FileValueMappings) || forceEmpty) FileValueMappings = null;
        //}

        public virtual ISet<FileValueMapping> FileValueMappings { get; set; }
        #endregion

        #region properties

        public virtual String ActualTable { get; set; }

        public virtual FileDetail FileDetail { get; set; }

        public virtual String ActualColumn { get; set; }

        public virtual UInt32 Position { get; set; }

        public virtual UInt32 OutputPosition { get; set; }

        public virtual String OutputColumnName { get; set; }

        public virtual ColloSysEnums.FileMappingValueType ValueType { get; set; }

        public virtual String TempTable { get; set; }

        public virtual String TempColumn { get; set; }

        public virtual String DefaultValue { get; set; }

        #endregion

        #region Date Range Component
        public virtual DateTime StartDate { get; set; }

        public virtual DateTime? EndDate { get; set; }
        #endregion

    }
}