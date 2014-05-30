#region references
using System;
using System.Collections.Generic;
using System.Linq;
using ColloSys.DataLayer.Billing;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;

#endregion

namespace ColloSys.QueryBuilder.Test.DataGeneration
{
    // and or : relational & gt, lt : conditional & sum, count , avg : Sql/number
    public class BillTokensDGC
    {
        #region ConditionTokens

        #region number

        public List<BillTokens> GreaterThanTokens()
        {
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Table", Value = "CustBillViewModel.Cycle", Priority = 0, DataType = "number", GroupType = "Condition"},
                new BillTokens {Type = "Operator", Value = "GreaterThan", Priority = 1, DataType = "conditional", GroupType = "Condition"},
                new BillTokens {Type = "Value", Value = "0", Priority = 2, DataType = "number", GroupType = "Condition"}
            };
            return query;
        }

        public IList<BillTokens> SumNGreaterThanTokens()
        {
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Table", Value = "CustBillViewModel.Cycle", Priority = 0, DataType = "number",GroupType = "Condition"},
                new BillTokens {Type = "Operator", Value = "Plus", Priority = 1, DataType = "number",GroupType = "Condition"},
                new BillTokens {Type = "Value", Value = "2", Priority = 2, DataType = "number",GroupType = "Condition"},
                new BillTokens {Type = "Operator", Value = "GreaterThan", Priority = 3, DataType = "conditional",GroupType = "Condition"},
                new BillTokens {Type = "Value", Value = "0", Priority = 4, DataType = "number",GroupType = "Condition"}
            };
            return query;
        }

        public IList<BillTokens> GreaterThanNSumTokens()
        {
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Table", Value = "CustBillViewModel.Cycle", Priority = 0, DataType = "number",GroupType = "Condition"},
                new BillTokens {Type = "Operator", Value = "GreaterThan", Priority = 1, DataType = "conditional",GroupType = "Condition"},
                new BillTokens {Type = "Value", Value = "2", Priority = 2, DataType = "number",GroupType = "Condition"},
                new BillTokens {Type = "Operator", Value = "Plus", Priority = 3, DataType = "number",GroupType = "Condition"},
                new BillTokens {Type = "Value", Value = "0", Priority = 4, DataType = "number",GroupType = "Condition"}
            };
            return query;
        }

        public List<BillTokens> EqualToTokens()
        {
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Table", Value = "CustBillViewModel.Cycle", Priority = 0, DataType = "number", GroupType = "Condition"},
                new BillTokens {Type = "Operator", Value = "EqualTo", Priority = 1, DataType = "conditional", GroupType = "Condition"},
                new BillTokens {Type = "Value", Value = "3", Priority = 2, DataType = "number", GroupType = "Condition"}
            };
            return query;
        }

        public IList<BillTokens> NotEqualToTokens()
        {
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Table", Value = "CustBillViewModel.Cycle", Priority = 0, DataType = "number", GroupType = "Condition"},
                new BillTokens {Type = "Operator", Value = "NotEqualTo", Priority = 1, DataType = "conditional", GroupType = "Condition"},
                new BillTokens {Type = "Value", Value = "2", Priority = 2, DataType = "number", GroupType = "Condition"}
            };
            return query;
        }

        public IList<BillTokens> GreaterThanEqualToTokens()
        {
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Table", Value = "CustBillViewModel.Cycle", Priority = 0, DataType = "number", GroupType = "Condition"},
                new BillTokens {Type = "Operator", Value = "GreaterThanEqualTo", Priority = 1, DataType = "conditional", GroupType = "Condition"},
                new BillTokens {Type = "Value", Value = "3", Priority = 2, DataType = "number", GroupType = "Condition"}
            };
            return query;
        }

        public IList<BillTokens> LessThanTokens()
        {
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Table", Value = "CustBillViewModel.Cycle", Priority = 0, DataType = "number", GroupType = "Condition"},
                new BillTokens {Type = "Operator", Value = "LessThan", Priority = 1, DataType = "conditional", GroupType = "Condition"},
                new BillTokens {Type = "Value", Value = "4", Priority = 2, DataType = "number", GroupType = "Condition"}
            };
            return query;
        }

        public IList<BillTokens> LessThanEqualToTokens()
        {
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Table", Value = "CustBillViewModel.Cycle", Priority = 0, DataType = "number", GroupType = "Condition"},
                new BillTokens {Type = "Operator", Value = "LessThanEqualTo", Priority = 1, DataType = "conditional", GroupType = "Condition"},
                new BillTokens {Type = "Value", Value = "3", Priority = 2, DataType = "number", GroupType = "Condition"}
            };
            return query;
        }

        public IList<BillTokens> SumNLessThanTokens()
        {
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Table", Value = "CustBillViewModel.Cycle", Priority = 0, DataType = "number",GroupType = "Condition"},
                new BillTokens {Type = "Operator", Value = "Plus", Priority = 1, DataType = "number",GroupType = "Condition"},
                new BillTokens {Type = "Value", Value = "2", Priority = 2, DataType = "number",GroupType = "Condition"},
                new BillTokens {Type = "Operator", Value = "LessThan", Priority = 3, DataType = "conditional",GroupType = "Condition"},
                new BillTokens {Type = "Value", Value = "5", Priority = 4, DataType = "number",GroupType = "Condition"}
            };
            return query;
        }

        public IList<BillTokens> LessThanNSumTokens()
        {
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Table", Value = "CustBillViewModel.Cycle", Priority = 0, DataType = "number",GroupType = "Condition"},
                new BillTokens {Type = "Operator", Value = "LessThan", Priority = 1, DataType = "conditional",GroupType = "Condition"},
                new BillTokens {Type = "Value", Value = "0", Priority = 2, DataType = "number",GroupType = "Condition"},
                new BillTokens {Type = "Operator", Value = "Plus", Priority = 3, DataType = "number",GroupType = "Condition"},
                new BillTokens {Type = "Value", Value = "4", Priority = 4, DataType = "number",GroupType = "Condition"}
            };
            return query;
        }

        public IList<BillTokens> TotalAmountRecoveredMultiPlay2PerGraterThenEqual10000_Tokens()
        {
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Table", Value = "CustBillViewModel.TotalAmountRecovered", Priority = 0, DataType = "number"},
                 new BillTokens {Type = "Operator", Value = "Multiply", Priority = 1, DataType = "Arithmetic"},
                  new BillTokens {Type = "Value", Value = "0.02", Priority = 1, DataType = "number"},
                new BillTokens {Type = "Operator", Value = "GreaterThenEqual", Priority = 1, DataType = "conditional"},
                new BillTokens {Type = "Value", Value = "10000", Priority = 2, DataType = "number"}
            };
            return query;
        }

        public IList<BillTokens> TotalAmountRecoveredMultiPlay2PerLessThenEqual10000_Tokens()
        {
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Table", Value = "CustBillViewModel.TotalAmountRecovered", Priority = 0, DataType = "number"},
                 new BillTokens {Type = "Operator", Value = "Multiply", Priority = 1, DataType = "Arithmetic"},
                  new BillTokens {Type = "Value", Value = "0.02", Priority = 1, DataType = "number"},
                new BillTokens {Type = "Operator", Value = "LessThan", Priority = 1, DataType = "conditional"},
                new BillTokens {Type = "Value", Value = "10000", Priority = 2, DataType = "number"}
            };
            return query;
        }

        #endregion

        #region  string

        public IList<BillTokens> EqualTo_String_Tokens()
        {
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Table", Value = "CustBillViewModel.City", Priority = 0, DataType = "string",GroupType = "Condition"},
                new BillTokens {Type = "Operator", Value = "EqualTo", Priority = 1,DataType = "conditional",GroupId = 0, GroupType = "Condition"},
                new BillTokens {Type = "Value", Value = "pune", Priority = 2, DataType = "text",GroupType = "Condition"}
            };
            return query;
        }

        public IList<BillTokens> Not_EqualTo_String_Tokens()
        {
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Table", Value = "CustBillViewModel.City", Priority = 0, DataType = "string",GroupType = "Condition"},
                new BillTokens {Type = "Operator", Value = "NotEqualTo", Priority = 1,DataType = "conditional",GroupId = 0, GroupType = "Condition"},
                new BillTokens {Type = "Value", Value = "pune", Priority = 2, DataType = "text",GroupType = "Condition"}
            };
            return query;
        }

        public IList<BillTokens> Contains_String_Tokens()
        {
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Table", Value = "CustBillViewModel.City", Priority = 0, DataType = "string",GroupType = "Condition"},
                new BillTokens {Type = "Operator", Value = "Contains", Priority = 1,DataType = "conditional",GroupId = 0, GroupType = "Condition"},
                new BillTokens {Type = "Value", Value = "pune", Priority = 2, DataType = "text",GroupType = "Condition"}
            };
            return query;
        }

        public IList<BillTokens> StartsWith_String_Tokens()
        {
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Table", Value = "CustBillViewModel.City", Priority = 0, DataType = "string",GroupType = "Condition"},
                new BillTokens {Type = "Operator", Value = "StartsWith", Priority = 1,DataType = "conditional",GroupId = 0, GroupType = "Condition"},
                new BillTokens {Type = "Value", Value = "p", Priority = 2, DataType = "text",GroupType = "Condition"}
            };
            return query;
        }

        public IList<BillTokens> EndsWith_String_Tokens()
        {
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Table", Value = "CustBillViewModel.City", Priority = 0, DataType = "string",GroupType = "Condition"},
                new BillTokens {Type = "Operator", Value = "EndsWith", Priority = 1,DataType = "conditional",GroupId = 0, GroupType = "Condition"},
                new BillTokens {Type = "Value", Value = "e", Priority = 2, DataType = "text",GroupType = "Condition"}
            };
            return query;
        }

        public IList<BillTokens> NotIn_String_Tokens()
        {
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Table", Value = "CustBillViewModel.City", Priority = 0, DataType = "string",GroupType = "Condition"},
                new BillTokens {Type = "Operator", Value = "NotIn", Priority = 1,DataType = "conditional",GroupId = 0, GroupType = "Condition"},
                new BillTokens {Type = "Value", Value = "mumbai", Priority = 2, DataType = "text",GroupType = "Condition"}
            };
            return query;
        }

        #endregion

        #region date

        public IList<BillTokens> GreaterThan_Date_Tokens()
        {
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Table", Value = "CustBillViewModel.ChargeofDate", Priority = 0, DataType = "date",GroupType = "Condition"},
                new BillTokens {Type = "Operator", Value = "GreaterThan", Priority = 1,DataType = "conditional",GroupId = 0, GroupType = "Condition"},
                new BillTokens {Type = "Value", Value = "01/03/2014", Priority = 2, DataType = "date",GroupType = "Condition"}
            };
            return query;
        }

        public IList<BillTokens> GreaterThan_EqualTo_Date_Tokens()
        {
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Table", Value = "CustBillViewModel.ChargeofDate", Priority = 0, DataType = "date",GroupType = "Condition"},
                new BillTokens {Type = "Operator", Value = "GreaterThanEqualTo", Priority = 1,DataType = "conditional",GroupId = 0, GroupType = "Condition"},
                new BillTokens {Type = "Value", Value = "01/03/2014", Priority = 2, DataType = "date",GroupType = "Condition"}
            };
            return query;
        }

        public IList<BillTokens> LessThan_Date_Tokens()
        {
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Table", Value = "CustBillViewModel.ChargeofDate", Priority = 0, DataType = "date",GroupType = "Condition"},
                new BillTokens {Type = "Operator", Value = "LessThan", Priority = 1,DataType = "conditional",GroupId = 0, GroupType = "Condition"},
                new BillTokens {Type = "Value", Value = "01/03/2014", Priority = 2, DataType = "date",GroupType = "Condition"}
            };
            return query;
        }

        public IList<BillTokens> LessThan_EqualTo_Date_Tokens()
        {
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Table", Value = "CustBillViewModel.ChargeofDate", Priority = 0, DataType = "date",GroupType = "Condition"},
                new BillTokens {Type = "Operator", Value = "LessThanEqualTo", Priority = 1,DataType = "conditional",GroupId = 0, GroupType = "Condition"},
                new BillTokens {Type = "Value", Value = "01/03/2014", Priority = 2, DataType = "date",GroupType = "Condition"}
            };
            return query;
        }

        public IList<BillTokens> EqualTo_Date_Tokens()
        {
            var query = new List<BillTokens>()
            {
                new BillTokens {Type = "Table", Value = "CustBillViewModel.ChargeofDate", Priority = 0, DataType = "date",GroupType = "Condition"},
                new BillTokens {Type = "Operator", Value = "EqualTo", Priority = 1,DataType = "conditional",GroupId = 0, GroupType = "Condition"},
                new BillTokens {Type = "Value", Value = "01/03/2014", Priority = 2, DataType = "date",GroupType = "Condition"}
            };
            return query;
        }

        public IList<BillTokens> Not_EqualTo_Date_Tokens()
        {
            var query = new List<BillTokens>()
            {
                new BillTokens {Type = "Table", Value = "CustBillViewModel.ChargeofDate", Priority = 0, DataType = "date",GroupType = "Condition"},
                new BillTokens {Type = "Operator", Value = "NotEqualTo", Priority = 1,DataType = "conditional",GroupId = 0, GroupType = "Condition"},
                new BillTokens {Type = "Value", Value = "01/03/2014", Priority = 2, DataType = "date",GroupType = "Condition"}
            };
            return query;
        }
        #endregion

        #endregion

        public IList<BillTokens> GreaterThanWithPlus2Tokens()
        {
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Table", Value = "CustBillViewModel.Cycle", Priority = 0, DataType = "number", GroupId = 0,GroupType = "Condition"},
                new BillTokens {Type = "Operator", Value = "EqualTo", Priority = 1, DataType = "conditional",GroupId = 0, GroupType = "Condition"},
                new BillTokens {Type = "Value", Value = "0", Priority = 2, DataType = "number",GroupId = 0, GroupType = "Condition"},

                new BillTokens {Type = "Table", Value = "CustBillViewModel.Cycle", Priority = 0, DataType = "number",GroupId = 0, GroupType = "Output"},
                new BillTokens {Type = "Operator", Value = "Plus", Priority = 1, DataType = "number",GroupId = 0, GroupType = "Output"},
                new BillTokens {Type = "Value", Value = "2", Priority = 2, DataType = "number",GroupId = 0, GroupType = "Output"}
            };
            return query;
        }

        public IList<BillTokens> CityCategoryIsIn_Tokens()
        {
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Table", Value = "CustBillViewModel.CityCategory", Priority = 0, DataType = "enum"},
                new BillTokens {Type = "Operator", Value = "IsIn", Priority = 1, DataType = "conditional"},
                new BillTokens {Type = "Value", Value = "Metro, A", Priority = 2, DataType = "enum"}
            };
            return query;
        }

        public IList<BillTokens> City_CityCategory_Flag_Product_Tokens()
        {
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Table",Value = "CustBillViewModel.City",Priority = 0,DataType = "string", GroupId = 0,GroupType = "Condition"},
                new BillTokens {Type = "Operator", Value = "EqualTo", Priority = 1, DataType = "conditional", GroupId = 0,GroupType = "Condition"},
                new BillTokens {Type = "Value", Value = "Pune", Priority = 2, DataType = "string", GroupId = 0,GroupType = "Condition"},

                new BillTokens {Type = "Operator", Value = "And", Priority = 3, DataType = "relational", GroupId = 0,GroupType = "Condition"},

                new BillTokens {Type = "Table", Value = "CustBillViewModel.CityCategory", Priority = 4, DataType = "string", GroupId = 0,GroupType = "Condition"},
                new BillTokens {Type = "Operator", Value = "EqualTo", Priority = 5, DataType = "conditional", GroupId = 0,GroupType = "Condition"},
                new BillTokens {Type = "Value", Value = "Tier1", Priority = 6, DataType = "string", GroupId = 0,GroupType = "Condition"},

                new BillTokens {Type = "Operator", Value = "And", Priority = 7, DataType = "relational", GroupId = 0,GroupType = "Condition"},

                new BillTokens {Type = "Table", Value = "CustBillViewModel.Flag", Priority = 8, DataType = "string", GroupId = 0,GroupType = "Condition"},
                new BillTokens {Type = "Operator", Value = "EqualTo", Priority = 9, DataType = "conditional", GroupId = 0,GroupType = "Condition"},
                new BillTokens {Type = "Value", Value = "O", Priority = 10, DataType = "string", GroupId = 0,GroupType = "Condition"},

                 new BillTokens {Type = "Operator", Value = "And", Priority = 11, DataType = "relational", GroupId = 0,GroupType = "Condition"},

                new BillTokens {Type = "Table", Value = "CustBillViewModel.Product", Priority = 12, DataType = "string", GroupId = 0,GroupType = "Condition"},
                new BillTokens {Type = "Operator", Value = "EqualTo", Priority = 13, DataType = "conditional", GroupId = 0,GroupType = "Condition"},
                new BillTokens {Type = "Value", Value = "PL", Priority = 14, DataType = "string", GroupId = 0,GroupType = "Condition"}
            };
            return query;
        }

        #region Output Tokens

        public IList<BillTokens> Field_Plus_Value_Tokens()
        {
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Table", Value = "CustBillViewModel.Cycle", Priority = 0, DataType = "number",GroupType = "Output"},
                new BillTokens {Type = "Operator", Value = "Plus", Priority = 1, DataType = "number",GroupType = "Output"},
                new BillTokens {Type = "Value", Value = "2", Priority = 2, DataType = "number",GroupType = "Output"}
            };
            return query;
        }

        public IList<BillTokens> Value_Plus_Field_Tokens()
        {
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Value", Value = "2", Priority = 2, DataType = "number",GroupType = "Output"},
                new BillTokens {Type = "Operator", Value = "Plus", Priority = 1, DataType = "number",GroupType = "Output"},
                new BillTokens {Type = "Table", Value = "CustBillViewModel.Cycle", Priority = 0, DataType = "number",GroupType = "Output"}
            };
            return query;
        }

        public IList<BillTokens> Field_Multiply_By_Value_Tokens()
        {
            var query = new List<BillTokens>
            {
                new BillTokens{Type = "Table",Value = "CustBillViewModel.TotalDueOnAllocation",Priority = 0,DataType = "number",GroupType = "Output"},
                new BillTokens {Type = "Operator", Value = "Multiply", Priority = 1, DataType = "number",GroupType = "Output"},
                new BillTokens {Type = "Value", Value = "0.02", Priority = 1, DataType = "number",GroupType = "Output"}
            };
            return query;
        }

        public IList<BillTokens> Value_Multiply_By_Field_Tokens()
        {
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Value", Value = "0.02", Priority = 0, DataType = "number",GroupType = "Output"},
                new BillTokens {Type = "Operator", Value = "Multiply", Priority = 1, DataType = "number",GroupType = "Output"},
                new BillTokens{Type = "Table",Value = "CustBillViewModel.TotalDueOnAllocation",Priority = 2,DataType = "number",GroupType = "Output"},
            };
            return query;
        }

        public IList<BillTokens> Field_Divide_By_Value_Tokens()
        {
            var query = new List<BillTokens>
            {
                new BillTokens{Type = "Table",Value = "CustBillViewModel.TotalDueOnAllocation",Priority = 0,DataType = "number",GroupType = "Output"},
                new BillTokens {Type = "Operator", Value = "Divide", Priority = 1, DataType = "number",GroupType = "Output"},
                new BillTokens {Type = "Value", Value = "2", Priority = 1, DataType = "number",GroupType = "Output"}
            };
            return query;
        }

        public IList<BillTokens> Value_Divide_By_Field_Tokens()
        {
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Value", Value = "2000", Priority = 0, DataType = "number",GroupType = "Output"},
                new BillTokens {Type = "Operator", Value = "Divide", Priority = 1, DataType = "number",GroupType = "Output"},
                new BillTokens{Type = "Table",Value = "CustBillViewModel.TotalDueOnAllocation",Priority = 2,DataType = "number",GroupType = "Output"}
            };
            return query;
        }

        public IList<BillTokens> Field_Minus_Value_Tokens()
        {
            var query = new List<BillTokens>
            {
                new BillTokens{Type = "Table",Value = "CustBillViewModel.TotalDueOnAllocation",Priority = 0,DataType = "number",GroupType = "Output"},
                new BillTokens {Type = "Operator", Value = "Minus", Priority = 1, DataType = "number",GroupType = "Output"},
                new BillTokens {Type = "Value", Value = "100", Priority = 1, DataType = "number",GroupType = "Output"}
            };
            return query;
        }

        public IList<BillTokens> Value_Minus_Field_Tokens()
        {
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Value", Value = "100", Priority = 0, DataType = "number",GroupType = "Output"},
                new BillTokens {Type = "Operator", Value = "Minus", Priority = 1, DataType = "number",GroupType = "Output"},
                new BillTokens{Type = "Table",Value = "CustBillViewModel.TotalDueOnAllocation",Priority = 2,DataType = "number",GroupType = "Output"},
            };
            return query;
        }

        public IList<BillTokens> Field_Plus_Field_Multiply_By_Value_Tokens()
        {
            // TotalDueOnAllocation + TotalAmountRecovered * 2
            var query = new List<BillTokens>
            {
                new BillTokens{Type = "Table",Value = "CustBillViewModel.TotalDueOnAllocation",Priority = 0,DataType = "number",GroupType = "Output"},
                new BillTokens {Type = "Operator", Value = "Plus", Priority = 1, DataType = "number",GroupType = "Output"},
                new BillTokens{Type = "Table",Value = "CustBillViewModel.TotalAmountRecovered",Priority = 2,DataType = "number",GroupType = "Output"},
                new BillTokens {Type = "Operator", Value = "Multiply", Priority = 3, DataType = "number",GroupType = "Output"},
                new BillTokens {Type = "Value", Value = "2", Priority = 4, DataType = "number",GroupType = "Output"}
            };
            return query;
        }

        public IList<BillTokens> Field_Multiply_By_Value_Plus_Field()
        {
            //TotalDueOnAllocation * 2 + TotalAmountRecovered 
            var query = new List<BillTokens>
            {
                new BillTokens{Type = "Table",Value = "CustBillViewModel.TotalDueOnAllocation",Priority = 0,DataType = "number",GroupType = "Output"},
                new BillTokens {Type = "Operator", Value = "Multiply", Priority = 1, DataType = "number",GroupType = "Output"},
                new BillTokens {Type = "Value", Value = "2", Priority = 2, DataType = "number",GroupType = "Output"},
                new BillTokens {Type = "Operator", Value = "Plus", Priority = 3, DataType = "number",GroupType = "Output"},
                new BillTokens{Type = "Table",Value = "CustBillViewModel.TotalAmountRecovered",Priority = 4,DataType = "number",GroupType = "Output"},
            };
            return query;
        }

        public IList<BillTokens> Field_Multiply_By_Field_Divided_By_Value()
        {
            //TotalDueOnAllocation * TotalAmountRecovered / 2

            var query = new List<BillTokens>
            {
                new BillTokens{Type = "Table",Value = "CustBillViewModel.TotalDueOnAllocation",Priority = 0,DataType = "number",GroupType = "Output"},
                new BillTokens {Type = "Operator", Value = "Multiply", Priority = 1, DataType = "number",GroupType = "Output"},
                new BillTokens{Type = "Table",Value = "CustBillViewModel.TotalAmountRecovered",Priority = 2,DataType = "number",GroupType = "Output"},
                new BillTokens {Type = "Operator", Value = "Divide", Priority = 3, DataType = "number",GroupType = "Output"},
                new BillTokens {Type = "Value", Value = "2", Priority = 4, DataType = "number",GroupType = "Output"},
            };
            return query;
        }

        public IList<BillTokens> Field_Minus_Value_Multiply_By_Value()
        {
            //TotalDueOnAllocation - 100 * 2
            var query = new List<BillTokens>
            {
                new BillTokens{Type = "Table",Value = "CustBillViewModel.TotalDueOnAllocation",Priority = 0,DataType = "number",GroupType = "Output"},
                new BillTokens {Type = "Operator", Value = "Minus", Priority = 1, DataType = "number",GroupType = "Output"},
                new BillTokens {Type = "Value", Value = "100", Priority = 2, DataType = "number",GroupType = "Output"},
                new BillTokens {Type = "Operator", Value = "Multiply", Priority = 3, DataType = "number",GroupType = "Output"},
                new BillTokens {Type = "Value", Value = "2", Priority = 4, DataType = "number",GroupType = "Output"},
            };
            return query;
        }

        #endregion

        public List<CustBillViewModel> GenerateData()
        {
            var list = new List<CustBillViewModel>();
            for (int i = 0; i < 5; i++)
            {
                var j = i + 1;
                list.Add(new CustBillViewModel
                {
                    Cycle = (uint)i,
                    Bucket = (uint)i,
                    TotalAmountRecovered = j * 10000,
                    TotalDueOnAllocation = j * 15000,
                    Product = ScbEnums.Products.PL,
                    CityCategory = ColloSysEnums.CityCategory.Tier1,
                    City = "pune",
                    Flag = ColloSysEnums.DelqFlag.O
                });
            }
            return list;
        }

        #region AndOr

        private IEnumerable<BillTokens> Product_EqualTo_PL()
        {
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Table", Value = "CustBillViewModel.Product", Priority = 0, DataType = "enum", GroupType = "Condition"},
                new BillTokens {Type = "Operator", Value = "EqualTo", Priority = 1, DataType = "conditional", GroupType = "Condition"},
                new BillTokens {Type = "Value", Value = "PL", Priority = 2, DataType = "enum", GroupType = "Condition"}
            };
            return query;
        }

        private IEnumerable<BillTokens> Cycle_GreaterThan_2()
        {
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Table", Value = "CustBillViewModel.Cycle", Priority = 4, DataType = "number", GroupType = "Condition"},
                new BillTokens {Type = "Operator", Value = "GreaterThan", Priority = 5, DataType = "conditional", GroupType = "Condition"},
                new BillTokens {Type = "Value", Value = "2", Priority = 6, DataType = "number", GroupType = "Condition"}
            };
            return query;
        }

        private IEnumerable<BillTokens> TotalDue_LessThan_7000()
        {
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Table", Value = "CustBillViewModel.TotalDueOnAllocation", Priority = 8, DataType = "number", GroupType = "Condition"},
                new BillTokens {Type = "Operator", Value = "LessThan", Priority = 9, DataType = "conditional", GroupType = "Condition"},
                new BillTokens {Type = "Value", Value = "7000", Priority = 10, DataType = "number", GroupType = "Condition"}
            };
            return query;
        }

        private IEnumerable<BillTokens> TotalAmount_GreaterThan_1000()
        {
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Table", Value = "CustBillViewModel.TotalAmountRecovered", Priority = 12, DataType = "number", GroupType = "Condition"},
                new BillTokens {Type = "Operator", Value = "GreaterThan", Priority = 13, DataType = "conditional", GroupType = "Condition"},
                new BillTokens {Type = "Value", Value = "1000", Priority = 14, DataType = "number", GroupType = "Condition"}
            };
            return query;
        }

        private IEnumerable<BillTokens> And_Token(int priority)
        {
            var query = new List<BillTokens>
            {
               new BillTokens {Type = "Operator", Value = "And", Priority = priority, DataType = "relational", GroupId = 0,GroupType = "Condition"},
            };
            return query;
        }

        private IEnumerable<BillTokens> Or_Token(int priority)
        {
            var query = new List<BillTokens>
            {
               new BillTokens {Type = "Operator", Value = "Or", Priority = priority, DataType = "relational", GroupId = 0,GroupType = "Condition"},
            };
            return query;
        }

        public IList<BillTokens> Condition_And_Condition_Tokens()
        {
            //Product=PL And Cycle>2
            return Product_EqualTo_PL()
                .Concat(And_Token(3))
                .Concat(Cycle_GreaterThan_2())
                .ToList();
        }

        public IList<BillTokens> Condition_Or_Condition_Tokens()
        {
            //Product=PL Or Cycle>2
            return Product_EqualTo_PL()
                .Concat(Or_Token(3))
                .Concat(Cycle_GreaterThan_2())
                .ToList();
        }

        public IList<BillTokens> Condition_And_Condition_And_Condition_Tokens()
        {
            //Product=PL And Cycle>2 And TotalDueOnAllocation < 7000 
            return Product_EqualTo_PL()
                .Concat(And_Token(3))
                .Concat(Cycle_GreaterThan_2())
                .Concat(And_Token(7))
                .Concat(TotalDue_LessThan_7000())
                .ToList();
        }

        public IList<BillTokens> Condition_Or_Condition_Or_Condition_Tokens()
        {
            //Product=PL Or Cycle>2 Or TotalDueOnAllocation < 7000
            return Product_EqualTo_PL()
                .Concat(Or_Token(3))
                .Concat(Cycle_GreaterThan_2())
                .Concat(Or_Token(7))
                .Concat(TotalDue_LessThan_7000())
                .ToList();
        }

        public IList<BillTokens> Condition_And_Condition_Or_Condition_Tokens()
        {
            //Product=PL And Cycle>2 Or TotalDueOnAllocation < 7000
            return Product_EqualTo_PL()
                .Concat(And_Token(3))
                .Concat(Cycle_GreaterThan_2())
                .Concat(Or_Token(7))
                .Concat(TotalDue_LessThan_7000())
                .ToList();
        }

        public IList<BillTokens> Condition_Or_Condition_And_Condition_Tokens()
        {
            //Product=PL Or Cycle>2 And TotalDueOnAllocation < 7000
            return Product_EqualTo_PL()
                .Concat(Or_Token(3))
                .Concat(Cycle_GreaterThan_2())
                .Concat(And_Token(7))
                .Concat(TotalDue_LessThan_7000())
                .ToList();
        }

        public IList<BillTokens> Condition_And_Condition_And_Condition_And_Condition()
        {
            //Product=PL And Cycle>2 And TotalDueOnAllocation < 7000 And TotalAmountRecovered > 1000
            return Product_EqualTo_PL()
                .Concat(And_Token(3))
                .Concat(Cycle_GreaterThan_2())
                .Concat(And_Token(7))
                .Concat(TotalDue_LessThan_7000())
                .Concat(And_Token(11))
                .Concat(TotalAmount_GreaterThan_1000())
                .ToList();
        }

        public IList<BillTokens> Condition_Or_Condition_Or_Condition_Or_Condition()
        {
            //Product=PL Or Cycle>2 Or TotalDueOnAllocation < 7000 Or TotalAmountRecovered > 1000
            return Product_EqualTo_PL()
                .Concat(Or_Token(3))
                .Concat(Cycle_GreaterThan_2())
                .Concat(Or_Token(7))
                .Concat(TotalDue_LessThan_7000())
                .Concat(Or_Token(11))
                .Concat(TotalAmount_GreaterThan_1000())
                .ToList();
        }

        public IList<BillTokens> Condition_And_Condition_Or_Condition_And_Condition()
        {
            //Product=PL And Cycle>2 Or TotalDueOnAllocation < 7000 And TotalAmountRecovered > 1000
            return Product_EqualTo_PL()
                .Concat(And_Token(3))
                .Concat(Cycle_GreaterThan_2())
                .Concat(Or_Token(7))
                .Concat(TotalDue_LessThan_7000())
                .Concat(And_Token(11))
                .Concat(TotalAmount_GreaterThan_1000())
                .ToList();
        }

        public IList<BillTokens> Condition_And_Condition_And_Condition_Or_Condition()
        {
            //Product=PL And Cycle>2 And TotalDueOnAllocation < 7000 Or TotalAmountRecovered > 1000
            return Product_EqualTo_PL()
                .Concat(And_Token(3))
                .Concat(Cycle_GreaterThan_2())
                .Concat(And_Token(7))
                .Concat(TotalDue_LessThan_7000())
                .Concat(Or_Token(11))
                .Concat(TotalAmount_GreaterThan_1000())
                .ToList();
        }

        public IList<BillTokens> Condition_Or_Condition_And_Condition_Or_Condition()
        {
            //Product=PL Or Cycle>2 And TotalDueOnAllocation < 7000 Or TotalAmountRecovered > 1000
            return Product_EqualTo_PL()
                .Concat(Or_Token(3))
                .Concat(Cycle_GreaterThan_2())
                .Concat(And_Token(7))
                .Concat(TotalDue_LessThan_7000())
                .Concat(Or_Token(11))
                .Concat(TotalAmount_GreaterThan_1000())
                .ToList();
        }

        public IList<BillTokens> Condition_Or_Condition_Or_Condition_And_Condition()
        {
            //Product=PL Or Cycle>2 Or TotalDueOnAllocation < 7000 And TotalAmountRecovered > 1000
            return Product_EqualTo_PL()
               .Concat(Or_Token(3))
               .Concat(Cycle_GreaterThan_2())
               .Concat(Or_Token(7))
               .Concat(TotalDue_LessThan_7000())
               .Concat(And_Token(11))
               .Concat(TotalAmount_GreaterThan_1000())
               .ToList();
        }

        #endregion


        #region Bill Subpolicy

        public IList<BillTokens> EqualToWithPlas2SubpolicTokens()
        {
            // and or : relational & gt, lt : conditional & sum, count , avg : Sql/number
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Table", Value = "CustBillViewModel.Cycle", Priority = 0, DataType = "number", GroupId = 0,GroupType = "Condition"},
                new BillTokens {Type = "Operator", Value = "EqualTo", Priority = 1, DataType = "conditional",GroupId = 0, GroupType = "Condition"},
                new BillTokens {Type = "Value", Value = "0", Priority = 2, DataType = "number",GroupId = 0, GroupType = "Condition"},

                new BillTokens {Type = "Table", Value = "CustBillViewModel.Cycle", Priority = 0, DataType = "number",GroupId = 0, GroupType = "Output"},
                new BillTokens {Type = "Operator", Value = "Plus", Priority = 1, DataType = "number",GroupId = 0, GroupType = "Output"},
                new BillTokens {Type = "Value", Value = "2", Priority = 2, DataType = "number",GroupId = 0, GroupType = "Output"}
            };
            return query;
        }

        public IList<BillTokens> EqualToWithFormulaCyclePlus2SubpolicTokens()
        {
            // and or : relational & gt, lt : conditional & sum, count , avg : Sql/number
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Table", Value = "CustBillViewModel.Cycle", Priority = 0, DataType = "number", GroupId = 0,GroupType = "Condition"},
                new BillTokens {Type = "Operator", Value = "EqualTo", Priority = 1, DataType = "conditional",GroupId = 0, GroupType = "Condition"},
                new BillTokens {Type = "Value", Value = "0", Priority = 2, DataType = "number",GroupId = 0, GroupType = "Condition"},

                new BillTokens {Type = "Formula", Value = "CyclePlus2", Priority = 0, DataType = "number",GroupId = 0, GroupType = "Output"},
            };
            return query;
        }

        public IList<BillTokens> FormulaCycleGreterThen2AndFormulaCyclePlus2SubpolicTokens()
        {
            // and or : relational & gt, lt : conditional & sum, count , avg : Sql/number
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Formula", Value = "CycleGreterThen2", Priority = 0, DataType = "number", GroupId = 0,GroupType = "Condition"},
            
                new BillTokens {Type = "Formula", Value = "CyclePlus2", Priority = 0, DataType = "number",GroupId = 0, GroupType = "Output"},
            };
            return query;
        }

        #endregion

        #region enum

        public IList<BillTokens> ProductEqualPL_Tokens()
        {
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Table", Value = "CustBillViewModel.Product", Priority = 0, DataType = "enum",GroupType = "Condition"},
                new BillTokens {Type = "Operator", Value = "EqualTo", Priority = 1, DataType = "conditional",GroupType = "Condition"},
                new BillTokens {Type = "Value", Value = "PL", Priority = 2, DataType = "enum",GroupType = "Condition"}
            };
            return query;
        }

        public IList<BillTokens> ProductNotEqualPL_Tokens()
        {
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Table", Value = "CustBillViewModel.Product", Priority = 0, DataType = "enum",GroupType = "Condition"},
                new BillTokens {Type = "Operator", Value = "NotEqualTo", Priority = 1, DataType = "conditional",GroupType = "Condition"},
                new BillTokens {Type = "Value", Value = "PL", Priority = 2, DataType = "enum",GroupType = "Condition"}
            };
            return query;
        }

        #endregion

        #region formual Tokens

        public IList<BillTokens> Formula_within_Formula_Tokens()
        {
            //var formula = new BillingSubpolicy()
            //{
            //    Id = Guid.Parse("0A51C0D0-5351-4C10-9215-A338013A4192"),
            //    Name = "Formula1",
            //    PayoutSubpolicyType = ColloSysEnums.PayoutSubpolicyType.Formula
            //};
            //var tokens = Formula_Output_Tokens();
            //formula.BillTokens = SetFormulaReference(tokens, formula);
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Formula", Value = "0A51C0D0-5351-4C10-9215-A338013A4192", Priority = 0, DataType = "number", GroupType = "Output"},
                new BillTokens {Type = "Operator", Value = "Plus", Priority = 1, DataType = "number", GroupType = "Output"},
                new BillTokens {Type = "Value", Value = "200", Priority = 2, DataType = "number", GroupType = "Output"}
            };
            return query;
        }

        private IList<BillTokens> Formula_Output_Tokens()
        {
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Table", Value = "CustBillViewModel.TotalDueOnAllocation", Priority = 0, DataType = "number", GroupType = "Output"},
                new BillTokens {Type = "Operator", Value = "Plus", Priority = 1, DataType = "number", GroupType = "Output"},
                new BillTokens {Type = "Value", Value = "200", Priority = 2, DataType = "number", GroupType = "Output"}
            };
            return query;
        }

        private IList<BillTokens> SetFormulaReference(IList<BillTokens> tokens, BillingSubpolicy billingSubpolicy)
        {
            foreach (var billTokense in tokens)
            {
                billTokense.BillingSubpolicy = billingSubpolicy;
            }
            return tokens;
        }
        #endregion

        
    }
}
