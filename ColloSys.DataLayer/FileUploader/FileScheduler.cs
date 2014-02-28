#region References
using System;
using System.ComponentModel.DataAnnotations;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.FileUploader;
using Iesi.Collections.Generic;
using NHibernate;

#endregion

namespace ColloSys.DataLayer.Domain
{
    public class FileScheduler : Entity
    {
        #region Relationship

        //public override void MakeEmpty(bool forceEmpty = false)
        //{
        //    if (!NHibernateUtil.IsInitialized(FileStatuss) || forceEmpty) FileStatuss = null;
        //    if (!NHibernateUtil.IsInitialized(CLiners) || forceEmpty) CLiners = null;
        //    if (!NHibernateUtil.IsInitialized(CUnbilleds) || forceEmpty) CUnbilleds = null;
        //    if (!NHibernateUtil.IsInitialized(CPayments) || forceEmpty) CPayments = null;
        //    if (!NHibernateUtil.IsInitialized(CWriteoffs) || forceEmpty) CWriteoffs = null;
        //    if (!NHibernateUtil.IsInitialized(CacsActivities) || forceEmpty) CacsActivities = null;
        //    if (!NHibernateUtil.IsInitialized(ELiners) || forceEmpty) ELiners = null;
        //    if (!NHibernateUtil.IsInitialized(EPayments) || forceEmpty) EPayments = null;
        //    if (!NHibernateUtil.IsInitialized(EWriteoffs) || forceEmpty) EWriteoffs = null;
        //    if (!NHibernateUtil.IsInitialized(RLiners) || forceEmpty) RLiners = null;
        //    if (!NHibernateUtil.IsInitialized(RPayments) || forceEmpty) RPayments = null;
        //    if (!NHibernateUtil.IsInitialized(RWriteoffs) || forceEmpty) RWriteoffs = null;

        //}
        ////Collection
        public virtual ISet<FileStatus> FileStatuss { get; set; }
        public virtual ISet<CLiner> CLiners { get; set; }
        public virtual ISet<CUnbilled> CUnbilleds { get; set; }
        public virtual ISet<CPayment> CPayments { get; set; }
        public virtual ISet<CWriteoff> CWriteoffs { get; set; }
        public virtual ISet<CacsActivity> CacsActivities { get; set; }


        public virtual ISet<ELiner> ELiners { get; set; }
        public virtual ISet<EPayment> EPayments { get; set; }
        public virtual ISet<EWriteoff> EWriteoffs { get; set; }

        public virtual ISet<RLiner> RLiners { get; set; }
        public virtual ISet<RPayment> RPayments { get; set; }
        public virtual ISet<RWriteoff> RWriteoffs { get; set; }

        #endregion

        #region Property

        public virtual FileDetail FileDetail { get; set; }

        public virtual String FileServer { get; set; }

        public virtual String FileDirectory { get; set; }

        public virtual String FileName { get; set; }

        public virtual string FileNameDisplay
        {
            get
            {
                if (string.IsNullOrWhiteSpace(FileName))
                    return string.Empty;
                return FileName.Length < 16 ? FileName : FileName.Substring(16);
            }
        }

        public virtual UInt64 FileSize { get; set; }

        public virtual Boolean IsImmediate { get; set; }

        public virtual ColloSysEnums.UploadStatus UploadStatus { get; set; }

        public virtual string StatusDescription { get; set; }

        public virtual DateTime FileDate { get; set; }

        public virtual UInt64 TotalRows { get; set; }

        public virtual UInt64 ValidRows { get; set; }

        public virtual UInt64 ErrorRows { get; set; }

        public virtual string ImmediateReason { get; set; }

        public virtual DateTime StartDateTime { get; set; }
        public virtual DateTime? EndDateTime { get; set; }

        #endregion

        #region Category Component

        public virtual ScbEnums.ScbSystems ScbSystems { get; set; }

        public virtual ScbEnums.Category Category { get; set; }

        public virtual bool AllocBillDone { get; set; }

        #endregion


    }
}