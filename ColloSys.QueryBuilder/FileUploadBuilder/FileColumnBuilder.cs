#region references

using System;
using System.Collections.Generic;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.QueryBuilder.BaseTypes;
using ColloSys.QueryBuilder.TransAttributes;
using NHibernate.Criterion;

#endregion

namespace ColloSys.QueryBuilder.FileUploadBuilder
{
    public class FileColumnBuilder : QueryBuilder<FileColumn>
    {
        public override QueryOver<FileColumn, FileColumn> DefaultQuery()
        {
            return QueryOver.Of<FileColumn>();
        }

        [Transaction]
        public IEnumerable<FileColumn> OnFileDetailId(Guid fileDetailId)
        {
            return
                SessionManager.GetCurrentSession()
                              .QueryOver<FileColumn>()
                              .Where(c => c.FileDetail.Id == fileDetailId)
                              .List();
        }
    }

    public class FileMappingBuilder : QueryBuilder<FileMapping>
    {
        public override QueryOver<FileMapping, FileMapping> DefaultQuery()
        {
            return QueryOver.Of<FileMapping>();
        }
    }
}

