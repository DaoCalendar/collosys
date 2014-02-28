#region References

using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;

#endregion

namespace ColloSys.DataLayer.Mapping
{
    public class BMatrixValueMap : EntityMap<BMatrixValue>
    {
        public BMatrixValueMap()
        {
            Table("B_MATRIX_VALUES");

            #region property

            Property(x => x.RowNo1D, map => map.UniqueKey("UQ_MATRIX_VALUE"));
            Property(x => x.ColumnNo2D, map => map.UniqueKey("UQ_MATRIX_VALUE"));
            Property(x => x.RowNo3D, map => map.UniqueKey("UQ_MATRIX_VALUE"));
            Property(x => x.ColumnNo4D, map => map.UniqueKey("UQ_MATRIX_VALUE"));
            Property(x => x.Value);
            Property(x=>x.RowOperator);
            Property(x=>x.ColumnOperator);

            #endregion

            #region relationship

            ManyToOne(x => x.BMatrix, map =>
                {
                    map.NotNullable(true);
                    map.UniqueKey("UQ_MATRIX_VALUE");
                });

            #endregion
        }
    }
}