#region references

using System;
using System.Collections.Generic;
using System.Linq;
using ColloSys.DataLayer.Domain;
using ColloSys.FileUploadService.Interfaces;

#endregion

namespace ColloSys.FileUploadService.Implementers
{
    class RowCounter : IRowCounter
    {
        #region ctor

        private readonly bool _hasPerLineRecord;

        public RowCounter(bool hasPerLineRecords)
        {
            LineNoStatus = new Dictionary<ulong, LineStatus>();
            RecordNoStatus = new Dictionary<ulong, RecordStatus>();

            _hasPerLineRecord = hasPerLineRecords;
        }

        #endregion

        #region print

        // row
        public IDictionary<ulong, LineStatus> LineNoStatus { get; private set; }
        public string GetLineStatusAsString()
        {
            var statusString = LineNoStatus.Aggregate(string.Empty,
                                                    (current, rowStatuse) =>
                                                    current + rowStatuse.Key + ":" + rowStatuse.Value + ",");
            LineNoStatus.Clear();
            return statusString;
        }

        // record
        public IDictionary<ulong, RecordStatus> RecordNoStatus { get; private set; }
        public string GetRecordStatusAsString()
        {
            if (RecordNoStatus.Count() > 10000)
            {
                var message = string.Format("Total Record Ignore : {0}", RecordNoStatus.Count());
                RecordNoStatus.Clear();
                return message;
            }

            var statusString = RecordNoStatus.Aggregate(string.Empty,
                                                    (current, recordStatuse) =>
                                                    current + recordStatuse.Key + ":" + recordStatuse.Value + ",");
            RecordNoStatus.Clear();
            return statusString;
        }

        #endregion

        #region line

        private ulong _lineIgnored;
        private ulong _lineValid;
        private ulong _lineError;

        public ulong GetTotalLineCount()
        {
            return _lineIgnored + _lineError + _lineValid;
        }

        public ulong GetIgnoredLineCount()
        {
            return _lineIgnored;
        }

        public ulong GetValidLineCount()
        {
            return _lineValid;
        }

        public ulong GetErrorLineCount()
        {
            return _lineError;
        }

        public void AddIgnoredLine(ulong rowNo)
        {
            if (_hasPerLineRecord)
            {
                throw new InvalidProgramException(
                    "Please use record counters & not row counters for perLineRecord files.");
            }

            _lineIgnored++;
            LineNoStatus.Add(rowNo, LineStatus.Ignored);
        }

        public void AddValidLine(ulong rowNo)
        {
            if (_hasPerLineRecord)
            {
                throw new InvalidProgramException(
                    "Please use record counters & not row counters for perLineRecord files.");
            }

            if (LineNoStatus.Count == int.MaxValue)
            {
                throw new InvalidProgramException("Please clear the rows dictionary. Its has grown too big in size.");
            }

            _lineValid++;
            LineNoStatus.Add(rowNo, LineStatus.Valid);
        }

        public void AddErroLine(ulong rowNo)
        {
            if (_hasPerLineRecord)
            {
                throw new InvalidProgramException(
                    "Please use record counters & not row counters for perLineRecord files.");
            }

            if (LineNoStatus.Count == int.MaxValue)
            {
                throw new InvalidProgramException("Please clear the rows dictionary. Its has grown too big in size.");
            }

            _lineError++;
            LineNoStatus.Add(rowNo, LineStatus.Error);
        }

        #endregion

        #region record

        private ulong _recordIgnored;
        private ulong _recordError;
        private ulong _recordValid;
        private ulong _recordDuplicate;
        private ulong _recordUploaded;

        public ulong GetTotalRecordCount()
        {
            return _recordValid + _recordError + _recordIgnored;//_recordDuplicate + +_recordUploaded;
        }

        public ulong GetIgnoredRecordCount()
        {
            return _recordIgnored;
        }

        public ulong GetErrorRecordCount()
        {
            return _recordError;
        }

        public ulong GetValidRecordCount()
        {
            return _recordValid;
        }

        public ulong GetDuplicateRecordCount()
        {
            return _recordDuplicate;
        }

        public ulong GetUploadedRecordCount()
        {
            return _recordUploaded;
        }

        public void InitializeCounter(FileStatus fileStatus)
        {
            if (fileStatus == null)
                return;

            _recordUploaded = fileStatus.UploadedRows;
            _recordDuplicate = fileStatus.DuplicateRows;
            _recordValid = fileStatus.ValidRows;
            _recordError = fileStatus.ErrorRows;
            _recordIgnored = fileStatus.IgnoredRows;
        }

        public void AddIgnoredRecord(ulong recordNo)
        {
            if (_hasPerLineRecord)
            {
                _lineIgnored++;
            }
            _recordIgnored++;

            if (RecordNoStatus.Count == int.MaxValue)
            {
                throw new InvalidProgramException("Please clear the records dictionary. Its has grown too big in size.");
            }

            if (RecordNoStatus.ContainsKey(recordNo))
            {
                RecordNoStatus.Remove(recordNo);
            }
            RecordNoStatus.Add(recordNo, RecordStatus.Ignored);
        }

        public void AddValidRecord(ulong recordNo)
        {
            if (_hasPerLineRecord)
            {
                _lineValid++;
            }
            _recordValid++;

            if (RecordNoStatus.Count == int.MaxValue)
            {
                throw new InvalidProgramException("Please clear the records dictionary. Its has grown too big in size.");
            }

            if (RecordNoStatus.ContainsKey(recordNo))
            {
                RecordNoStatus.Remove(recordNo);
            }
            RecordNoStatus.Add(recordNo, RecordStatus.Valid);
        }

        public void AddUploadRecord(ulong recordNo)
        {
            if (_hasPerLineRecord)
            {
                _lineValid++;
            }
            _recordUploaded++;

            if (RecordNoStatus.Count == int.MaxValue)
            {
                throw new InvalidProgramException("Please clear the records dictionary. Its has grown too big in size.");
            }

            if (RecordNoStatus.ContainsKey(recordNo))
            {
                RecordNoStatus.Remove(recordNo);
            }
            RecordNoStatus.Add(recordNo, RecordStatus.Uploaded);
        }

        public void AddErrorRecord(ulong recordNo)
        {
            if (_hasPerLineRecord)
            {
                _lineError++;
            }
            _recordError++;

            if (RecordNoStatus.Count == int.MaxValue)
            {
                throw new InvalidProgramException("Please clear the records dictionary. Its has grown too big in size.");
            }

            if (RecordNoStatus.ContainsKey(recordNo))
            {
                RecordNoStatus.Remove(recordNo);
            }
            RecordNoStatus.Add(recordNo, RecordStatus.Error);
        }

        public void AddDuplicateRecord(ulong recordNo)
        {
            if (_hasPerLineRecord)
            {
                _lineError++;
            }
            _recordDuplicate++;

            if (RecordNoStatus.Count == int.MaxValue)
            {
                throw new InvalidProgramException("Please clear the records dictionary. Its has grown too big in size.");
            }

            if (RecordNoStatus.ContainsKey(recordNo))
            {
                RecordNoStatus.Remove(recordNo);
            }
            RecordNoStatus.Add(recordNo, RecordStatus.Duplicate);
        }

        #endregion
    }
}
