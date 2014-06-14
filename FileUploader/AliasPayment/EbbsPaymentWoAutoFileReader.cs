#region references

using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.FileUploaderService.FileReader;

#endregion

namespace ColloSys.FileUploaderService.AliasPayment
{
    public class EbbsPaymentWoAutoFileReader : FileReader<Payment>
    {
        public EbbsPaymentWoAutoFileReader(FileScheduler fileScheduler)
            : base(fileScheduler, new EbbsPaymentWoAutoRecordCreator())
        {

        }
    }
}
