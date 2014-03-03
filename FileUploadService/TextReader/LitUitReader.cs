#region references

using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using System;
using System.Globalization;
using ColloSys.DataLayer.SharedDomain;
using ColloSys.FileUploadService.Interfaces;
using ColloSys.Shared.SharedUtils;
using NLog;

#endregion


namespace ColloSys.FileUploadService.TextReader
{
    internal class LitUitReader : SingleLineTextReader<Payment>
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        #region constructor

        private static FileReaderProperties GetFileReaderProperties()
        {
            return new FileReaderProperties
            {
                //HasFileDateInsideFile = true
            };
        }

        public LitUitReader(FileScheduler file)
            : base(file, GetFileReaderProperties())
        {

        }

        #endregion

        #region Get Next Row

        protected override TextFileRow<string> GetNextRow()
        {
            while (!HasEofReached())
            {
                string currentLine;

                lock (InputFileStream)
                {
                    RowNo++;
                    currentLine = InputFileStream.ReadLine();
                }

                if ((currentLine == null) || (string.IsNullOrWhiteSpace(currentLine)))
                {
                    Reader.Counter.AddIgnoredRecord(RowNo);
                    continue;
                }

                try
                {
                    var scbIndex = currentLine.IndexOf("SG CARDLINK",
                                                       StringComparison.InvariantCulture);
                    if (scbIndex > 0)
                    {
                        //var filedatestring = currentLine.Substring2((uint)(46 + scbIndex), 8);
                        //Reader.UploadedFile.FileDate = Utilities.ParseDateTime(filedatestring, "dd/MM/yy");
                        Reader.Counter.AddIgnoredRecord(RowNo);
                        continue;
                    }

                    var transCode = int.Parse(currentLine.Substring2(41, 2));
                    if ((transCode < 19) || (transCode > 30))
                    {
                        Reader.Counter.AddIgnoredRecord(RowNo);
                        continue;
                    }

                    return new TextFileRow<string> { LineNo = RowNo, RowValue = currentLine };
                }
                // ReSharper disable EmptyGeneralCatchClause
                catch
                // ReSharper restore EmptyGeneralCatchClause
                {
                    Reader.Counter.AddIgnoredRecord(LineNo);
                }
            }

            return null;
        }

        #endregion

        #region Get Record
        protected override Payment GetRecord(string row)
        {
            var currentLine = row;
            var payment = new Payment { TransCode = int.Parse(currentLine.Substring2(41, 2)) };

            if ((payment.TransCode < 19) || (payment.TransCode > 30))
                return null;

            payment.AccountNo = ulong.Parse(currentLine.Substring2(21, 16)).ToString(CultureInfo.InvariantCulture);
            if (payment.AccountNo.ToString(CultureInfo.InvariantCulture).Length != 16)
            {
                throw new Exception("Invalid Account Number :" + payment.AccountNo);
            }

            payment.TransAmount = decimal.Parse(currentLine.Substring2(45, 10).Replace(",", ""));
            payment.TransDate = Utilities.ParseDateTime(currentLine.Substring2(57, 6), "ddMMyy");
            payment.TransDesc = currentLine.Substring2(63);

            if (payment.TransCode == 16 || payment.TransCode == 24 || payment.TransCode == 26 || payment.TransCode == 27 ||
                payment.TransCode == 28)
            {
                payment.TransAmount = payment.TransAmount * -1;
            }


            return payment;
        }

        protected override void PopulateDefault(Payment record)
        {
            record.Status = ColloSysEnums.ApproveStatus.NotApplicable;
            record.BillDate = null;
            record.BillStatus = ColloSysEnums.BillStatus.Unbilled;
            record.IsExcluded = false;
            record.ExcludeReason = string.Empty;
            record.Products = ScbEnums.Products.CC;
            record.IsDebit = true;
        }

        #endregion

        #region Handle Unique Record

        protected override bool PerformUpdates(Payment record)
        {
            return false;
        }

        #endregion

        #region Record Valid
        protected override bool IsRecordValid(Payment record)
        {
            if (record.AccountNo.ToString(CultureInfo.InvariantCulture).Length != 16)
            {
                _log.Info(string.Format("Card No : {0} has length {1} is not valid", record.AccountNo, record.AccountNo.ToString(CultureInfo.InvariantCulture).Length));
                return false;
            }

            //if (record.TransAmount <= 0)
            //{
            //    _log.Info(string.Format("Card No : {0} has TransAmount {1} is not valid", record.AccountNo, record.TransAmount));
            //    return false;
            //}

            if (record.TransDate <= DateTime.Now.AddYears(-1) || (record.TransDate >= DateTime.Now.AddYears(1)))
            {
                _log.Info(string.Format("Card No : {0} has TransDate {1} is not valid", record.AccountNo, record.TransDate));
                return false;
            }

            var filter = IsNotFiletered(record);

            if (!filter)
            {
                _log.Info(string.Format("Card No : {0} has TransDesc {1} is not valid due to filter", record.AccountNo, record.TransDesc));
            }

            return filter;
        }

        //private bool IsNotFiletered2(CPayment record)
        //{
        //    if (record.TransDesc.StartsWith("OT"))
        //    {

        //    }

        //    if (record.TransDesc.IndexOf("OT", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("ZERO", StringComparison.InvariantCulture) > 0 && record.TransCode == 22)
        //        return false;
        //    if (record.TransDesc.IndexOf("REVS", StringComparison.InvariantCulture) > 0 && record.TransCode == 22)
        //        return false;
        //    if (record.TransDesc.IndexOf("rev", StringComparison.InvariantCulture) > 0 && record.TransCode == 22)
        //        return false;
        //    if (record.TransDesc.IndexOf("INT.REVS", StringComparison.Ordinal) > 0)
        //        return false;
        //    //if (  transDesc.IndexOf("TRFD")>0 && FLAG="TOB";
        //    if (record.TransDesc.StartsWith("TRFD"))
        //    {

        //    }

        //    if (record.TransDesc.IndexOf("TRFD", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("ZEROISATION", StringComparison.InvariantCulture) > 0 && record.TransCode == 22)
        //        return false;
        //    if (record.TransDesc.IndexOf("SETTLEMENT", StringComparison.InvariantCulture) > 0 && record.TransCode == 22)
        //        return false;
        //    if (record.TransDesc.IndexOf("TRANSFER REVERSAL", StringComparison.InvariantCulture) > 0 && record.TransCode == 22)
        //        return false;
        //    if (record.TransDesc.IndexOf("NUNP", StringComparison.InvariantCulture) > 0 && record.TransCode == 22)
        //        return false;
        //    if (record.TransDesc.IndexOf("SERV", StringComparison.InvariantCulture) > 0 && record.TransCode == 22)
        //        return false;
        //    if (record.TransDesc.IndexOf("FEE", StringComparison.InvariantCulture) > 0 && record.TransCode == 22)
        //        return false;
        //    if (record.TransDesc.IndexOf("@", StringComparison.InvariantCulture) > 0 && record.TransCode == 22)
        //        return false;
        //    if (record.TransDesc.IndexOf("BILL", StringComparison.InvariantCulture) > 0 && record.TransCode == 22)
        //        return false;
        //    if (record.TransDesc.IndexOf("MISC CREDIT", StringComparison.InvariantCulture) > 0 && record.TransCode == 22)
        //        return false;
        //    if (record.TransDesc.IndexOf("DISCOUNT", StringComparison.InvariantCulture) > 0 && record.TransCode == 22)
        //        return false;
        //    if (record.TransDesc.IndexOf("TRANSFER", StringComparison.InvariantCulture) > 0 && record.TransCode == 27)
        //        return false;
        //    if (record.TransDesc.IndexOf("RELEASE", StringComparison.InvariantCulture) > 0 && record.TransCode == 16)
        //        return false;
        //    if (record.TransDesc.IndexOf("RELEASE", StringComparison.InvariantCulture) > 0 && record.TransCode == 22)
        //        return false;
        //    if (record.TransDesc.IndexOf("REXT", StringComparison.InvariantCulture) > 0 && record.TransCode == 20) // Removing TC 20 rejetced paymemt 28-Aug-12
        //        return false;
        //    if (record.TransDesc.IndexOf("provision", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("EMI", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("PROV", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("80PROV", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("14CREDIT CARD REPAYMENT", StringComparison.InvariantCulture) > 0 && record.TransCode != 22)//and TC not in ('22');
        //        return false;
        //    if (record.TransDesc.IndexOf("NEFT CREDIT", StringComparison.InvariantCulture) > 0 && record.TransCode != 21)//and TC not in ('21');
        //        return false;
        //    if (record.TransDesc.IndexOf("CREDIT", StringComparison.InvariantCulture) > 0 && (record.TransCode != 21 && record.TransCode != 22))//and TC not in ('21','22')THEN DELETE;
        //        return false;
        //    if (record.TransDesc.IndexOf("36INTEREST CHARGES REVERSAL", StringComparison.InvariantCulture) > 0 && record.TransCode == 22)
        //        return false;
        //    if (record.TransDesc.IndexOf("33LATE CHARGES REVERSAL", StringComparison.InvariantCulture) > 0 && record.TransCode == 22)
        //        return false;
        //    if (record.TransDesc.IndexOf("A3INST CHARGE REV", StringComparison.InvariantCulture) > 0 && record.TransCode == 22)
        //        return false;
        //    if (record.TransDesc.IndexOf("BlCard Replacement Fee Rev", StringComparison.InvariantCulture) > 0 && record.TransCode == 22)
        //        return false;
        //    if (record.TransDesc.IndexOf("BlCadr Replacement Fee Rev", StringComparison.InvariantCulture) > 0 && record.TransCode == 22)
        //        return false;
        //    if (record.TransDesc.IndexOf("31LATE CHARGES REVERSAL", StringComparison.InvariantCulture) > 0 && record.TransCode == 22)
        //        return false;
        //    if (record.TransDesc.IndexOf("Interest Holiday Balance Transfer", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("KUCH BHI", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("ADJUSTMENT", StringComparison.InvariantCulture) > 0 && record.TransCode == 1722)
        //        return false;
        //    if (record.TransDesc.IndexOf("EASYPAY", StringComparison.InvariantCulture) > 0 && record.TransCode == 22)
        //        return false;
        //    if (record.TransDesc.IndexOf("EASY PAY REVERSAL", StringComparison.InvariantCulture) > 0 && record.TransCode == 22)
        //        return false;
        //    if (record.TransDesc.IndexOf("GIFT", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("SURCHARGE", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("INTEREST HOLIDAY BALANCE TRANSF", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("PRO-AH", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("PRO-BH", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("PRO-BL", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("PRO-BY", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("PRO-CA", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("PRO-CB", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("PRO-CH", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("PRIVILEGE CUSTOMER", StringComparison.InvariantCulture) > 0 && record.TransCode == 22)
        //        return false;
        //    if (record.TransDesc.IndexOf("PRIVELEGE CUSTOMER", StringComparison.InvariantCulture) > 0 && record.TransCode == 22)
        //        return false;
        //    if (record.TransDesc.IndexOf("SCB DIAL A LOAN", StringComparison.InvariantCulture) > 0 && record.TransCode == 22)
        //        return false;
        //    if (record.TransDesc.IndexOf("CB# ", StringComparison.InvariantCulture) > 0 && record.TransCode == 22)
        //        return false;
        //    if (record.TransDesc.IndexOf("PRO-CO", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("PRO-HY", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("PRO-IE", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("PRO-JA", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("PRO-KA", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("PRO-LK", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("PRO-LU", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("PRO-MA", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("PRO-MY", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("PRO-ND", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("PRO-PU", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("PRO-VA", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("Prov-AH", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("Prov-BH", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("Prov-BL", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("Prov-BY", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("Prov-CA", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("Prov-CB", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("Prov-CH", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("EASY PAY OFFER", StringComparison.InvariantCulture) > 0 && record.TransCode == 22)
        //        return false;
        //    if (record.TransDesc.IndexOf("PRIVILEGE CUSTOMER", StringComparison.InvariantCulture) > 0 && record.TransCode == 22)
        //        return false;
        //    if (record.TransDesc.IndexOf("PRIVELEGE CUSTOMER", StringComparison.InvariantCulture) > 0 && record.TransCode == 22)
        //        return false;
        //    if (record.TransDesc.IndexOf("SCB DIAL A LOAN", StringComparison.InvariantCulture) > 0 && record.TransCode == 22)
        //        return false;
        //    if (record.TransDesc.IndexOf("CB# ", StringComparison.InvariantCulture) > 0 && record.TransCode == 22)
        //        return false;
        //    if (record.TransDesc.IndexOf("MC# ", StringComparison.InvariantCulture) > 0 && record.TransCode == 22)
        //        return false;
        //    if (record.TransDesc.IndexOf("Prov-CO", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("Prov-HY", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("Prov-IE", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("Prov-JA", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("Prov-KA", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("Prov-LK", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("Prov-LU", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("Prov-MA", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("Prov-MY", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("Prov-ND", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("Prov-PU", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("Prov-VA", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("PROV-AH", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("PROV-BY", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("PROV-BL", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("PROV-CA", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("PROV-CO", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("PROV-CB", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("PROV-ND", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("PROV-HY", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("PROV-MA", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("PROV-PU", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("PROV-IE", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("PROV-VZ", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("PROV-BU", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("PROV-TR", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("PROV-VA", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("PROV-MY", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("PROV-BH", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("PROV-CH", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("PROV-LU", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("PROV-JA", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("PROV-LK", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("PROV-KA", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("PROV REL", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("-Prov Rele-", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("APPORTIONED CREDIT", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("APPORTIONED DEBIT", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("APPORTIONMENT CREDIT", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("APPORTIONMENT DEBIT", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("20APPORTIONMENT CREDIT", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("20APPORTIONMENT DEBIT", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("APPORTIONMENT CREDIT SETTL", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("APPORTIONMENT DEBIT SETTL", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("DEBIT ADJUSTMENT", StringComparison.InvariantCulture) > 0)
        //        return false;
        //    if (record.TransDesc.IndexOf("CREDIT ADJUSTMENT", StringComparison.InvariantCulture) > 0)
        //        return false;

        //    return true;
        //}

        private bool IsNotFiletered(Payment record)
        {
            if (record.TransDesc.IndexOf("OT", StringComparison.InvariantCulture) > -1 && (record.TransCode != 19 && record.TransCode != 20 && record.TransCode != 21 && record.TransCode != 27 && record.TransCode != 28))
                return false;
            if (record.TransDesc.IndexOf("ZERO", StringComparison.InvariantCulture) > -1 && record.TransCode == 22)
                return false;
            if (record.TransDesc.IndexOf("REVS", StringComparison.InvariantCulture) > -1 && record.TransCode == 22)
                return false;
            if (record.TransDesc.IndexOf("rev", StringComparison.InvariantCulture) > -1 && record.TransCode == 22)
                return false;
            if (record.TransDesc.IndexOf("INT.REVS", StringComparison.Ordinal) > -1)
                return false;
            //if (  transDesc.IndexOf("TRFD")>0 && FLAG="TOB";
            if (record.TransDesc.IndexOf("TRFD", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("ZEROISATION", StringComparison.InvariantCulture) > -1 && record.TransCode == 22)
                return false;
            if (record.TransDesc.IndexOf("SETTLEMENT", StringComparison.InvariantCulture) > -1 && record.TransCode == 22)
                return false;
            if (record.TransDesc.IndexOf("TRANSFER REVERSAL", StringComparison.InvariantCulture) > -1 && record.TransCode == 22)
                return false;
            if (record.TransDesc.IndexOf("NUNP", StringComparison.InvariantCulture) > -1 && record.TransCode == 22)
                return false;
            if (record.TransDesc.IndexOf("SERV", StringComparison.InvariantCulture) > -1 && record.TransCode == 22)
                return false;
            if (record.TransDesc.IndexOf("FEE", StringComparison.InvariantCulture) > -1 && record.TransCode == 22)
                return false;
            if (record.TransDesc.IndexOf("@", StringComparison.InvariantCulture) > -1 && record.TransCode == 22)
                return false;
            if (record.TransDesc.IndexOf("BILL", StringComparison.InvariantCulture) > -1 && record.TransCode == 22)
                return false;
            if (record.TransDesc.IndexOf("MISC CREDIT", StringComparison.InvariantCulture) > -1 && record.TransCode == 22)
                return false;
            if (record.TransDesc.IndexOf("DISCOUNT", StringComparison.InvariantCulture) > -1 && record.TransCode == 22)
                return false;
            if (record.TransDesc.IndexOf("TRANSFER", StringComparison.InvariantCulture) > -1 && record.TransCode == 27)
                return false;
            if (record.TransDesc.IndexOf("RELEASE", StringComparison.InvariantCulture) > -1 && record.TransCode == 16)
                return false;
            if (record.TransDesc.IndexOf("RELEASE", StringComparison.InvariantCulture) > -1 && record.TransCode == 22)
                return false;
            if (record.TransDesc.IndexOf("REXT", StringComparison.InvariantCulture) > -1 && record.TransCode == 20) // Removing TC 20 rejetced paymemt 28-Aug-12
                return false;
            if (record.TransDesc.IndexOf("provision", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("EMI", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("PROV", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("80PROV", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("14CREDIT CARD REPAYMENT", StringComparison.InvariantCulture) > -1 && record.TransCode != 22)//and TC not in ('22');
                return false;
            if (record.TransDesc.IndexOf("NEFT CREDIT", StringComparison.InvariantCulture) > -1 && record.TransCode != 21)//and TC not in ('21');
                return false;
            if (record.TransDesc.IndexOf("CREDIT", StringComparison.InvariantCulture) > -1 && (record.TransCode != 21 && record.TransCode != 22))//and TC not in ('21','22')THEN DELETE;
                return false;
            if (record.TransDesc.IndexOf("36INTEREST CHARGES REVERSAL", StringComparison.InvariantCulture) > -1 && record.TransCode == 22)
                return false;
            if (record.TransDesc.IndexOf("33LATE CHARGES REVERSAL", StringComparison.InvariantCulture) > -1 && record.TransCode == 22)
                return false;
            if (record.TransDesc.IndexOf("A3INST CHARGE REV", StringComparison.InvariantCulture) > -1 && record.TransCode == 22)
                return false;
            if (record.TransDesc.IndexOf("BlCard Replacement Fee Rev", StringComparison.InvariantCulture) > -1 && record.TransCode == 22)
                return false;
            if (record.TransDesc.IndexOf("BlCadr Replacement Fee Rev", StringComparison.InvariantCulture) > -1 && record.TransCode == 22)
                return false;
            if (record.TransDesc.IndexOf("31LATE CHARGES REVERSAL", StringComparison.InvariantCulture) > -1 && record.TransCode == 22)
                return false;
            if (record.TransDesc.IndexOf("Interest Holiday Balance Transfer", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("KUCH BHI", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("ADJUSTMENT", StringComparison.InvariantCulture) > -1 && record.TransCode == 22)
                return false;
            if (record.TransDesc.IndexOf("EASYPAY", StringComparison.InvariantCulture) > -1 && record.TransCode == 22)
                return false;
            if (record.TransDesc.IndexOf("EASY PAY REVERSAL", StringComparison.InvariantCulture) > -1 && record.TransCode == 22)
                return false;
            if (record.TransDesc.IndexOf("GIFT", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("SURCHARGE", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("INTEREST HOLIDAY BALANCE TRANSF", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("PRO-AH", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("PRO-BH", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("PRO-BL", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("PRO-BY", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("PRO-CA", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("PRO-CB", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("PRO-CH", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("PRIVILEGE CUSTOMER", StringComparison.InvariantCulture) > -1 && record.TransCode == 22)
                return false;
            if (record.TransDesc.IndexOf("PRIVELEGE CUSTOMER", StringComparison.InvariantCulture) > -1 && record.TransCode == 22)
                return false;
            if (record.TransDesc.IndexOf("SCB DIAL A LOAN", StringComparison.InvariantCulture) > -1 && record.TransCode == 22)
                return false;
            if (record.TransDesc.IndexOf("CB# ", StringComparison.InvariantCulture) > -1 && record.TransCode == 22)
                return false;
            if (record.TransDesc.IndexOf("PRO-CO", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("PRO-HY", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("PRO-IE", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("PRO-JA", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("PRO-KA", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("PRO-LK", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("PRO-LU", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("PRO-MA", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("PRO-MY", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("PRO-ND", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("PRO-PU", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("PRO-VA", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("Prov-AH", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("Prov-BH", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("Prov-BL", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("Prov-BY", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("Prov-CA", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("Prov-CB", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("Prov-CH", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("EASY PAY OFFER", StringComparison.InvariantCulture) > -1 && record.TransCode == 22)
                return false;
            if (record.TransDesc.IndexOf("PRIVILEGE CUSTOMER", StringComparison.InvariantCulture) > -1 && record.TransCode == 22)
                return false;
            if (record.TransDesc.IndexOf("PRIVELEGE CUSTOMER", StringComparison.InvariantCulture) > -1 && record.TransCode == 22)
                return false;
            if (record.TransDesc.IndexOf("SCB DIAL A LOAN", StringComparison.InvariantCulture) > -1 && record.TransCode == 22)
                return false;
            if (record.TransDesc.IndexOf("CB# ", StringComparison.InvariantCulture) > -1 && record.TransCode == 22)
                return false;
            if (record.TransDesc.IndexOf("MC# ", StringComparison.InvariantCulture) > -1 && record.TransCode == 22)
                return false;
            if (record.TransDesc.IndexOf("Prov-CO", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("Prov-HY", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("Prov-IE", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("Prov-JA", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("Prov-KA", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("Prov-LK", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("Prov-LU", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("Prov-MA", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("Prov-MY", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("Prov-ND", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("Prov-PU", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("Prov-VA", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("PROV-AH", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("PROV-BY", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("PROV-BL", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("PROV-CA", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("PROV-CO", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("PROV-CB", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("PROV-ND", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("PROV-HY", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("PROV-MA", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("PROV-PU", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("PROV-IE", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("PROV-VZ", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("PROV-BU", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("PROV-TR", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("PROV-VA", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("PROV-MY", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("PROV-BH", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("PROV-CH", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("PROV-LU", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("PROV-JA", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("PROV-LK", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("PROV-KA", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("PROV REL", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("-Prov Rele-", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("APPORTIONED CREDIT", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("APPORTIONED DEBIT", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("APPORTIONMENT CREDIT", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("APPORTIONMENT DEBIT", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("20APPORTIONMENT CREDIT", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("20APPORTIONMENT DEBIT", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("APPORTIONMENT CREDIT SETTL", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("APPORTIONMENT DEBIT SETTL", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("DEBIT ADJUSTMENT", StringComparison.InvariantCulture) > -1)
                return false;
            if (record.TransDesc.IndexOf("CREDIT ADJUSTMENT", StringComparison.InvariantCulture) > -1)
                return false;

            return true;
        }


        #endregion
    }
}
