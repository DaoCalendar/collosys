#region references

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Web.Http;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.SharedDomain;
using ColloSys.Shared.ExcelWriter;
using ColloSys.Shared.Types4Product;
using NHibernate.Criterion;
using NLog;

#endregion

namespace AngularUI.FileUpload.uploadpincode
{
    public class FileData
    {
        public string FileName { get; set; }
        public int Count { get; set; }
    }

    public class PincodeUploadApiController : ApiController
    {

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [HttpGet]
        public HttpResponseMessage FetchProducts()
        {
            var values = Enum.GetValues(typeof(ScbEnums.Products));
            var list = values.Cast<ScbEnums.Products>().ToList()
                             .Where(x => x.ToString() != ScbEnums.Products.UNKNOWN.ToString())
                             .OrderBy(x => x.ToString());
            return Request.CreateResponse(HttpStatusCode.OK, list);
        }

        [HttpGet]
        public HttpResponseMessage FetchMissingPincodes(ScbEnums.Products product)
        {

            var session = SessionManager.GetCurrentSession();
            var infotype = ClassType.GetInfoType(product);
            Logger.Info(string.Format("FileDownload : MissingPincode : get missing pincodes for : {0}, from table : {1}"
                                      , product, infotype.Name));

            IList result;
            try
            {
                var memberhelper = new MemberHelper<CustomerInfo>();
                var pincodename = memberhelper.GetName(x => x.Pincode);
                var accnoname = memberhelper.GetName(x => x.AccountNo);
                var custname = memberhelper.GetName(x => x.CustomerName);
                var productname = memberhelper.GetName(x => x.Product);

                var criteria = session.CreateCriteria(infotype, infotype.Name);
                criteria.Add(Restrictions.Or(Restrictions.IsNull(string.Format("{0}.{1}", infotype.Name, pincodename)),
                                             Restrictions.Eq(string.Format("{0}.{1}", infotype.Name, pincodename), (uint)0)));
                criteria.Add(Restrictions.Eq(string.Format("{0}.{1}", infotype.Name, productname), product));
                criteria.SetProjection(Projections.ProjectionList()
                                                  .Add(Projections.Property(custname))
                                                  .Add(Projections.Property(accnoname))
                                                  .Add(Projections.Property(pincodename))
                    );
                result = criteria.List();
                Logger.Info(string.Format("FileDownload : MissingPincode : count : {0}", result.Count));
            }
            catch (Exception exception)
            {
                Logger.ErrorException("FileDownload : MissingPincode : could not fetch data. ", exception);
                throw new ExternalException("Could not generate excel. " + exception.Message);
            }

            var fileInfo = new FileInfo(string.Format(@"{2}\{0}_MissingPincodes_{1}.xlsx"
                                                      , product
                                                      , DateTime.Now.ToString("yyyyMMdd_HHmmssfff")
                                                      , Path.GetTempPath()));

            try
            {
                IList<PincodeInfo> infolist = result.OfType<object[]>()
                                                    .Select(e => new PincodeInfo
                                                        {
                                                            AccountNo = e[1].ToString(),
                                                            CustomerName = e[0].ToString(),
                                                            Pincode = string.Empty
                                                        })
                                                    .ToList();
                ExcelWriter.ListToExcel(infolist, fileInfo);
            }
            catch (Exception exception)
            {
                Logger.ErrorException("FileStatus : could not generate excel. ", exception);
                throw new ExternalException("Could not generate excel. " + exception.Message);
            }

            var data = new FileData() { FileName = fileInfo.FullName, Count = result.Count };
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpGet]
        public HttpResponseMessage FetchMissingRcodes(ScbEnums.Products product)
        {
            var session = SessionManager.GetCurrentSession();
            var infotype = ClassType.GetInfoType(product);
            Logger.Info(string.Format("FileDownload : MissingRcode : get missing rcodes for : {0}, from table : {1}"
                                      , product, infotype.Name));

            IList result;
            try
            {
                var memberhelper = new MemberHelper<CustomerInfo>();
                var rcodename = memberhelper.GetName(x => x.CustStatus);
                var accnoname = memberhelper.GetName(x => x.AccountNo);
                var custname = memberhelper.GetName(x => x.CustomerName);
                var productname = memberhelper.GetName(x => x.Product);

                var criteria = session.CreateCriteria(infotype, infotype.Name);
                criteria.Add(Restrictions.Or(Restrictions.IsNull(string.Format("{0}.{1}", infotype.Name, rcodename)),
                                             Restrictions.Eq(string.Format("{0}.{1}", infotype.Name, rcodename), string.Empty)));
                criteria.Add(Restrictions.Eq(string.Format("{0}.{1}", infotype.Name, productname), product));
                criteria.SetProjection(Projections.ProjectionList()
                                                  .Add(Projections.Property(accnoname))
                                                  .Add(Projections.Property(custname))
                                                  .Add(Projections.Property(rcodename))
                    );
                result = criteria.List();
                Logger.Info(string.Format("FileDownload : MissingRcode : count : {0}", result.Count));
            }
            catch (Exception exception)
            {
                Logger.ErrorException("FFileDownload : MissingRcode : could not fetch data. ", exception);
                throw new ExternalException("Could not generate excel. " + exception.Message);
            }

            var fileInfo = new FileInfo(string.Format(@"{2}\{0}_MissingRcodes_{1}.xlsx"
                                                      , product
                                                      , DateTime.Now.ToString("yyyyMMdd_HHmmssfff")
                                                      , Path.GetTempPath()));

            try
            {
                IList<RcodeInfo> infolist = (from object entity in result
                                             select entity as object[]
                                                 into e
                                                 select new RcodeInfo
                                                 {
                                                     AccountNo = e[0].ToString(),
                                                     CustomerName = e[1].ToString(),
                                                     Rcode = string.Empty
                                                 })
                    .ToList();
                ExcelWriter.ListToExcel(infolist, fileInfo);
            }
            catch (Exception exception)
            {
                Logger.ErrorException("FileStatus : could not generate excel. ", exception);
                throw new ExternalException("Could not generate excel. " + exception.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK, fileInfo.FullName);
        }

        [HttpPost]
        public HttpResponseMessage UploadPincode(UploadInfo upload)
        {
            var fileInfo = new FileInfo(upload.FileName);
            if (!fileInfo.Exists) throw new FileNotFoundException("Not Found", upload.FileName);
            var count = PincodeUploadHelper.ReadPincodeExcel(upload.Product, fileInfo);
            return Request.CreateResponse(HttpStatusCode.OK, count);
        }

        [HttpPost]
        public HttpResponseMessage UploadRcode(UploadInfo upload)
        {
            var fileInfo = new FileInfo(upload.FileName);
            if (!fileInfo.Exists) throw new FileNotFoundException("Not Found", upload.FileName);
            var count = RcodeUploadHelper.ReadRcodeExcel(upload.Product, fileInfo);
            return Request.CreateResponse(HttpStatusCode.OK, count);
        }
    }
}