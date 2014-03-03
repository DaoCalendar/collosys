#region references

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.FileUploadService.Interfaces;
using ColloSys.Shared.SharedUtils;
using NLog;

#endregion

namespace ColloSys.FileUploadService.TextReader
{
    internal class CacsActivityReader : MultiLineTextReader<CacsActivity>
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        #region constructor

        public CacsActivityReader(FileScheduler file)
            : base(file, new FileReaderProperties())
        {
            //var entities = Reader.GetDataLayer.GetDataForDate<CacsActivity>(file.FileDate.Date);
            //entities.ForEach(x => TodayRecordList.Add(x.AccountNo, x));
        }

        #endregion

        #region Read Next Batch

        private string _location = string.Empty;

        protected override List<TextFileRow<string[]>> GetNextBatch()
        {
            _logger.Debug("ActualUpload: Fetching next batch.");
            var rowList = new List<TextFileRow<string[]>>();

            var secondLine = string.Empty;
            var thirdLine = string.Empty;

            lock (InputFileStream)
            {
                while (!HasEofReached())
                {
                    // read the record
                    try
                    {
                        LineNo++;
                        var currentLine = InputFileStream.ReadLine();
                        if ((currentLine == null) || (string.IsNullOrWhiteSpace(currentLine)))
                        {
                            Reader.Counter.AddIgnoredRecord(LineNo);
                            continue;
                        }

                        // read the record
                        if (currentLine.Contains("ACCOUNT  NOT  UPDATED"))
                        {
                            Reader.Counter.AddIgnoredRecord(LineNo);
                            continue;
                        }

                        var firstLine = _location;
                        if (currentLine.Contains("LOCATION"))
                        {
                            if (!string.IsNullOrWhiteSpace(_location))
                            {
                                _location = currentLine;
                                return rowList;
                            }

                            _location = currentLine;
                            Reader.Counter.AddIgnoredRecord(LineNo);
                            continue;
                        }

                        // read collector id (line contains no other useful info)
                        if (currentLine.Contains("COLLECTOR ID"))
                        {
                            secondLine = currentLine;
                            Reader.Counter.AddIgnoredRecord(LineNo);
                            continue;
                        }

                        // get file date
                        DateTime filedate;
                        if (DateTime.TryParse(currentLine.Substring2(27, 8), out filedate))
                        {
                            thirdLine = filedate.ToString(CultureInfo.InvariantCulture);
                            Reader.Counter.AddIgnoredRecord(LineNo);
                            continue;
                        }

                        // ReSharper disable ReturnValueOfPureMethodIsNotUsed
                        var value = ulong.Parse(currentLine.Substring2(1, 16));
                        if (value == 0)
                        {
                            Reader.Counter.AddIgnoredRecord(LineNo);
                            continue;
                        }
                        // ReSharper restore ReturnValueOfPureMethodIsNotUsed

                        var dataRow = new[] { firstLine, secondLine, thirdLine, currentLine };

                        rowList.Add(new TextFileRow<string[]> { LineNo = LineNo, RowValue = dataRow });
                    }
                    // ReSharper disable EmptyGeneralCatchClause
                    catch (Exception)
                    {
                        Reader.Counter.AddIgnoredRecord(LineNo);
                    }
                    // ReSharper restore EmptyGeneralCatchClause
                }
            }

            _logger.Debug("ActualUpload: Returning batch of #records - " + rowList.Count);
            return rowList;
        }

        #endregion

        #region Handle Unqiue Record

        //protected override CacsActivity GetByUniqueKey(CacsActivity record)
        //{
        //    if (!Reader.FileDateList.Contains(record.FileDate.Date))
        //    {
        //        var newList = Reader.GetDataLayer.GetDataForDate<CacsActivity>(record.FileDate.Date);
        //        Reader.FileDateList.Add(record.FileDate.Date);

        //        OldDbRecordList.AddEntities(newList);
        //        //foreach (var cacsActivity in newList)
        //        //{
        //        //    OldDbRecordList.Add(cacsActivity);
        //        //}
        //    }


        //    return OldDbRecordList.SingleOrDefault(x => x.TelecallerId == record.TelecallerId
        //                                              && x.AccountNumber == record.AccountNo
        //                                              && x.CallDateTime == record.CallDateTime);
        //}

        protected override bool PerformUpdates(CacsActivity record)
        {
            return false;
        }

        #endregion

        #region Generate Record

        protected override CacsActivity GetRecord(string[] row)
        {
            var firstLine = row[0];
            var secondLine = row[1];
            //var thirdLine = row[2];
            var forthLine = row[3];

            var record = new CacsActivity();

            // read location from first line
            var location = Convert.ToUInt64(firstLine.Substring2(11, 6).ToString(CultureInfo.InvariantCulture));
            record.Products = DecodeProduct(location);
            record.Region = DecodeRegion(location);

            // read telecaller id from second line
            record.TelecallerId = secondLine.Substring2(14, 8);

            // read file date from third line
            record.FileDate = Reader.UploadedFile.FileDate.Date;

            // get other properties from forth line
            record.AccountNo = ulong.Parse(forthLine.Substring2(1, 16)).ToString(CultureInfo.InvariantCulture);
            record.CallDirection = forthLine.Substring2(51, 2); //AC
            record.CallLocation = forthLine.Substring2(54, 1); //P
            record.CallResponce = forthLine.Substring2(56, 2); //C
            record.ActivityCode = forthLine.Substring2(58, 3); //RTE

            if (!string.IsNullOrWhiteSpace(forthLine.Substring2(70, 11)))
                record.Ptp1Amt = Convert.ToDecimal(forthLine.Substring2(70, 11));

            if (!string.IsNullOrWhiteSpace(forthLine.Substring2(83, 5)))
                record.Ptp1Date = Utilities.ParseNullableDateTime(forthLine.Substring2(83, 5), "dd/MM");

            if (!string.IsNullOrWhiteSpace(forthLine.Substring2(92, 14)))
                record.Ptp2Amt = Convert.ToDecimal(forthLine.Substring2(92, 14));

            if (!string.IsNullOrWhiteSpace(forthLine.Substring2(107, 5)))
                record.Ptp2Date = Utilities.ParseNullableDateTime(forthLine.Substring2(107, 5), "dd/MM");

            record.ExcuseCode = forthLine.Substring2(118, 1);

            // convert time from singapore to india
            var datetime = Utilities.ParseDateTime(forthLine.Substring2(35, 14), "dd/MM/yy HH:mm");
            var singapore = TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time");
            record.CallDateTime = TimeZoneInfo.ConvertTime(datetime, singapore, TimeZoneInfo.Local);
            // store the duration in seconds
            var duraion = Utilities.ParseDateTime(forthLine.Substring2(124, 5), "m:ss");
            record.CallDuration = (uint)(duraion.Minute * 60 + duraion.Second);

            //records to be allocated
            var consideredActivityCodes = new[] { "H70", "M70", "F70" };
            record.ConsiderInAllocation = consideredActivityCodes.Contains(record.ActivityCode);

            return record;
        }

        protected override void PopulateDefault(CacsActivity record)
        {
            if (record.FileDate < DateTime.Now.AddYears(-1))
                record.FileDate = DateTime.Today;
        }

        protected override bool IsRecordValid(CacsActivity record)
        {
            var accLength = record.AccountNo.ToString(CultureInfo.InvariantCulture).Length;
            if (!((accLength == 8) || (accLength == 11) || (accLength == 16)))
            {
                _logger.Debug(string.Format("Account Number :{0} has length {1} is not valid", record.AccountNo, accLength));
                return false;
            }

            if (record.ActivityCode.Length != 3)
            {
                _logger.Debug(string.Format("For Account Number {0}, Activity code {1} has length {2} is not valid"
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

        protected override TextFileRow<string[]> GetNextRow()
        {
            throw new NotImplementedException();
        }

        #endregion

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
