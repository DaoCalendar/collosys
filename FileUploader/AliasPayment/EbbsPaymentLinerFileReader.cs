#region references

using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.FileUploaderService.FileReader;

#endregion

namespace ColloSys.FileUploaderService.AliasPayment
{
   public class EbbsPaymentLinerFileReader : FileReader<Payment>
    {
       public EbbsPaymentLinerFileReader(FileScheduler fileScheduler)
            : base(fileScheduler, new EbbsPaymentLinerRecordCreator())
        {

        }
    }
}
