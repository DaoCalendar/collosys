using System;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.QueryBuilder.BaseTypes;
using ColloSys.QueryBuilder.StakeholderBuilder;
using ColloSys.QueryBuilder.BillingBuilder;
using NUnit.Framework;
using System.Linq;

namespace ColloSys.QueryBuilder.Test.StakeBuilder.Test
{
    public class StakeBuilderTest
    {
        [Test]
        public void GetAllTest()
        {
            var stak = new StakeQueryBuilder();
            var data = stak.GetAll();
            Assert.AreEqual(1,1);
        }

        [Test]
        public void Check_DefaultQuery_And_ExecuteQuery()
        {
            var stakeQuery = new StakeQueryBuilder();
            var query = stakeQuery.ApplyRelations();
            var data = stakeQuery.Execute(query);

            var totalData = stakeQuery.GetAll();
            Assert.AreEqual(data.Count(),totalData.Count());
        }

        [Test]
        public void Check_asdk()
        {
            
        }

        [Test]
        public void Check_Policies()
        {
            var policyBuilder = new BillingPolicyBuilder();
            var data = policyBuilder.OnProductCategoryWIthTokens(ScbEnums.Products.HL,  ScbEnums.Category.Liner);
        }
    }
}
