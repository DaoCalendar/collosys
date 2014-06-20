#region references

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.FileUploaderService.RecordManager;

#endregion

namespace ColloSys.FileUploaderService.AliasLiner.Rls
{
// ReSharper disable once InconsistentNaming
    public abstract class RLinerSharedRC: RecordCreator<RLiner>
    {
        private uint AccountPos;
        private uint AccountNoLength;

        protected RLinerSharedRC(uint accPos,uint accNoLength )
        {
            AccountPos = accPos;
            AccountNoLength = accNoLength;
        }

        public override bool IsRecordValid(RLiner entity)
        {
            return true;
        }

        public override bool CheckBasicField()
        {
            // loan no should be a number
            ulong loanNumber;
            if (!ulong.TryParse(Reader.GetValue(AccountPos), out loanNumber))
            {
                Counter.IncrementIgnoreRecord();
                return false;
            }

            // loan number must be of 2 digits min
            return (loanNumber.ToString(CultureInfo.InvariantCulture).Length >= 2);
        }

        public override RLiner GetRecordForUpdate()
        {
            return new RLiner();
        }

        public override RLiner GetPreviousDayEntity(RLiner entity)
        {
            return PreviousDayLiner.SingleOrDefault(x => x.AccountNo == entity.AccountNo);
        }

        protected uint GetRLinerBucketNumber(string bucket)
        {
            bucket = bucket.Replace(",", "");

            if (FileScheduler.FileDetail.AliasName == ColloSysEnums.FileAliasName.E_LINER_OD_SME)
            {
                if (string.IsNullOrWhiteSpace(bucket) || Convert.ToUInt32(bucket) == 0)
                    return 1;

                return (Convert.ToUInt32(bucket) / 30) + 1;
            }

            uint bucketNo;
            var data = new Dictionary<string, uint>();
            for (uint i = 65, j = 1; i <= 90; i++, j++)
            {
                data.Add(((char)i).ToString(CultureInfo.InvariantCulture), j);
            }
            if (Regex.IsMatch(bucket, @"^[a-zA-Z]+$"))
            {
                bucketNo = data[bucket.ToUpper()];
            }
            else
            {
                throw new FormatException("Given Bucket is not in well format");
            }
            return bucketNo - 1;
        }

        public void GetComputetions(RLiner entity)
        {
            if (entity.ImpairmentFlag == "Y")
            {
                entity.IsImpaired = true;
            }
            else
            {
                entity.IsImpaired = false;
                
            }
        }
    }
}
