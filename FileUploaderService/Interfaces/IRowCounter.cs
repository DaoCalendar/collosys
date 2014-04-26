using System.Collections.Generic;
using ColloSys.DataLayer.Domain;

namespace ColloSys.FileUploadService.Interfaces
{
    public enum LineStatus
    {
        Ignored,
        Valid,
        Error
    }

    public enum RecordStatus
    {
        Ignored,
        Valid,
        Duplicate,
        Error,
        Uploaded
    }

    public interface IRowCounter
    {
        #region row

        // ReSharper disable ReturnTypeCanBeEnumerable.Global
        IDictionary<ulong, LineStatus> LineNoStatus { get; }
        // ReSharper restore ReturnTypeCanBeEnumerable.Global

        ulong GetTotalLineCount();

        ulong GetIgnoredLineCount();

        ulong GetValidLineCount();

        ulong GetErrorLineCount();

        void AddIgnoredLine(ulong lineNo);

        void AddValidLine(ulong lineNo);

        void AddErroLine(ulong lineNo);

        string GetLineStatusAsString();

        #endregion

        #region record

        // ReSharper disable ReturnTypeCanBeEnumerable.Global
        IDictionary<ulong, RecordStatus> RecordNoStatus { get; }
        // ReSharper restore ReturnTypeCanBeEnumerable.Global

        ulong GetTotalRecordCount();

        ulong GetValidRecordCount();

        ulong GetUploadedRecordCount();

        ulong GetDuplicateRecordCount();

        ulong GetErrorRecordCount();

        ulong GetIgnoredRecordCount();

        void InitializeCounter(FileStatus fileStatus);

        void AddValidRecord(ulong recordNo);

        void AddUploadRecord(ulong recordNo);

        void AddDuplicateRecord(ulong recordNo);

        void AddErrorRecord(ulong recordNo);

        void AddIgnoredRecord(ulong recordNo);

        string GetRecordStatusAsString();

        #endregion
    }
}
