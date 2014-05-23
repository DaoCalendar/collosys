#region references

using System.Collections.Generic;
using ColloSys.DataLayer.Billing;
using NUnit.Framework;

#endregion

namespace ColloSys.QueryBuilder.Test.BillingTest
{

    public class StringQueryBuilderTest
    {
        private StringQueryBuilder StringQueryBuilder;

        [Test]
        public void Test_Greater_Than_Tokens()
        {
            StringQueryBuilder = new StringQueryBuilder("x");
            var tokensList = DataProvider.Greater_Than_Tokens();
            const string result = "(x.Cycle>0)";
            var query = StringQueryBuilder.GenerateQuery(tokensList);
            Assert.AreEqual(result, query);
        }

        [Test]
        public void Test_Sum_Of_Two_Tokens()
        {
            StringQueryBuilder = new StringQueryBuilder();
            var tokensList = DataProvider.Sum_Of_Two_Tokens();
            const string result = "(Cycle+2)";
            var query = StringQueryBuilder.GenerateQuery(tokensList);
            Assert.AreEqual(result, query);
        }

        [Test]
        public void Test_Sum_Of_Two_Tokens_Reverse()
        {
            StringQueryBuilder = new StringQueryBuilder(string.Empty);
            var tokensList = DataProvider.Sum_Of_Two_Tokens_Reverse();
            const string result = "(2+CustBillViewModel.Cycle)";
            var query = StringQueryBuilder.GenerateQuery(tokensList);
            Assert.AreEqual(result, query);
        }

        [Test]
        public void Test_Sum_and_Greater_Than_Tokens()
        {
            StringQueryBuilder = new StringQueryBuilder(string.Empty);
            var tokensList = DataProvider.Sum_and_Greater_Than_Tokens();
            const string result = "(CustBillViewModel.Cycle+2>0)";
            var query = StringQueryBuilder.GenerateQuery(tokensList);
            Assert.AreEqual(result, query);
        }

        [Test]
        public void Test_Sql_Function_At_Start()
        {
            StringQueryBuilder = new StringQueryBuilder(string.Empty);
            var tokensList = DataProvider.Sql_Function_At_Start();
            const string result = "(Sumof(CustBillViewModel.Cycle)>0)";
            var query = StringQueryBuilder.GenerateQuery(tokensList);
            Assert.AreEqual(result, query);
        }

        [Test]
        public void Test_Sql_Function_At_End()
        {
            StringQueryBuilder = new StringQueryBuilder(string.Empty);
            var tokensList = DataProvider.Sql_Function_At_End();
            const string result = "(0>Sumof(CustBillViewModel.Cycle))";
            var query = StringQueryBuilder.GenerateQuery(tokensList);
            Assert.AreEqual(result, query);
        }

        [Test]
        public void Test_Sql_Function_At_Start_With_Relational()
        {
            StringQueryBuilder = new StringQueryBuilder(string.Empty);
            var tokensList = DataProvider.Sql_Function_At_Start_With_Relational();
            const string result = "(Sumof(CustBillViewModel.Cycle)>0) AND (" +
                                  "CustBillViewModel.CityCategory=\"Tier1\")";
            var query = StringQueryBuilder.GenerateQuery(tokensList);
            Assert.AreEqual(result, query);
        }

        [Test]
        public void Test_Sql_Function_At_End_With_Relational()
        {
            StringQueryBuilder = new StringQueryBuilder(string.Empty);
            var tokensList = DataProvider.Sql_Function_At_End_With_Relational();
            const string result = "(CustBillViewModel.CityCategory=\"Tier1\") AND (Sumof(CustBillViewModel.Cycle)>0)";
            var query = StringQueryBuilder.GenerateQuery(tokensList);
            Assert.AreEqual(result, query);
        }

        [Test]
        public void Test_City_CityCategory_Flag_Product_Tokens()
        {
            StringQueryBuilder = new StringQueryBuilder(string.Empty);
            var tokensList = DataProvider.City_CityCategory_Flag_Product_Tokens();
            var query = StringQueryBuilder.GenerateQuery(tokensList);
            const string finalResult = "(CustBillViewModel.City=\"Pune\") AND (" +
                                       "CustBillViewModel.CityCategory=\"Tier1\") AND (" +
                                       "CustBillViewModel.Flag=\"O\") AND (" +
                                       "CustBillViewModel.Product=\"PL\")";
            Assert.AreEqual(query, finalResult);
        }
    }

    public static class DataProvider
    {
        public static IList<BillTokens> Greater_Than_Tokens()
        {
            // and or : relational & gt, lt : conditional & sum, count , avg : Sql/number
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Table", Value = "CustBillViewModel.Cycle", Priority = 0, DataType = "number"},
                new BillTokens {Type = "Operator", Value = "GreaterThan", Priority = 1, DataType = "conditional"},
                new BillTokens {Type = "Value", Value = "0", Priority = 2, DataType = "number"}
            };
            return query;
        }

        public static IList<BillTokens> Sum_Of_Two_Tokens()
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

        public static IList<BillTokens> Sum_Of_Two_Tokens_Reverse()
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

        public static IList<BillTokens> Sum_and_Greater_Than_Tokens()
        {
            // and or : relational & gt, lt : conditional & sum, count , avg : Sql/number
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Table", Value = "CustBillViewModel.Cycle", Priority = 0, DataType = "number"},
                new BillTokens {Type = "Operator", Value = "Plus", Priority = 1, DataType = "number"},
                new BillTokens {Type = "Value", Value = "2", Priority = 2, DataType = "number"},
                new BillTokens {Type = "Operator", Value = "GreaterThan", Priority = 3, DataType = "conditional"},
                new BillTokens {Type = "Value", Value = "0", Priority = 4, DataType = "number"}
            };
            return query;
        }

        public static IList<BillTokens> City_CityCategory_Flag_Product_Tokens()
        {
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Table",Value = "CustBillViewModel.City",Priority = 0,DataType = "string"},
                new BillTokens {Type = "Operator", Value = "EqualTo", Priority = 1, DataType = "conditional"},
                new BillTokens {Type = "Value", Value = "Pune", Priority = 2, DataType = "string"},

                new BillTokens {Type = "Operator", Value = "AND", Priority = 3, DataType = "relational"},

                new BillTokens {Type = "Table", Value = "CustBillViewModel.CityCategory", Priority = 4, DataType = "string"},
                new BillTokens {Type = "Operator", Value = "EqualTo", Priority = 5, DataType = "conditional"},
                new BillTokens {Type = "Value", Value = "Tier1", Priority = 6, DataType = "string"},

                new BillTokens {Type = "Operator", Value = "AND", Priority = 7, DataType = "relational"},

                new BillTokens {Type = "Table", Value = "CustBillViewModel.Flag", Priority = 8, DataType = "string"},
                new BillTokens {Type = "Operator", Value = "EqualTo", Priority = 9, DataType = "conditional"},
                new BillTokens {Type = "Value", Value = "O", Priority = 10, DataType = "string"},

                 new BillTokens {Type = "Operator", Value = "AND", Priority = 11, DataType = "relational"},

                new BillTokens {Type = "Table", Value = "CustBillViewModel.Product", Priority = 12, DataType = "string"},
                new BillTokens {Type = "Operator", Value = "EqualTo", Priority = 13, DataType = "conditional"},
                new BillTokens {Type = "Value", Value = "PL", Priority = 14, DataType = "string"}
            };
            return query;
        }

        public static IList<BillTokens> Sql_Function_At_Start()
        {
            // and or : relational & gt, lt : conditional & sum, count , avg : Sql/number
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Sql", Value = "Sumof", Priority = 0, DataType = "number"},
                new BillTokens {Type = "Table", Value = "CustBillViewModel.Cycle", Priority = 1, DataType = "number"},
                new BillTokens {Type = "Operator", Value = "GreaterThan", Priority = 2, DataType = "conditional"},
                new BillTokens {Type = "Value", Value = "0", Priority = 3, DataType = "number"}
            };
            return query;
        }

        public static IList<BillTokens> Sql_Function_At_End()
        {
            // and or : relational & gt, lt : conditional & sum, count , avg : Sql/number
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Value", Value = "0", Priority = 0, DataType = "number"},
                new BillTokens {Type = "Operator", Value = "GreaterThan", Priority = 1, DataType = "conditional"},
                new BillTokens {Type = "Sql", Value = "Sumof", Priority = 2, DataType = "number"},
                new BillTokens {Type = "Table", Value = "CustBillViewModel.Cycle", Priority = 3, DataType = "number"},
            };
            return query;
        }

        public static IList<BillTokens> Sql_Function_At_Start_With_Relational()
        {
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Sql", Value = "Sumof", Priority = 0, DataType = "number"},
                new BillTokens {Type = "Table", Value = "CustBillViewModel.Cycle", Priority = 1, DataType = "number"},
                new BillTokens {Type = "Operator", Value = "GreaterThan", Priority = 2, DataType = "conditional"},
                new BillTokens {Type = "Value", Value = "0", Priority = 3, DataType = "number"},

                 new BillTokens {Type = "Operator", Value = "AND", Priority = 4, DataType = "relational"},

                new BillTokens {Type = "Table", Value = "CustBillViewModel.CityCategory", Priority = 5, DataType = "string"},
                new BillTokens {Type = "Operator", Value = "EqualTo", Priority = 6, DataType = "conditional"},
                new BillTokens {Type = "Value", Value = "Tier1", Priority = 7, DataType = "string"},
            };
            return query;
        }

        public static IList<BillTokens> Sql_Function_At_End_With_Relational()
        {
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Table", Value = "CustBillViewModel.CityCategory", Priority = 0, DataType = "string"},
                new BillTokens {Type = "Operator", Value = "EqualTo", Priority = 1, DataType = "conditional"},
                new BillTokens {Type = "Value", Value = "Tier1", Priority = 2, DataType = "string"},

                new BillTokens {Type = "Operator", Value = "AND", Priority = 3, DataType = "relational"},

                new BillTokens {Type = "Sql", Value = "Sumof", Priority = 4, DataType = "number"},
                new BillTokens {Type = "Table", Value = "CustBillViewModel.Cycle", Priority = 5, DataType = "number"},
                new BillTokens {Type = "Operator", Value = "GreaterThan", Priority = 6, DataType = "conditional"},
                new BillTokens {Type = "Value", Value = "0", Priority = 7, DataType = "number"}
            };
            return query;
        }
    }

}
