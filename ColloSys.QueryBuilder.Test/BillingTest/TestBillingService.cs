using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColloSys.BillingService;
using NUnit.Framework;

namespace ColloSys.QueryBuilder.Test.BillingTest
{
    public class TestBillingService
    {
        [Test]
        public void StartBillingService()
        {
            var billService = new BillingServices();
            billService.StartBillingService();
        }
    }
}
