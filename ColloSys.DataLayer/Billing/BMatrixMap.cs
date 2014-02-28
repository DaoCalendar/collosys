#region References

using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;
using NHibernate;

#endregion

namespace ColloSys.DataLayer.Mapping
{
    public class BMatrixMap : EntityMap<BMatrix>
    {
        public BMatrixMap()
        {
            Table("B_MATRIX");

            #region property

            Property(x => x.Name, map => map.UniqueKey("UQ_MATRIX_NAME"));
            Property(x => x.Dimension);
            Property(x => x.Row1DCount);
            Property(x => x.Row1DType);
            Property(x => x.Row1DTypeName);
            Property(x => x.Column2DCount);
            Property(x => x.Column2DType);
            Property(x => x.Column2DTypeName, map => map.NotNullable(false));
            Property(x => x.Row3DCount);
            Property(x => x.Row3DType);
            Property(x => x.Row3DTypeName, map => map.NotNullable(false));
            Property(x => x.Column4DCount);
            Property(x => x.Column4DType);
            Property(x => x.Column4DTypeName, map => map.NotNullable(false));
            Property(x => x.IsPercentile);
            Property(x=>x.ColumnsOperator);
            Property(x=>x.RowsOperator);
            Property(x=>x.MatrixPerType);
            Property(x=>x.MatrixPerTypeName,map=>map.NotNullable(false));

            #endregion

            #region IStatus
            Property(x => x.IsActive);
            Property(x => x.IsInUse);
            #endregion

            #region IProduct

            Property(x => x.Products, map => map.UniqueKey("UQ_MATRIX_NAME"));
            Property(x => x.Category, map => map.UniqueKey("UQ_MATRIX_NAME"));
            #endregion

            #region IApprove
            Property(x => x.ApprovedBy, map => map.NotNullable(false));
            Property(p => p.ApprovedOn);
            Property(x => x.Description, map => map.NotNullable(false));
            Property(x => x.Status);
            Property(x => x.OrigEntityId);
            Property(x => x.RowStatus);

            #endregion

            #region collection

            Set(x => x.BMatricesValues, colmap => { }, map => map.OneToMany(x => { }));

            #endregion
        }
    }
}