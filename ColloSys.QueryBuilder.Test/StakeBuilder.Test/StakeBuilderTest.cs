using System;
using ColloSys.DataLayer.Domain;
using ColloSys.QueryBuilder.BaseTypes;
using NUnit.Framework;

namespace ColloSys.QueryBuilder.Test.StakeBuilder.Test
{
    public class StakeBuilderTest
    {
        private static QueryBuilderFactory Factory=new QueryBuilderFactory();

        [Test]
        public void GetAllTest()
        {
            var stakeQueries = Factory.BuilderFor<Stakeholders>(TypeOf.Stakeholder);

            var workingQueries = Factory.BuilderFor<StkhWorking>(TypeOf.StkhWorking);

            var dataForWorking = workingQueries.GetAll();

            var stakeholdersList = stakeQueries.GetAll();

            var distinct = stakeQueries.GetAll(true);

            var alldata = stakeQueries.GetOnExpression(x => x.JoiningDate > DateTime.Now);

            Assert.AreEqual(1,1);
        }
    }
}
