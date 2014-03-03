#region references

using ColloSys.DataLayer.Domain;
using ColloSys.FileUploadService.Interfaces;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;

#endregion

namespace ColloSys.FileUploadService.TextReader
{
    public class TextFileRow<T> where T : class
    {
        public ulong LineNo { get; set; }
        public T RowValue { get; set; }
    }

    internal abstract class TextReader<TEntity> : IFileReaderNeeds where TEntity : class
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        #region constructor

        private IList<TextFileRow<TEntity>> _rowQueue;
        protected readonly IFileReader Reader;
        protected ulong LineNo;
        protected ulong RowNo;

        protected TextReader(FileScheduler file, FileReaderProperties props)
        {
            Reader = new FileReaderBase();
            Reader.InitFileReader(file, props, this);
            _rowQueue = new List<TextFileRow<TEntity>>();
            LineNo = 0;
            RowNo = 0;
        }

        #endregion

        #region abstract member

        public void UploadFile()
        {
            Reader.UploadFile();
        }

        protected abstract TextFileRow<TEntity> GetNextRow();

        public abstract void SaveRowList(out string errorMessage);

        void IFileReaderNeeds.SaveErrorTable()
        {
        }

        public virtual bool PostProcesing()
        {
            return true;
        }
        #endregion

        #region read file

        protected StreamReader InputFileStream { get; private set; }
        public bool ReadFile(out string errorMessage)
        {
            errorMessage = string.Empty;
            try
            {
                InputFileStream = new StreamReader(Reader.GetInputFile.FullName);
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        public bool HasEofReached()
        {
            return InputFileStream == null || InputFileStream.EndOfStream;
        }

        #endregion

        #region Read Next Batch

        protected virtual List<TextFileRow<TEntity>> GetNextBatch()
        {
            var rowList = new List<TextFileRow<TEntity>>();

            _log.Info("ActualUpload: get next batch.");
            for (long i = 0; rowList.Count < Reader.GetBatchSize && !HasEofReached(); i++)
            {
                // read the record
                try
                {
                    var dataRow = GetNextRow();
                    if (dataRow == null)
                        continue;

                    rowList.Add(dataRow);
                }
                // ReSharper disable EmptyGeneralCatchClause
                catch (Exception)
                {
                }
            }
            return rowList;
        }

        #endregion

        #region Retry Error Rows

        public bool RetryErrorRows()
        {
            return true;
        }

        #endregion

        #region locking/unlocking

        public void EnqueueRowList()
        {
            _log.Info("ActualUpload: get next batch.");
            _rowQueue = GetNextBatch();
        }

        protected IList<TextFileRow<TEntity>> DequeueRowList()
        {
            // get string list from queue
            _log.Info("ActualUpload: save the batch.");
            return _rowQueue;
        }
        #endregion

    }
}
