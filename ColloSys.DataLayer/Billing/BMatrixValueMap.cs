using ColloSys.DataLayer.BaseEntity;

namespace ColloSys.DataLayer.Billing
{
    public class BMatrixValueMap : EntityMap<BMatrixValue>
    {
        public BMatrixValueMap()
        {
            ManyToOne(x => x.BMatrix, map => { map.NotNullable(true); map.UniqueKey("UQ_MATRIX_VALUE"); });
            Property(x => x.RowOperator);
            Property(x => x.RowNo1D, map => map.UniqueKey("UQ_MATRIX_VALUE"));
            Property(x => x.ColumnOperator);
            Property(x => x.ColumnNo2D, map => map.UniqueKey("UQ_MATRIX_VALUE"));
            Property(x => x.Value);
        }
    }
}