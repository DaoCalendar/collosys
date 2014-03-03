using System.ServiceModel;

namespace ColloSys.BillingServiceInstaller
{
    [ServiceContract]
    public interface IBillingStart
    {
        [OperationContract]
        void StartBillingProcess();
    }
}
