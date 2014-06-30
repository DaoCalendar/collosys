using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AngularUI.Shared.apis;
using ColloSys.DataLayer.Allocation;
using ColloSys.QueryBuilder.AllocationBuilder;

namespace AngularUI.Allocation.distributionpercentage
{
    public class DistributionPercentageApiController : BaseApiController<DistributionPercentage>
    {
        private static readonly DistributionPercBuilder DistributionPerc =new DistributionPercBuilder();
       
    }
}