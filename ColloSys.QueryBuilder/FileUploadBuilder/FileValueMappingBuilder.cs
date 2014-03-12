#region references

using ColloSys.DataLayer.FileUploader;
using ColloSys.QueryBuilder.BaseTypes;
using NHibernate.Criterion;

#endregion

namespace ColloSys.QueryBuilder.FileUploadBuilder
{
    public class FileValueMappingBuilder : QueryBuilder<FileValueMapping>
    {
        public override QueryOver<FileValueMapping, FileValueMapping> DefaultQuery()
        {
            return QueryOver.Of<FileValueMapping>();
        }
    }
}