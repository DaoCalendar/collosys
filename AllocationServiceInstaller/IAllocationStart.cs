using System.ServiceModel;

namespace ColloSys.AllocationServiceInstaller
{
    [ServiceContract]
    public interface IAllocationStart
    {
        [OperationContract]
        void StartAllocationProcess();
    }
}
