using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColloSys.AllocationService.EmailAllocations;
using NUnit.Framework;

namespace ColloSys.QueryBuilder.Test.Allocation
{
    [TestFixture]
    public class EmailTest
    {
        [Test]
        public void CheckStakeholders()
        {
            var emailMessenger = new AllocationEmailMessanger();
            var data = emailMessenger.GetStakeholderWithManger();
        }
    }
}
