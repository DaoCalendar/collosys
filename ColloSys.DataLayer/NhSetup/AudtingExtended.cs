#region references

using System;
using NHibernate.Envers.Configuration.Attributes;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

#endregion

namespace ColloSys.DataLayer.NhSetup
{
    [Serializable]
    [RevisionEntity]
    public class EnversAuditInfo  //: DefaultRevisionEntity   
    {
        #region properties
        // ReSharper disable UnusedAutoPropertyAccessor.Global

        [RevisionNumber]
        public virtual long Id { get; set; }

        [RevisionTimestamp]
        public virtual DateTime RevisionDate { get; set; }

        // ReSharper restore UnusedAutoPropertyAccessor.Global
        #endregion

        #region equals/hashcode

        public override bool Equals(object obj)
        {
            var casted = obj as EnversAuditInfo;
            if (casted == null)
                return false;
            return (Id == casted.Id && RevisionDate.Equals(casted.RevisionDate));
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode() ^ RevisionDate.GetHashCode();
        }

        #endregion
    }

    public class EnversAuditInfoMap : ClassMapping<EnversAuditInfo>
    {
        public EnversAuditInfoMap()
        {
            Id(x => x.Id, map => map.Generator(Generators.Native));
            Property(x => x.RevisionDate);
        }
    }

}

//[Serializable]
//EnversAuditInfo  //: DefaultRevisionEntity 
//public class DefaultRevisionEntity
//{
//    [RevisionNumber]
//    public virtual int Id { get; set; }

//    [RevisionTimestamp]
//    public virtual DateTime RevisionDate { get; set; }

//    public override bool Equals(object obj)
//    {
//        if (this == obj) return true;
//        var revisionEntity = obj as DefaultRevisionEntity;
//        if (revisionEntity == null) return false;

//        var that = revisionEntity;

//        if (Id != that.Id) return false;
//        return RevisionDate == that.RevisionDate;
//    }

//    public override int GetHashCode()
//    {
//        var result = Id;
//        result = 31 * result + (int)(((ulong)RevisionDate.Ticks) ^ (((ulong)RevisionDate.Ticks) >> 32));
//        return result;
//    }
//}
