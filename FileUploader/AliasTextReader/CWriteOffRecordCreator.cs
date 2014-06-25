using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColloSys.DataLayer.Domain;
using ColloSys.FileUploaderService.RecordManager;
using ColloSys.Shared.SharedUtils;

namespace ColloSys.FileUploaderService.AliasTextReader
{
   public class CWriteOffRecordCreator:TextRecordCreator<CWriteoff>
    {
        string _firstLine = string.Empty;
        string _secondLine = string.Empty;
        string _thirdLine = string.Empty;

        public override bool CreateRecord(out CWriteoff recordCWriteoff)
        {
           // var currentLine = InpuStreamReader.ReadLine();
            if (!CheckValidRecord(InpuStreamReader))
            {
                recordCWriteoff = null;
                return false;
            }

            try
            {
                var firstLine = _firstLine;
                var secondLine =_secondLine;
                var thirdLine = _thirdLine;

                var writeoff = new CWriteoff
                {
                    AccountNo = Convert.ToUInt64(firstLine.Substring2(1, 16)).ToString(CultureInfo.InvariantCulture)
                };

                // read first line
                if (writeoff.AccountNo.ToString(CultureInfo.InvariantCulture).Length != 16)
                {
                    throw new Exception("Invalid Account Number :" + writeoff.AccountNo);
                }

                writeoff.CustomerName = firstLine.Substring2(18, 15);
                writeoff.Cycle = Convert.ToUInt16(firstLine.Substring2(34, 2));
                writeoff.Location = firstLine.Substring2(40, 2); // U2
                writeoff.Block = firstLine.Substring2(46, 1); // PB
                writeoff.AltBlock = firstLine.Substring2(48, 1); // AB
                writeoff.CreditLimit = (ulong)Convert.ToDecimal(firstLine.Substring2(71, 10));
                writeoff.TotalDue = Convert.ToDecimal(firstLine.Substring2(82, 13));
                writeoff.ActivationDate = Shared.SharedUtils.Utilities.ParseDateTime(firstLine.Substring2(96, 8), "dd/MM/yy");

                writeoff.Bucket1Due = Convert.ToDecimal(firstLine.Substring2(107, 2));
                writeoff.Bucket2Due = Convert.ToDecimal(firstLine.Substring2(111, 2));
                writeoff.Bucket3Due = Convert.ToDecimal(firstLine.Substring2(115, 2));
                writeoff.Bucket4Due = Convert.ToDecimal(firstLine.Substring2(119, 2));
                writeoff.Bucket5Due = Convert.ToDecimal(firstLine.Substring2(123, 2));
                writeoff.Bucket6Due = Convert.ToDecimal(firstLine.Substring2(127, 2));
                writeoff.Bucket7Due = Convert.ToDecimal(firstLine.Substring2(131, 2));


                // read second line
                writeoff.LastPayDate = Shared.SharedUtils.Utilities.ParseNullableDateTime(secondLine.Substring2(96, 8), "dd/MM/yy");

                // read third line
                writeoff.ExpirtyDate = Shared.SharedUtils.Utilities.ParseDateTime(thirdLine.Substring2(96, 8), "MM/yy");

                recordCWriteoff = writeoff;
                return true;
            }
            catch (Exception)
            {
                recordCWriteoff = null;
                Counter.IncrementIgnoreRecord();
                return false;
            }
        }

        private bool CheckValidRecord(StreamReader currentLine)
        {
            _firstLine = currentLine.ReadLine();
            if ((_firstLine == null) || (string.IsNullOrWhiteSpace(_firstLine)))
            {
                Counter.IncrementIgnoreRecord();
                return false;
            }

            try
            {
                var scbIndex = _firstLine.IndexOf("STANDARD CHARTERED BANK INDIA   CURRENCY",
                                                         StringComparison.InvariantCulture);
                if (scbIndex > 0)
                {
                    //var filedatestring = firstLine.Substring2((uint)(103 + scbIndex), 8);
                    //Reader.UploadedFile.FileDate = Utilities.ParseDateTime(filedatestring, "dd/MM/yy");
                    Counter.IncrementIgnoreRecord();
                    return false;
                }
                Convert.ToUInt64(_firstLine.Substring2(1, 16));
                // ReSharper restore ReturnValueOfPureMethodIsNotUsed
                Shared.SharedUtils.Utilities.ParseDateTime(_firstLine.Substring2(96, 8), "dd/MM/yy");


                // get second line
                Counter.IncrementLineNo();
                _secondLine = currentLine.ReadLine();
                Shared.SharedUtils.Utilities.ParseNullableDateTime(_secondLine.Substring2(96, 8), "dd/MM/yy");

                // get third line
                Counter.IncrementLineNo();
                _thirdLine = currentLine.ReadLine();
                Shared.SharedUtils.Utilities.ParseDateTime(_thirdLine.Substring2(96, 8), "MM/yy");

                #region cmt

                //if (currentLine.Contains("PAN:"))
                //{
                //    _firstLine = currentLine;
                //    Counter.IncrementIgnoreRecord();
                //    return false;
                //}

                //if (currentLine.Contains("MERCHANT:"))
                //{
                //    _secondLine = currentLine;
                //    Counter.IncrementIgnoreRecord();
                //    return false;
                //}

                //if (currentLine.Contains("MCC:"))
                //{
                //    var mcc = Convert.ToInt32(currentLine.Substring2(99, 5).Trim());

                //    if (mcc == 6012 || mcc == 6051)
                //    {
                //        //throw new Exception("");
                //        return true;
                //        //return new TextFileRow<string[]>
                //        //{
                //        //    LineNo = LineNo,
                //        //    RowValue = new[] { firstLine, secondLine }
                //        //};
                //    }
                //    Counter.IncrementIgnoreRecord();
                //}

                #endregion

                return true;        
            }
            catch (Exception)
            {
                Counter.IncrementIgnoreRecord();
                return false;
            }
        }
        public override CWriteoff GetRecordForUpdate()
        {
            throw new NotImplementedException();
        }
    }
}
