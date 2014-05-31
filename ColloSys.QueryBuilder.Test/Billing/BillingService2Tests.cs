using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BillingService2;
using ColloSys.FileUploadService;
using NUnit.Framework;

namespace ColloSys.QueryBuilder.Test.Billing
{
    [TestFixture]
    public class BillingService2Tests
    {
        [Test]
        public void StartService()
        {
            var billingService = new BillingServices();
            billingService.StartBillingService();
        }
    }
}
