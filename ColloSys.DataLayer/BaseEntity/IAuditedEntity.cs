#region references

using System;

#endregion

namespace ColloSys.DataLayer.BaseEntity
{
    public interface IAuditedEntity
    {
        string CreateAction { get; set; }
        string CreatedBy { get; set; }
        DateTime CreatedOn { get; set; }
    }
}