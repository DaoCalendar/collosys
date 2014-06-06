using ColloSys.DataLayer.BaseEntity;

namespace ColloSys.DataLayer.Domain
{
    public class Reports : Entity
    {
        public virtual string Name { get; set; }

        public virtual string Filter { get; set; }

        public virtual string TableName { get; set; }

        public virtual string Columns { get; set; }

        public virtual string ColumnsFilter { get; set; }
    }
}
