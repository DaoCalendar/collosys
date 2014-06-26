using System;
using System.Globalization;
using System.IO;
using System.Linq;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.FileUploaderService.RecordManager;
using ColloSys.Shared.SharedUtils;

namespace ColloSys.FileUploaderService.AliasTextReader
{
    public class CacsActivityRecordCreator : TextRecordCreator<CacsActivity>
    {
        string _location = string.Empty;
        string _currentLine = string.Empty;
        string _firstLine = string.Empty;
        string _secondLine = string.Empty;

        public override bool CreateRecord(out CacsActivity obj)
        {
            var firstLine = _firstLine;
            var secondLine = _secondLine;
            //var thirdLine = row[2];

            if (!CheckValidRecord(InpuStreamReader))
            {
                obj = null;
                return false;
            }

            var record = new CacsActivity();


            // read location from first line
            var location = Convert.ToUInt64(firstLine.Substring2(11, 6).ToString(CultureInfo.InvariantCulture));
            record.Products = DecodeProduct(location);
            record.Region = DecodeRegion(location);

            // read telecaller id from second line
            record.TelecallerId = secondLine.Substring2(14, 8);

            // read file date from third line
            record.FileDate = FileScheduler.FileDate.Date;

            // get other properties from forth line
            var forthLine = _currentLine;

            if (!string.IsNullOrWhiteSpace(forthLine))
            {


                record.AccountNo = ulong.Parse(forthLine.Substring2(1, 16)).ToString(CultureInfo.InvariantCulture);
                record.CallDirection = forthLine.Substring2(51, 2); //AC
                record.CallLocation = forthLine.Substring2(54, 1); //P
                record.CallResponce = forthLine.Substring2(56, 2); //C
                record.ActivityCode = forthLine.Substring2(58, 3); //RTE

                if (!string.IsNullOrWhiteSpace(forthLine.Substring2(70, 11)))
                    record.Ptp1Amt = Convert.ToDecimal(forthLine.Substring2(70, 11));

                if (!string.IsNullOrWhiteSpace(forthLine.Substring2(83, 5)))
                    record.Ptp1Date = Shared.SharedUtils.Utilities.ParseNullableDateTime(forthLine.Substring2(83, 5),
                        "dd/MM");

                if (!string.IsNullOrWhiteSpace(forthLine.Substring2(92, 14)))
                    record.Ptp2Amt = Convert.ToDecimal(forthLine.Substring2(92, 14));

                if (!string.IsNullOrWhiteSpace(forthLine.Substring2(107, 5)))
                    record.Ptp2Date = Shared.SharedUtils.Utilities.ParseNullableDateTime(forthLine.Substring2(107, 5),
                        "dd/MM");

                record.ExcuseCode = forthLine.Substring2(118, 1);

                // convert time from singapore to india
                var datetime = Shared.SharedUtils.Utilities.ParseDateTime(forthLine.Substring2(35, 14), "dd/MM/yy HH:mm");
                var singapore = TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time");
                record.CallDateTime = TimeZoneInfo.ConvertTime(datetime, singapore, TimeZoneInfo.Local);
                // store the duration in seconds
                var duraion = Shared.SharedUtils.Utilities.ParseDateTime(forthLine.Substring2(124, 5), "m:ss");
                record.CallDuration = (uint) (duraion.Minute*60 + duraion.Second);

                //records to be allocated
                var consideredActivityCodes = new[] {"H70", "M70", "F70"};
                record.ConsiderInAllocation = consideredActivityCodes.Contains(record.ActivityCode);

                PopulateDefault(record);

                if (!IsRecordValid(record))
                {
                    obj = null;
                    Counter.IncrementIgnoreRecord();
                    return false;
                }
                Counter.IncrementValidRecords();
                obj = record;
                return true;
            }
            Counter.IncrementIgnoreRecord();
            obj = null;
            return false;
        }

        private bool CheckValidRecord(StreamReader reader)
        {
            _currentLine = string.Empty;
          var  currentLine = reader.ReadLine();
            if ((currentLine == null) || (string.IsNullOrWhiteSpace(currentLine)))
            {
                Counter.IncrementIgnoreRecord();
                return false;
            }

            try
            {
                if (currentLine.Contains("ACCOUNT  NOT  UPDATED"))
                {
                    Counter.IncrementIgnoreRecord();
                    return false;
                }

                _firstLine = _location;
                if (currentLine.Contains("LOCATION"))
                {
                    if (!string.IsNullOrWhiteSpace(_location))
                    {
                        _location = currentLine;
                        return true;
                    }

                    _location = currentLine;
                    Counter.IncrementIgnoreRecord();
                    return false;
                }
                // read collector id (line contains no other useful info)
                if (currentLine.Contains("COLLECTOR ID"))
                {
                    _secondLine = currentLine;
                    Counter.IncrementIgnoreRecord();
                    return false;
                }

                // get file date
                DateTime filedate;
                if (DateTime.TryParse(currentLine.Substring2(27, 8), out filedate))
                {
                  //  _thirdLine = filedate.ToString(CultureInfo.InvariantCulture);
                    Counter.IncrementIgnoreRecord();
                    return false;
                }

                var value = ulong.Parse(currentLine.Substring2(1, 16));
                if (value == 0)
                {
                    Counter.IncrementIgnoreRecord();
                    return false;
                }

                _currentLine = currentLine;

                return true;

            }
            catch (Exception)
            {
                Counter.IncrementIgnoreRecord();
                return false;
            }
        }

        private void PopulateDefault(CacsActivity record)
        {
            if (record.FileDate < DateTime.Now.AddYears(-1))
                record.FileDate = DateTime.Today;
        }

        private bool IsRecordValid(CacsActivity record)
        {
            var accLength = record.AccountNo.ToString(CultureInfo.InvariantCulture).Length;
            if (!((accLength == 8) || (accLength == 11) || (accLength == 16)))
            {
                Logger.Debug(string.Format("Account Number :{0} has length {1} is not valid", record.AccountNo, accLength));
                return false;
            }

            if (record.ActivityCode.Length != 3)
            {
                Logger.Debug(string.Format("For Account Number {0}, Activity code {1} has length {2} is not valid"
                                        , record.AccountNo, record.ActivityCode, record.ActivityCode.Length));
                return false;
            }

            if (string.IsNullOrWhiteSpace(record.CallDirection)
                || string.IsNullOrWhiteSpace(record.CallLocation)
                || string.IsNullOrWhiteSpace(record.CallResponce))
            {
                return false;
            }

            return true;
        }

        public override CacsActivity GetRecordForUpdate()
        {
            throw new NotImplementedException();
        }

        #region Helper
        private static ScbEnums.Products DecodeProduct(UInt64 code)
        {
            switch (code)
            {
                case 80101: //return "Cards";
                case 80102: //return "Cards";
                case 80103: //return "Cards";
                case 80104: //return "Cards";
                case 80105: //return "Cards";
                case 80106: //return "Cards";
                case 80107: //return "Cards";
                case 80108: //return "Cards";
                case 80109: //return "Cards";
                case 80110: //return "Cards";
                case 80111: //return "Cards";
                    return ScbEnums.Products.CC;

                case 81001: //return "PL";
                case 81002: //return "PL";
                case 81003: //return "PL";
                case 81004: //return "PL";

                case 81301: //return "AEB_PL";
                case 81302: //return "AEB_PL";
                case 81303: //return "AEB_PL";
                case 81304: //return "AEB_PL";
                    return ScbEnums.Products.PL;

                //return EnumHelper.Subproduct.AEBPL;

                case 81801: //return "Mortgage";
                case 81802: //return "Mortgage";
                case 81803: //return "Mortgage";
                case 81804: //return "Mortgage";
                    return ScbEnums.Products.MORT;

                case 81401: //return "Auto";
                case 81402: //return "Auto";
                case 81403: //return "Auto";
                case 81404: //return "Auto";
                    return ScbEnums.Products.AUTO_OD;

                case 82401: //return "Auto_OD";
                case 82402: //return "Auto_OD";
                case 82403: //return "Auto_OD";
                case 82404: //return "Auto_OD";
                    return ScbEnums.Products.AUTO_OD;

                case 82001: //return "SMC";
                case 82002: //return "SMC";
                case 82003: //return "SMC";
                case 82004: //return "SMC";
                    return ScbEnums.Products.SMC;

                case 83801: //return "SMEBIL";
                case 83802: //return "SMEBIL";
                case 83803: //return "SMEBIL";
                case 83804: //return "SMEBIL";
                    return ScbEnums.Products.SME_BIL;
                default:
                    throw new InvalidCastException("Product not Find in Cacs Activity Exception");
            }

            //return null;
        }

        private static string DecodeRegion(UInt64 code)
        {
            switch (code)
            {
                case 80101: return "BL";
                case 80102: return "BY";
                case 80103: return "CA";
                case 80104: return "ND";
                case 80105: return "MA";
                case 80106: return "CB";
                case 80107: return "BY";
                case 80108: return "PU";
                case 80109: return "AH";
                case 80110: return "VA";
                case 80111: return "CO";

                case 81001: return "East";
                case 81002: return "West";
                case 81003: return "North";
                case 81004: return "South";

                case 81301: return "East";
                case 81302: return "West";
                case 81303: return "North";
                case 81304: return "South";

                case 81401: return "East";
                case 81402: return "West";
                case 81403: return "North";
                case 81404: return "South";

                case 81801: return "East";
                case 81802: return "West";
                case 81803: return "North";
                case 81804: return "South";

                case 82401: return "East";
                case 82402: return "West";
                case 82403: return "North";
                case 82404: return "South";

                case 82001: return "East";
                case 82002: return "West";
                case 82003: return "North";
                case 82004: return "South";

                case 83801: return "East";
                case 83802: return "West";
                case 83803: return "North";
                case 83804: return "South";
            }

            return string.Empty;
        }
        #endregion
    }
}
