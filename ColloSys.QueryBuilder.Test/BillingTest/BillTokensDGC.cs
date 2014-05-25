using System.Collections.Generic;
using ColloSys.DataLayer.Billing;
using ColloSys.DataLayer.Enumerations;

namespace ColloSys.QueryBuilder.Test.BillingTest
{
    public class BillTokensDGC
    {
        internal IList<BillTokens> GreaterThanWithPlas2Tokens()
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


        public List<BillTokens> GreaterThanTokens()
        {
            // and or : relational & gt, lt : conditional & sum, count , avg : Sql/number
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Table", Value = "CustBillViewModel.Cycle", Priority = 0, DataType = "number", GroupType = "Condition"},
                new BillTokens {Type = "Operator", Value = "GreaterThan", Priority = 1, DataType = "conditional", GroupType = "Condition"},
                new BillTokens {Type = "Value", Value = "0", Priority = 2, DataType = "number", GroupType = "Condition"}
            };
            return query;
        }

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

        internal IList<BillTokens> SumNGreaterThanTokens()
        {
            // and or : relational & gt, lt : conditional & sum, count , avg : Sql/number
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

        internal IList<BillTokens> GreaterThanNSumTokens()
        {
            // and or : relational & gt, lt : conditional & sum, count , avg : Sql/number
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

        // x => x.Product == ScbEnums.Products.PL
        internal IList<BillTokens> ProductEqualPL_Tokens()
        {
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Table", Value = "CustBillViewModel.Product", Priority = 0, DataType = "enum"},
                new BillTokens {Type = "Operator", Value = "Equal", Priority = 1, DataType = "conditional"},
                new BillTokens {Type = "Value", Value = "PL", Priority = 2, DataType = "enum"}
            };
            return query;
        }

        // x => x.CityCategory.IsIn(new object[] { "Metro", "A" })
        internal IList<BillTokens> CityCategoryIsIn_Tokens()
        {
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Table", Value = "CustBillViewModel.CityCategory", Priority = 0, DataType = "enum"},
                new BillTokens {Type = "Operator", Value = "IsIn", Priority = 1, DataType = "conditional"},
                new BillTokens {Type = "Value", Value = "Metro, A", Priority = 2, DataType = "enum"}
            };
            return query;
        }


        // x => x.City == "Pune" && x.CityCategory == ColloSysEnums.CityCategory.Tier1 && x.Flag == ColloSysEnums.DelqFlag.O && x.Product == ScbEnums.Products.PL
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

        // x => (x.TotalAmountRecovered * (decimal)0.02) >= 10000
        internal IList<BillTokens> TotalAmountRecoveredMultiPlay2PerGraterThenEqual10000_Tokens()
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

        #region Output Tokens

        // dataList.ForEach(x => x.TotalDueOnAllocation = (x.TotalAmountRecovered * (decimal)0.02))s
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


        // dataList.ForEach(x => x.ResolutionPercentage = (x.TotalAmountRecovered / x.ResolutionPercentage))
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

        #endregion

    }
}
