namespace ColloSys.FileUploadService.Interfaces
{
    public class FileReaderProperties
    {
        public FileReaderProperties()
        {
            CsvDelimiter = ",";
        }

        public bool HasMultiLineRecords { get; set; }
        
        //private bool? _hasFileDateInsideFile;
        //public bool HasFileDateInsideFile
        //{
        //    get
        //    {
        //        // if value is not set, then _hasFileDateInsideFile is by default false
        //        return (_hasFileDateInsideFile != null) && _hasFileDateInsideFile.Value;
        //    }
        //    set
        //    {
        //        _hasFileDateInsideFile = value;
        //        //if _hasFileDateInsideFile is se to false, then we have only one date from filescheduler
        //        // else if it is inside the file, we should be able to deal with multiple dates
        //        _hasDataForMultipleDates = value;
        //    }
        //}

        //private bool? _hasDataForMultipleDates;
        //public bool HasDataForMultipleDates
        //{
        //    get
        //    {
        //        // if value is not set, then _hasDataForMultipleDates is by default true
        //        return (_hasDataForMultipleDates != null) && _hasDataForMultipleDates.Value;
        //    }
        //}

        public string CsvDelimiter { get; set; }
    }
}