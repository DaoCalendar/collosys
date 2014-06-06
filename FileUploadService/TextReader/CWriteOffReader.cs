#region references

using System.Globalization;
using ColloSys.DataLayer.Domain;
using System;
using System.Linq;
using ColloSys.DataLayer.Enumerations;
using ColloSys.FileUploadService.Interfaces;
using ColloSys.Shared.SharedUtils;
using NLog;

#endregion

namespace ColloSys.FileUploadService.TextReader
{
    internal class CWriteOffReader : MultiLineTextReader<CWriteoff>
    {
        #region constructor

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public CWriteOffReader(FileScheduler file)
            : base(file, new FileReaderProperties())
        {
            var entities = Reader.GetDataLayer.GetDataForDate<CWriteoff>(file.FileDate.Date);
            entities.ForEach(x => TodayRecordList.Add(x.AccountNo, x));
        }

        #endregion

        #region Read Next Row

        protected override TextFileRow<string[]> GetNextRow()
        {
            while (!HasEofReached())
            {
                try
                {
                    string firstLine;
                    string secondLine;
                    string thirdLine;

                    lock (InputFileStream)
                    {
                        LineNo++;
                        firstLine = InputFileStream.ReadLine();
                        if ((firstLine == null) || (string.IsNullOrWhiteSpace(firstLine)))
                        {
                            Reader.Counter.AddIgnoredRecord(LineNo);
                            continue;
                        }


                        var scbIndex = firstLine.IndexOf("STANDARD CHARTERED BANK INDIA   CURRENCY",
                                                          StringComparison.InvariantCulture);
                        if (scbIndex > 0)
                        {
                            //var filedatestring = firstLine.Substring2((uint)(103 + scbIndex), 8);
                            //Reader.UploadedFile.FileDate = Utilities.ParseDateTime(filedatestring, "dd/MM/yy");
                            Reader.Counter.AddIgnoredRecord(LineNo);
                            continue;
                        }

                        // get first line
                        // ReSharper disable ReturnValueOfPureMethodIsNotUsed
                        Convert.ToUInt64(firstLine.Substring2(1, 16));
                        // ReSharper restore ReturnValueOfPureMethodIsNotUsed
                        Utilities.ParseDateTime(firstLine.Substring2(96, 8), "dd/MM/yy");


                        // get second line
                        LineNo++;
                        secondLine = InputFileStream.ReadLine();
                        Utilities.ParseNullableDateTime(secondLine.Substring2(96, 8), "dd/MM/yy");

                        // get third line
                        LineNo++;
                        thirdLine = InputFileStream.ReadLine();
                        Utilities.ParseDateTime(thirdLine.Substring2(96, 8), "MM/yy");
                    }

                    return new TextFileRow<string[]>
                        {
                            LineNo = LineNo,
                            RowValue = new[] { firstLine, secondLine, thirdLine }
                        };
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

        #region Generate Record

        protected override CWriteoff GetRecord(string[] row)
        {
            var firstLine = row[0];
            var secondLine = row[1];
            var thirdLine = row[2];

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
            writeoff.ActivationDate = Utilities.ParseDateTime(firstLine.Substring2(96, 8), "dd/MM/yy");

            writeoff.Bucket1Due = Convert.ToDecimal(firstLine.Substring2(107, 2));
            writeoff.Bucket2Due = Convert.ToDecimal(firstLine.Substring2(111, 2));
            writeoff.Bucket3Due = Convert.ToDecimal(firstLine.Substring2(115, 2));
            writeoff.Bucket4Due = Convert.ToDecimal(firstLine.Substring2(119, 2));
            writeoff.Bucket5Due = Convert.ToDecimal(firstLine.Substring2(123, 2));
            writeoff.Bucket6Due = Convert.ToDecimal(firstLine.Substring2(127, 2));
            writeoff.Bucket7Due = Convert.ToDecimal(firstLine.Substring2(131, 2));


            // read second line
            writeoff.LastPayDate = Utilities.ParseNullableDateTime(secondLine.Substring2(96, 8), "dd/MM/yy");

            // read third line
            writeoff.ExpirtyDate = Utilities.ParseDateTime(thirdLine.Substring2(96, 8), "MM/yy");


            return writeoff;
        }

        protected override void PopulateDefault(CWriteoff record)
        {
            record.Product = ScbEnums.Products.CC;
        }

        protected override bool IsRecordValid(CWriteoff record)
        {
            return true;
        }

        #endregion

        #region Handle Unqiue Record

        protected override CWriteoff GetByUniqueKey(CWriteoff helper)
        {
            return TodayRecordList.ContainsKey(helper.AccountNo) ? TodayRecordList[helper.AccountNo] : null;
        }

        protected override bool PerformUpdates(CWriteoff helper)
        {
            return true;
        }

        public override bool PostProcesing()
        {
            var entities = Reader.GetDataLayer.GetDataForDate<CLiner>(Reader.UploadedFile.FileDate.Date);

            // write off account becomes 'Z'
            var ztodayLiner = entities.Where(x => TodayRecordList.ContainsKey(x.AccountNo)).ToList();
            ztodayLiner.ForEach(x => x.Flag = ColloSysEnums.DelqFlag.Z);

            // 150 DPD account becomes 'Z'
            var blockList = new string[] { "S", "F" };
            var ztodayLinerby150Dbd = entities.Where(x => x.Bucket == 5 && x.Cycle <= Reader.UploadedFile.FileDate.Day
                                                          && !blockList.Contains(x.Block)
                                                          && x.Flag != ColloSysEnums.DelqFlag.Z)
                                              .ToList();
            ztodayLinerby150Dbd.ForEach(x => x.Flag = ColloSysEnums.DelqFlag.Z);
            ztodayLiner.AddRange(ztodayLinerby150Dbd);

            // yday Z then today Z
            var yesterdayLiner = Reader.GetDataLayer
                                       .GetDataForPreviousDay<CLiner>(ColloSysEnums.FileAliasName.C_LINER_COLLAGE,
                                                                      Reader.UploadedFile.FileDate.Date, (uint)7);
            var zAccountOfYesterday = yesterdayLiner.Where(x => x.Flag == ColloSysEnums.DelqFlag.Z)
                                                    .Select(x => x.AccountNo)
                                                    .ToList();
            var ztodayLinerbyYesterdayZ = entities.Where(x => zAccountOfYesterday.Contains(x.AccountNo)
                                                              && x.Flag != ColloSysEnums.DelqFlag.Z);
            ztodayLiner.AddRange(ztodayLinerbyYesterdayZ);



            // global cust id
            var blockList2 = new string[] {"L", "S", "F", "Q"};
            var zGcustids = ztodayLiner.Select(x => x.GlobalCustId);
            var zGcutIdLiner = entities.Where(x => zGcustids.Contains(x.GlobalCustId)
                                                   && x.Flag != ColloSysEnums.DelqFlag.Z
                                                   && !blockList2.Contains(x.Block))
                                       .ToList();
            zGcutIdLiner.ForEach(x => x.Flag = ColloSysEnums.DelqFlag.Z);
            ztodayLiner.AddRange(zGcutIdLiner);

            Reader.GetDataLayer.SaveOrUpdateData(ztodayLiner);
            
            return true;
        }
        #endregion
    }
}