using ColloSys.DataLayer.Components;

namespace ColloSys.DataLayer.Domain
{
    public interface ISoftDelq : IDelinquentCustomer
    {
        uint Cycle { get; set; }
        uint Bucket { get; set; }
    }
}