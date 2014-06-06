using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BillingService2.DBLayer;
using ColloSys.DataLayer.Billing;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using NLog.Filters;

namespace BillingService2.ViewModel
{
    public class MatrixCalulater
    {
        //public static void CalculateTwoDimentionMatrix<T>(BMatrix bMatrix, List<T> data)
        //{
        //    var conditionList = GetBConditionsForMatrix(bMatrix);

        //    //decimal amount = 0;
        //    foreach (var bConditions in conditionList)
        //    {
        //        //amount += CustBillViewModelDbLayer.GetBillingSubpolicyAmount(billDetail, bConditions.ToList(), data, traceLogs);
        //        var filterData = FilterData(billDetail, bConditions.ToList(), data, traceLogs);
        //        SetValueInData(billDetail, bConditions.ToList(), filterData);
        //    }

        //    //return amount;
        //}

        public static List<BillTokens> GetBConditionsForMatrix(BMatrix bMatrix)
        {
            var bTokens = new List<BillTokens>();

            var groupId = 0;
            for (int i = 1; i <= bMatrix.Row1DCount; i++)
            {

                for (int j = 1; j <= bMatrix.Column2DCount; j++)
                {
                    var valueBTokens = new List<BillTokens>();

                    // add row conditions
                    valueBTokens.AddRange(GetRowCondition(bMatrix, i, groupId));

                    // add col conditions
                    valueBTokens.AddRange(GetColumnCondition(bMatrix, j, groupId));

                    // add value output
                    valueBTokens.AddRange(GetValueOutput(bMatrix, i, j, groupId));

                    bTokens.AddRange(valueBTokens);
                    groupId++;
                }
            }

            return bTokens;
        }


        private static List<BillTokens> GetRowCondition(BMatrix bMatrix, int rowIndex,int groupid)
        {
            var matrixValueRowHeader = bMatrix.BMatricesValues.Single(x => x.RowNo1D == rowIndex
                                                                           && x.ColumnNo2D == 0);

            var token1 = new BillTokens()
            {
                GroupId = groupid,
                GroupType = "Condition",
                Priority = 0,
                Type = bMatrix.Row1DType.ToString(),
                Value = bMatrix.Row1DTypeName,
            };

            var token2 = new BillTokens()
            {
                GroupId = groupid,
                GroupType = "Condition",
                Priority = 1,
                Type = "Operator",
                Value = matrixValueRowHeader.RowOperator.ToString(),
            };

            var token3 = new BillTokens()
            {
                GroupId = groupid,
                GroupType = "Condition",
                Priority = 2,
                Type = "Value",
                Value = matrixValueRowHeader.Value,
            };

            //ConditionType = ColloSysEnums.ConditionType.Condition,
            //Priority = 1,
            //Ltype = bMatrix.Row1DType,
            //LtypeName = bMatrix.Row1DTypeName,
            //Operator = matrixValueRowHeader.RowOperator,
            //Rtype = ColloSysEnums.PayoutLRType.Value,
            //Rvalue = matrixValueRowHeader.Value,


            if (rowIndex == 1 || bMatrix.Row1DCount == rowIndex || matrixValueRowHeader.RowOperator == ColloSysEnums.Operators.EqualTo)
                return new List<BillTokens>() { token1, token2, token3 };


            var matrixValuePriviousRowHeader = bMatrix.BMatricesValues.Single(x => x.RowNo1D == rowIndex - 1
                                                                           && x.ColumnNo2D == 0);

            var nextOperator = (matrixValueRowHeader.RowOperator == ColloSysEnums.Operators.LessThan)
                              ? ColloSysEnums.Operators.GreaterThanEqualTo
                              : ColloSysEnums.Operators.LessThanEqualTo;

            var token4 = new BillTokens()
            {
                GroupId = groupid,
                GroupType = "Condition",
                Priority = 3,
                Type = bMatrix.Row1DType.ToString(),
                Value = bMatrix.Row1DTypeName,
            };

            var token5 = new BillTokens()
            {
                GroupId = groupid,
                GroupType = "Condition",
                Priority = 4,
                Type = "Operator",
                Value = nextOperator.ToString(),
            };

            var token6 = new BillTokens()
            {
                GroupId = groupid,
                GroupType = "Condition",
                Priority = 5,
                Type = "Value",
                Value = matrixValuePriviousRowHeader.Value,
            };

            //var bcondition2 = new BCondition()
            //{
            //    ConditionType = ColloSysEnums.ConditionType.Condition,
            //    Priority = 2,
            //    Ltype = bMatrix.Row1DType,
            //    LtypeName = bMatrix.Row1DTypeName,
            //    Operator = nextOperator,
            //    Rtype = ColloSysEnums.PayoutLRType.Value,
            //    Rvalue = matrixValuePriviousRowHeader.Value,
            //};

            return new List<BillTokens>() { token1, token2, token3, token4, token5, token6 };
        }

        private static List<BillTokens> GetColumnCondition(BMatrix bMatrix, int colIndex,int groupId)
        {
            var matrixValueColHeader = bMatrix.BMatricesValues.Single(x => x.RowNo1D == 0
                                                                                  && x.ColumnNo2D == colIndex);

            var token1 = new BillTokens()
            {
                GroupId = groupId,
                GroupType = "Condition",
                Priority = 6,
                Type = bMatrix.Column2DType.ToString(),
                Value = bMatrix.Column2DTypeName,
            };

            var token2 = new BillTokens()
            {
                GroupId = groupId,
                GroupType = "Condition",
                Priority = 7,
                Type = "Operator",
                Value = matrixValueColHeader.ColumnOperator.ToString(),
            };

            var token3 = new BillTokens()
            {
                GroupId = groupId,
                GroupType = "Condition",
                Priority = 8,
                Type = "Value",
                Value = matrixValueColHeader.Value,
            };

            //var bcondition1 = new BCondition()
            //{
            //    ConditionType = ColloSysEnums.ConditionType.Condition,
            //    Priority = 3,
            //    Ltype = bMatrix.Column2DType,
            //    LtypeName = bMatrix.Column2DTypeName,
            //    Operator = matrixValueColHeader.ColumnOperator,
            //    Rtype = ColloSysEnums.PayoutLRType.Value,
            //    Rvalue = matrixValueColHeader.Value,
            //};

            if (colIndex == 1 || bMatrix.Column2DCount == colIndex || matrixValueColHeader.ColumnOperator == ColloSysEnums.Operators.EqualTo)
                return new List<BillTokens>() { token1, token2, token3 };


            var matrixValuePriviousColHeader = bMatrix.BMatricesValues.Single(x => x.RowNo1D == 0
                                                                           && x.ColumnNo2D == colIndex - 1);

            var nextOperator = (matrixValueColHeader.ColumnOperator == ColloSysEnums.Operators.LessThan)
                              ? ColloSysEnums.Operators.GreaterThanEqualTo
                              : ColloSysEnums.Operators.LessThanEqualTo;

            var token4 = new BillTokens()
            {
                GroupId = groupId,
                GroupType = "Condition",
                Priority = 9,
                Type = bMatrix.Row1DType.ToString(),
                Value = bMatrix.Row1DTypeName,
            };

            var token5 = new BillTokens()
            {
                GroupId = groupId,
                GroupType = "Condition",
                Priority = 10,
                Type = "Operator",
                Value = nextOperator.ToString(),
            };

            var token6 = new BillTokens()
            {
                GroupId = groupId,
                GroupType = "Condition",
                Priority = 11,
                Type = "Value",
                Value = matrixValuePriviousColHeader.Value,
            };

            //var bcondition2 = new BCondition()
            //{
            //    ConditionType = ColloSysEnums.ConditionType.Condition,
            //    Priority = 4,
            //    Ltype = bMatrix.Column2DType,
            //    LtypeName = bMatrix.Column2DTypeName,
            //    Operator = nextOperator,
            //    Rtype = ColloSysEnums.PayoutLRType.Value,
            //    Rvalue = matrixValuePriviousColHeader.Value,
            //};

            return new List<BillTokens>() { token1, token2, token3, token4, token5, token6 };
        }


        private static List<BillTokens> GetValueOutput(BMatrix bMatrix, int rowIndex, int colIndex,int groupId)
        {
            var matrixValue = bMatrix.BMatricesValues.Single(x => x.RowNo1D == rowIndex
                                                                         && x.ColumnNo2D == colIndex);


            var token1 = new BillTokens()
            {
                GroupId = groupId,
                GroupType = "Output",
                Priority = 12,
                Type = "Value",
                Value = matrixValue.Value,
            };

            //var bcondition1 = new BCondition()
            //{
            //    ConditionType = ColloSysEnums.ConditionType.Output,
            //    Priority = 5,
            //    Rtype = ColloSysEnums.PayoutLRType.Value,
            //    Rvalue = matrixValue.Value.ToString(CultureInfo.InvariantCulture),
            //};

            //var bcondition2 = new BCondition()
            //{
            //    ConditionType = ColloSysEnums.ConditionType.Output,
            //    Operator = ColloSysEnums.Operators.Multiply,
            //    Priority = 6,
            //    Rtype = bMatrix.MatrixPerType,
            //    RtypeName = bMatrix.MatrixPerTypeName,
            //};

            return new List<BillTokens>() { token1 };
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
