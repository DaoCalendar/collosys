using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BillingService.DBLayer;
using ColloSys.DataLayer.Billing;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;

namespace BillingService.ViewModel
{
    public class MatrixCalulater
    {
        public static decimal CalculateMatrix<T>(BillDetail billDetail, string matrixName, List<T> data, TraceLogs traceLogs)
        {
            var matrix = BMatrixDbLayer.GetMatrix(billDetail.Products, matrixName);

            if (matrix == null)
                return 0;

            var result = CalculateTwoDimentionMatrix<T>(billDetail, matrix, data, traceLogs);
            return result;
        }

        public static decimal CalculateTwoDimentionMatrix<T>(BillDetail billDetail, BMatrix bMatrix, List<T> data, TraceLogs traceLogs)
        {
            var conditionList = GetBConditionsForMatrix(bMatrix);

            decimal amount = 0;
            foreach (var bConditions in conditionList)
            {
                traceLogs.ConditionFor = "Matrix";
                amount += CustBillViewModelDbLayer.GetBillingSubpolicyAmount(billDetail, bConditions.ToList(), data, traceLogs);
            }

            return amount;
        }

        private static List<BCondition[]> GetBConditionsForMatrix(BMatrix bMatrix)
        {
            var bConditions = new List<BCondition[]>();

            for (int i = 1; i <= bMatrix.Row1DCount; i++)
            {

                for (int j = 1; j <= bMatrix.Column2DCount; j++)
                {
                    var valueBConditions = new List<BCondition>();

                    // add row conditions
                    valueBConditions.AddRange(GetRowCondition(bMatrix, i));

                    // add col conditions
                    valueBConditions.AddRange(GetColumnCondition(bMatrix, j));

                    // add value output
                    valueBConditions.AddRange(GetValueOutput(bMatrix, i, j));

                    bConditions.Add(valueBConditions.ToArray());
                }
            }

            return bConditions;
        }


        private static List<BCondition> GetRowCondition(BMatrix bMatrix, int rowIndex)
        {
            var matrixValueRowHeader = bMatrix.BMatricesValues.Single(x => x.RowNo1D == rowIndex
                                                                           && x.ColumnNo2D == 0
                                                                           && x.RowNo3D == 0
                                                                           && x.ColumnNo4D == 0);

            var bcondition1 = new BCondition()
            {
                ConditionType = ColloSysEnums.ConditionType.Condition,
                Priority = 1,
                Ltype = bMatrix.Row1DType,
                LtypeName = bMatrix.Row1DTypeName,
                Operator = matrixValueRowHeader.RowOperator,
                Rtype = ColloSysEnums.PayoutLRType.Value,
                Rvalue = matrixValueRowHeader.Value,
            };

            if (rowIndex == 1 || bMatrix.Row1DCount == rowIndex || matrixValueRowHeader.RowOperator == ColloSysEnums.Operators.EqualTo)
                return new List<BCondition>() { bcondition1 };


            var matrixValuePriviousRowHeader = bMatrix.BMatricesValues.Single(x => x.RowNo1D == rowIndex - 1
                                                                           && x.ColumnNo2D == 0
                                                                           && x.RowNo3D == 0
                                                                           && x.ColumnNo4D == 0);

            var nextOperator = (matrixValueRowHeader.RowOperator == ColloSysEnums.Operators.LessThan)
                              ? ColloSysEnums.Operators.GreaterThanEqualTo
                              : ColloSysEnums.Operators.LessThanEqualTo;

            var bcondition2 = new BCondition()
            {
                ConditionType = ColloSysEnums.ConditionType.Condition,
                Priority = 2,
                Ltype = bMatrix.Row1DType,
                LtypeName = bMatrix.Row1DTypeName,
                Operator = nextOperator,
                Rtype = ColloSysEnums.PayoutLRType.Value,
                Rvalue = matrixValuePriviousRowHeader.Value,
            };

            return new List<BCondition>() { bcondition1, bcondition2 };
        }

        private static List<BCondition> GetColumnCondition(BMatrix bMatrix, int colIndex)
        {
            var matrixValueColHeader = bMatrix.BMatricesValues.Single(x => x.RowNo1D == 0
                                                                                  && x.ColumnNo2D == colIndex
                                                                                  && x.RowNo3D == 0
                                                                                  && x.ColumnNo4D == 0);

            var bcondition1 = new BCondition()
            {
                ConditionType = ColloSysEnums.ConditionType.Condition,
                Priority = 3,
                Ltype = bMatrix.Column2DType,
                LtypeName = bMatrix.Column2DTypeName,
                Operator = matrixValueColHeader.ColumnOperator,
                Rtype = ColloSysEnums.PayoutLRType.Value,
                Rvalue = matrixValueColHeader.Value,
            };

            if (colIndex == 1 || bMatrix.Column2DCount == colIndex || matrixValueColHeader.ColumnOperator == ColloSysEnums.Operators.EqualTo)
                return new List<BCondition>() { bcondition1 };


            var matrixValuePriviousColHeader = bMatrix.BMatricesValues.Single(x => x.RowNo1D == 0
                                                                           && x.ColumnNo2D == colIndex - 1
                                                                           && x.RowNo3D == 0
                                                                           && x.ColumnNo4D == 0);

            var nextOperator = (matrixValueColHeader.ColumnOperator == ColloSysEnums.Operators.LessThan)
                              ? ColloSysEnums.Operators.GreaterThanEqualTo
                              : ColloSysEnums.Operators.LessThanEqualTo;

            var bcondition2 = new BCondition()
            {
                ConditionType = ColloSysEnums.ConditionType.Condition,
                Priority = 4,
                Ltype = bMatrix.Column2DType,
                LtypeName = bMatrix.Column2DTypeName,
                Operator = nextOperator,
                Rtype = ColloSysEnums.PayoutLRType.Value,
                Rvalue = matrixValuePriviousColHeader.Value,
            };

            return new List<BCondition>() { bcondition1, bcondition2 };
        }


        private static List<BCondition> GetValueOutput(BMatrix bMatrix, int rowIndex, int colIndex)
        {
            var matrixValue = bMatrix.BMatricesValues.Single(x => x.RowNo1D == rowIndex
                                                                         && x.ColumnNo2D == colIndex
                                                                         && x.RowNo3D == 0
                                                                         && x.ColumnNo4D == 0);

            var bcondition1 = new BCondition()
            {
                ConditionType = ColloSysEnums.ConditionType.Output,
                Priority = 5,
                Rtype = ColloSysEnums.PayoutLRType.Value,
                Rvalue = matrixValue.Value.ToString(CultureInfo.InvariantCulture),
            };

            var bcondition2 = new BCondition()
            {
                ConditionType = ColloSysEnums.ConditionType.Output,
                Operator = ColloSysEnums.Operators.Multiply,
                Priority = 6,
                Rtype = bMatrix.MatrixPerType,
                RtypeName = bMatrix.MatrixPerTypeName,
            };

            return new List<BCondition>() { bcondition1, bcondition2 };
        }
    }
}




//var matrixValueRowHeader = bMatrix.BMatricesValues.Single(x => x.RowNo1D == i
//                                                     && x.ColumnNo2D == 0
//                                                     && x.RowNo3D == 0
//                                                     && x.ColumnNo4D == 0);
//var bcondition1 = new BCondition()
//    {
//        ConditionType = ColloSysEnums.ConditionType.Condition,
//        Priority = 1,
//        Ltype = bMatrix.Row1DType,
//        LtypeName = bMatrix.Row1DTypeName,
//        Operator = matrixValueRowHeader.RowOperator,
//        Rtype = ColloSysEnums.PayoutLRType.Value,
//        Rvalue = matrixValueRowHeader.Value.ToString(CultureInfo.InvariantCulture),
//    };


//var matrixValueColHeader = bMatrix.BMatricesValues.Single(x => x.RowNo1D == 0
//                                                               && x.ColumnNo2D == j
//                                                               && x.RowNo3D == 0
//                                                               && x.ColumnNo4D == 0);
//var bcondition2 = new BCondition()
//    {
//        ConditionType = ColloSysEnums.ConditionType.Condition,
//        Priority = 2,
//        Ltype = bMatrix.Column2DType,
//        LtypeName = bMatrix.Column2DTypeName,
//        Operator = matrixValueColHeader.ColumnOperator,
//        Rtype = ColloSysEnums.PayoutLRType.Value,
//        Rvalue = matrixValueColHeader.Value.ToString(CultureInfo.InvariantCulture),
//    };
