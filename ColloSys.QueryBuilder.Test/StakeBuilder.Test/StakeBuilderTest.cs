using System;
using ColloSys.DataLayer.Domain;
using ColloSys.QueryBuilder.BaseTypes;
using ColloSys.QueryBuilder.StakeholderBuilder;
using NUnit.Framework;

namespace ColloSys.QueryBuilder.Test.StakeBuilder.Test
{
    public class StakeBuilderTest
    {
        [Test]
        public void GetAllTest()
        {
            var stak = new StakeQueryBuilder();
            var data = stak.GetAll();
            stak.DefaultStakeholders();
            Assert.AreEqual(1,1);
        }
    }
}
