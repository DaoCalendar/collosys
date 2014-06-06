using ColloSys.DataLayer.BaseEntity;

namespace ColloSys.DataLayer.Billing
{
    public class BMatrixMap : EntityMap<BMatrix>
    {
        public BMatrixMap()
        {
            Table("BMatrix");
            Property(x => x.Products, map => map.UniqueKey("UQ_MATRIX_NAME"));
            Property(x => x.Name, map => map.UniqueKey("UQ_MATRIX_NAME"));
            Property(x => x.Dimension);
            Property(x => x.MatrixPerType);
            Property(x => x.MatrixPerTypeName, map => map.NotNullable(false));

            Property(x => x.Row1DCount);
            Property(x => x.Row1DType);
            Property(x => x.Row1DTypeName);
            Property(x => x.Rows1Operator);
            
            Property(x => x.Column2DCount);
            Property(x => x.Column2DType);
            Property(x => x.Column2DTypeName, map => map.NotNullable(false));
            Property(x => x.Columns2Operator);

            Bag(x => x.BMatricesValues, colmap => { }, map => map.OneToMany(x => { }));
        }
    }
}