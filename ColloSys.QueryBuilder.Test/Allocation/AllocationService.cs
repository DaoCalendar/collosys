using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColloSys.AllocationService;
using ColloSys.AllocationService.DBLayer;
using ColloSys.AllocationService.PincodeEntry;
using ColloSys.DataLayer.Enumerations;
using NUnit.Framework;

namespace ColloSys.QueryBuilder.Test.Allocation
{
    [TestFixture]
    public class AllocationService
    {
        [Test]
        public void CheckPolicies()
        {
            var product = ScbEnums.Products.SMC;
            var category = ScbEnums.Category.Liner;
           var policies= DbLayer.GetAllocationPolicy(product, category);

            Assert.AreEqual(policies.AllocRelations.Count,3);
        }

        [Test]
        public void MovePincodesToCustomerInfo()
        {
            LinerWriteoffPincodes.Init();
        }

        [Test]
        public void AllocationProcessForPerticularProduct()
        {
            ColloSys.AllocationService.AllocationLayer.Allocation.StartAllocationProcessV2(ScbEnums.Products.SMC, ScbEnums.Category.Liner);
        }

        [Test]
        public void StartAllocationProcess()
        {
            StartAllocation.Start();
        }
    }
}
