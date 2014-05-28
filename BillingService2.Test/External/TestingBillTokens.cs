using System.Collections.Generic;
using ColloSys.DataLayer.Billing;
using ColloSys.DataLayer.Enumerations;

namespace ColloSys.QueryBuilder.Test.BillingTest
{
    public class TestingBillTokens
    {
        #region Condition Tokens

        /// <summary>
        /// condition : Cycle > 0
        /// </summary>
        public List<BillTokens> GreaterThanTokens()
        {
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Table", Value = "CustBillViewModel.Cycle", Priority = 0, DataType = "number",GroupId = 0,GroupType = "Condition"},
                new BillTokens {Type = "Operator", Value = "GreaterThan", Priority = 1, DataType = "conditional",GroupId = 0, GroupType = "Condition"},
                new BillTokens {Type = "Value", Value = "0", Priority = 2, DataType = "number",GroupId = 0, GroupType = "Condition"}
            };
            return query;
        }

        /// <summary>
        /// condition : Cycle + 2 > 0
        /// </summary>
        internal IList<BillTokens> SumNGreaterThanTokens()
        {
            // and or : relational & gt, lt : conditional & sum, count , avg : Sql/number
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Table", Value = "CustBillViewModel.Cycle", Priority = 0, DataType = "number",GroupId = 0,GroupType = "Condition"},
                new BillTokens {Type = "Operator", Value = "Plus", Priority = 1, DataType = "number",GroupId = 0,GroupType = "Condition"},
                new BillTokens {Type = "Value", Value = "2", Priority = 2, DataType = "number",GroupId = 0,GroupType = "Condition"},
                new BillTokens {Type = "Operator", Value = "GreaterThan", Priority = 3, DataType = "conditional",GroupId = 0,GroupType = "Condition"},
                new BillTokens {Type = "Value", Value = "0", Priority = 4, DataType = "number",GroupId = 0,GroupType = "Condition"}
            };
            return query;
        }

        /// <summary>
        /// condition : Cycle > 2 + 0
        /// </summary>
        internal IList<BillTokens> GreaterThanNSumTokens()
        {
            // and or : relational & gt, lt : conditional & sum, count , avg : Sql/number
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Table", Value = "CustBillViewModel.Cycle", Priority = 0, DataType = "number",GroupId = 0,GroupType = "Condition"},
                new BillTokens {Type = "Operator", Value = "GreaterThan", Priority = 1, DataType = "conditional",GroupId = 0,GroupType = "Condition"},
                new BillTokens {Type = "Value", Value = "2", Priority = 2, DataType = "number",GroupId = 0,GroupType = "Condition"},
                new BillTokens {Type = "Operator", Value = "Plus", Priority = 3, DataType = "number",GroupId = 0,GroupType = "Condition"},
                new BillTokens {Type = "Value", Value = "0", Priority = 4, DataType = "number",GroupId = 0,GroupType = "Condition"}
            };
            return query;
        }

        /// <summary>
        /// condition : Product = PL
        /// </summary>
        internal IList<BillTokens> ProductEqualPL_Tokens()
        {
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Table", Value = "CustBillViewModel.Product", Priority = 0, DataType = "enum",GroupId = 0,GroupType = "Condition"},
                new BillTokens {Type = "Operator", Value = "EqualTo", Priority = 1, DataType = "conditional",GroupId = 0,GroupType = "Condition"},
                new BillTokens {Type = "Value", Value = "PL", Priority = 2, DataType = "string",GroupId = 0,GroupType = "Condition"}
            };
            return query;
        }

        /// <summary>
        /// condition : CityCategory IsIn ("Metro", "A")
        /// </summary>
        internal IList<BillTokens> CityCategoryIsIn_Tokens()
        {
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Table", Value = "CustBillViewModel.CityCategory", Priority = 0, DataType = "enum",GroupId = 0,GroupType = "Condition"},
                new BillTokens {Type = "Operator", Value = "IsIn", Priority = 1, DataType = "conditional",GroupId = 0,GroupType = "Condition"},
                new BillTokens {Type = "Value", Value = "Metro, A", Priority = 2, DataType = "enum",GroupId = 0,GroupType = "Condition"}
            };
            return query;
        }


        /// <summary>
        /// condition : City = "Pune" &&  CityCategory = "Tier1" && Flag = "O" && Product = "PL"
        /// </summary>
        internal IList<BillTokens> City_CityCategory_Flag_Product_Tokens()
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

        /// <summary>
        /// condition : TotalAmountRecovered * 0.02 > 10000
        /// </summary>
        internal IList<BillTokens> TotalAmountRecoveredMultiPlay2PerGraterThenEqual10000_Tokens()
        {
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Table", Value = "CustBillViewModel.TotalAmountRecovered", Priority = 0, DataType = "number",GroupId = 0,GroupType = "Condition"},
                 new BillTokens {Type = "Operator", Value = "Multiply", Priority = 1, DataType = "number",GroupId = 0,GroupType = "Condition"},
                  new BillTokens {Type = "Value", Value = "0.02", Priority = 1, DataType = "number",GroupId = 0,GroupType = "Condition"},
                new BillTokens {Type = "Operator", Value = "GreaterThanEqualTo", Priority = 1, DataType = "conditional",GroupId = 0,GroupType = "Condition"},
                new BillTokens {Type = "Value", Value = "10000", Priority = 2, DataType = "number",GroupId = 0,GroupType = "Condition"}
            };
            return query;
        }

        #endregion

        #region Output Tokens

        /// <summary>
        /// output : Cycle + 2
        /// </summary>
        internal IList<BillTokens> SumOfTwoTokens()
        {
            // and or : relational & gt, lt : conditional & sum, count , avg : Sql/number
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Table", Value = "CustBillViewModel.Cycle", Priority = 0, DataType = "number"},
                new BillTokens {Type = "Operator", Value = "Plus", Priority = 1, DataType = "number"},
                new BillTokens {Type = "Value", Value = "2", Priority = 2, DataType = "number"}
            };
            return query;
        }

        /// <summary>
        /// output : 2 + Cycle
        /// </summary>
        internal IList<BillTokens> SumOfTwoTokensReverse()
        {
            // and or : relational & gt, lt : conditional & sum, count , avg : Sql/number
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Value", Value = "2", Priority = 2, DataType = "number"},
                new BillTokens {Type = "Operator", Value = "Plus", Priority = 1, DataType = "number"},
                new BillTokens {Type = "Table", Value = "CustBillViewModel.Cycle", Priority = 0, DataType = "number"}
            };
            return query;
        }

        /// <summary>
        /// output : TotalAmountRecovered * 0.02
        /// </summary>
        internal IList<BillTokens> TotalAmountRecoveredMultiPlay2Per_Tokens()
        {
            var query = new List<BillTokens>
            {
                new BillTokens{Type = "Table",Value = "CustBillViewModel.TotalAmountRecovered",Priority = 0,DataType = "number"},
                new BillTokens {Type = "Operator", Value = "Multiply", Priority = 1, DataType = "Arithmetic"},
                new BillTokens {Type = "Value", Value = "0.02", Priority = 1, DataType = "number"}
            };
            return query;
        }


        /// <summary>
        /// output : TotalAmountRecovered / ResolutionPercentage
        /// </summary>
        internal IList<BillTokens> TotalAmountRecoveredDivideResolutionPercentage_Tokens()
        {
            var query = new List<BillTokens>
            {
                new BillTokens{Type = "Table",Value = "CustBillViewModel.TotalAmountRecovered",Priority = 0,DataType = "number"},
                new BillTokens {Type = "Operator", Value = "Divide", Priority = 1, DataType = "Arithmetic"},
                new BillTokens {Type = "Value", Value = "CustBillViewModel.ResolutionPercentage", Priority = 1, DataType = "number"}
            };
            return query;
        }

        #endregion

        #region Bill Subpolicy

        internal IList<BillTokens> EqualToWithPlas2SubpolicTokens()
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

        internal IList<BillTokens> EqualToWithFormulaCyclePlus2SubpolicTokens()
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

        internal IList<BillTokens> FormulaCycleGreterThen2AndFormulaCyclePlus2SubpolicTokens()
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

        public List<CustBillViewModel> GenerateData()
        {
            var list = new List<CustBillViewModel>();
            for (int i = 0; i < 5; i++)
            {
                list.Add(new CustBillViewModel
                {
                    Cycle = (uint)i,
                    Bucket = (uint)i,
                    TotalAmountRecovered = i * 10000,
                    TotalDueOnAllocation = i * 15000,
                    Product = ScbEnums.Products.PL,
                    CityCategory = ColloSysEnums.CityCategory.Tier1,
                    City = "pune",
                    Flag = ColloSysEnums.DelqFlag.O
                });
            }
            return list;
        }

    }
}
