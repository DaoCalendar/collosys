using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.SessionMgr;
using ColloSys.FileUploader.AliasFileReader;
using ColloSys.FileUploader.DbLayer;
using ColloSys.FileUploader.RowCounter;
using ColloSys.FileUploadService;
using FileUploaderService;
using NHibernate.Cfg.ConfigurationSchema;
using NHibernate.Linq;
using NUnit.Framework;
using ReflectionExtension.ExcelReader;
using ReflectionExtension.Tests.DataCreator.FileUploader;
using FileUploaderService = ColloSys.FileUploadService.FileUploaderService;

namespace ReflectionExtension.Tests.ExcelReader.Test
{
    public interface IRowReader<in TEntity> where TEntity : class, new()
    {
        void Create(TEntity a);
        int Computed(TEntity a);
    }

    public abstract class RowReader<TEntity> : IRowReader<TEntity> where TEntity : class, new()
    {
        public abstract int Computed(TEntity a);

        public void Create(TEntity b)
        {
            Computed(b);
        }
    }

    public class RlsPaymentRowReader : RowReader<Payments>
    {
        public override int Computed(Payments a)
        {
            return a.Dummy = 5;
        }
    }


    interface IFileReader<in TEntity> where TEntity : class,new()
    {
        void Read(TEntity obj);
    }

    public abstract class FileReader<TEntity> : IFileReader<TEntity> where TEntity : class , new()
    {
        private readonly IRowReader<TEntity> _dummy;

        protected FileReader(IRowReader<TEntity> obj)
        {
            _dummy = obj;
        }

        public void Read(TEntity obj1)
        {
            _dummy.Create(obj1);
        }
    }

    public class RlsPaymentFileReader1 : FileReader<Payments>
    {
        public RlsPaymentFileReader1() : base(new RlsPaymentRowReader()) { }
    }

    public class Payments
    {
        public int Dummy { get; set; }
    }

    [TestFixture]
    public class testGetNxtRw
    {
        private IDbLayer obj;
        private ColloSys.FileUploader.FileReader.IFileReader<Payment> _reader;
        readonly FileMappingData _data=new FileMappingData();
        private ICounter counter;
        [SetUp]
        public void Init()
        {
            obj = new DbLayer();
            counter=new ExcelRecordCounter();
            var scheduler = _data.GetUploadedFile();
            _reader =new RlsPaymentLinerFileReader(scheduler);
        }


        [Test]
        public void Test()
        {
            //_reader.Save();
            //var session = SessionManager.GetCurrentSession();

            //using (var transaction = session.BeginTransaction())
            //{
            //    session.Save("");

            //    transaction.Commit();
            //}

           // obj.GetNextFileForSchedule();
            global::FileUploaderService.FileUploaderService.UploadFiles();
        }
    }
}


