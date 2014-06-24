using System;
using System.Collections.Generic;
using System.IO;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;
using ColloSys.FileUploader.RowCounter;

namespace ColloSys.FileUploaderService.RecordManager
{
    public abstract class TextRecordCreator<TEntity> : ITextRecord<TEntity>
        where TEntity : Entity, new()
    {
        public FileScheduler FileScheduler;
        public ICounter Counter;
        public StreamReader InpuStreamReader;

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

       }


    }
}
