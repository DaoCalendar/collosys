#region references

using System;
using System.Globalization;
using System.IO;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.FileUploaderService.RecordManager;
using ColloSys.Shared.SharedUtils;

#endregion


namespace ColloSys.FileUploaderService.AliasTextReader
{
    public class LitUitRecordCreator : TextRecordCreator<Payment>
    {
       // private readonly StreamReader _inpuStreamReader;


        public LitUitRecordCreator(FileScheduler fileScheduler)
        {
           // var info = new FileInfo(fileScheduler.FileDirectory + @"\" + fileScheduler.FileName);
           // _inpuStreamReader = new StreamReader(info.FullName);
        }



        public override bool CreateRecord(out Payment payment)
        {
            var currentLine = InpuStreamReader.ReadLine();
            if (!CheckValidRecord(currentLine))
            {
                payment = null;
                return false;
            }

            try
            {
                payment = new Payment { TransCode = int.Parse(currentLine.Substring2(41, 2)) };

                if ((payment.TransCode < 19) || (payment.TransCode > 30))
                {
                    Counter.IncrementIgnoreRecord();
                    return false;
                }

                payment.AccountNo = ulong.Parse(currentLine.Substring2(21, 16)).ToString(CultureInfo.InvariantCulture);
                if (payment.AccountNo.ToString(CultureInfo.InvariantCulture).Length != 16)
                {
                    Counter.IncrementIgnoreRecord();
                    return false;
                }

                payment.TransAmount = decimal.Parse(currentLine.Substring2(45, 10).Replace(",", ""));
                payment.TransDate = Shared.SharedUtils.Utilities.ParseDateTime(currentLine.Substring2(57, 6), "ddMMyy");
                payment.TransDesc = currentLine.Substring2(63);

                if (payment.TransCode == 16 || payment.TransCode == 24 || payment.TransCode == 26 || payment.TransCode == 27 ||
                    payment.TransCode == 28)
                {
                    payment.TransAmount = payment.TransAmount * -1;
                }
                Counter.IncrementValidRecords();
            }
            catch (Exception)
            {
                payment = null;
                Counter.IncrementIgnoreRecord();
                return false;
            }

            return true;

        }

        private bool CheckValidRecord(string currentLine)
        {
            if ((currentLine == null) || (string.IsNullOrWhiteSpace(currentLine)))
            {
                Counter.IncrementIgnoreRecord();
                return false;
            }

            try
            {
                var scbIndex = currentLine.IndexOf("SG CARDLINK",
                                                   StringComparison.InvariantCulture);
                if (scbIndex > 0)
                {
                    Counter.IncrementIgnoreRecord();
                    return false;
                }

                int transCode;
                if (!int.TryParse(currentLine.Substring2(41, 2), out transCode))
                {
                    Counter.IncrementIgnoreRecord();
                    return false;
                }
                if ((transCode < 19) || (transCode > 30))
                {
                    Counter.IncrementIgnoreRecord();
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                Counter.IncrementIgnoreRecord();
                return false;
            }
        }

        public override Payment GetRecordForUpdate()
        {
            throw new NotImplementedException();
        }
    }
}
