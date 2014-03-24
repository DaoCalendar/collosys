#region references

using ColloSys.DataLayer.Domain;
using ColloSys.QueryBuilder.BaseTypes;
using NHibernate.Criterion;

#endregion


namespace ColloSys.QueryBuilder.FileUploadBuilder
{
    public class FileStatusBuilder : Repository<FileStatus>
    {
        public override QueryOver<FileStatus, FileStatus> WithRelation()
        {
            return QueryOver.Of<FileStatus>();
        }
    }
}