using System;
using System.Collections.Generic;
using System.IO;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;
using ColloSys.FileUploaderService.DbLayer;
using ColloSys.FileUploaderService.RowCounter;
using NLog;

namespace ColloSys.FileUploaderService.RecordManager
{
    public abstract class TextRecordCreator<TEntity> : ITextRecord<TEntity>
        where TEntity : Entity, new()
    {
        public FileScheduler FileScheduler;
        public ICounter Counter;
        public StreamReader InpuStreamReader;
        protected readonly IDbLayer DbLayer = new DbLayer.DbLayer();
        public readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public void Init(FileScheduler fileScheduler, ICounter counter)
        {
            var info = new FileInfo(fileScheduler.FileDirectory + @"\" + fileScheduler.FileName);
            FileScheduler = fileScheduler;
            Counter = counter;
            InpuStreamReader = new StreamReader(info.FullName);
        }

        public abstract bool CreateRecord( out TEntity obj);


        public bool CreateRecord(IList<FileMapping> mapings, out TEntity obj)
        {
            throw new NotImplementedException();
        }
    

    public IList<TEntity> YesterdayRecords { get; set; }

       public bool EndOfFile()
       {
           if (InpuStreamReader==null|| InpuStreamReader.EndOfStream)
           {
              return true;
           }
           return false;
       }

       public uint CurrentRow
       {
           get { return Counter.CurrentRow; }
       }

       public abstract TEntity GetRecordForUpdate();

       public void InitPreviousDayLiner(FileScheduler fileScheduler)
       {
           //if (HasMultiDayComputation)
           //    YesterdayRecords = DbLayer.GetDataForPreviousDay<TEntity>(fileScheduler.FileDetail.AliasName,
           //       fileScheduler.FileDate, fileScheduler.FileDetail.FileCount);

       }


    }
}
