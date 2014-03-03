using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.FileUploader;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.FileUploadService.BaseReader;
using ColloSys.FileUploadService.Excel2DT;
using ColloSys.Shared.ConfigSectionReader;
using ColloSys.UserInterface.Shared.Attributes;
using UserInterfaceAngular.Filters;

namespace ColloSys.UserInterface.Areas.FileUploader.Controllers
{
    public class UploadActualDataController : Controller
    {
        //
        // GET: /FileUploader/UploadActualData/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [UserActivity(Activity = ColloSysEnums.Activities.Development)]
        [MvcTransaction]
        public ActionResult Upload()
        {
            var files = Request.Files.Count;
            if (files == 0)
                return View("Index");

            var file = Request.Files["uploadFile"];
            var dir = System.IO.Path.GetTempPath();

            if (file == null)
                return View("Index");

            file.SaveAs(dir + file.FileName);

            var fileInfo = new FileInfo(dir + file.FileName);

            try
            {
                UploadData(fileInfo);
                ViewBag.Success = "Data saved";
            }
            catch (Exception exception)
            {
                ViewBag.Error = exception.ToString();
            }


            return View("Index");
        }

        private void UploadData(FileInfo fileInfo)
        {
            if (!fileInfo.Exists)
                return;

            var fileScheduler = CreateFileScheduler(fileInfo);
            var dataTable = NPOIExcelReader.GenerateCLinerDataTable(fileScheduler, fileInfo);

            var view = new System.Data.DataView(dataTable);

            // select columns of CLiner
            var cLinerDataTable = view.ToTable(false, new[] { "CLinerId", "Version", "CreatedBy", "CreatedOn", "CreateAction",
                                            "Flag", "Custno", "Cardno", "Name", "Cycle", "Limit", "Unbill", "CurBal", "Totbal", "PKBKT",
                                            "AcType", "Bucket", "BktAmt", "Block", "Alt_Blk", "LpmntAmt", "LPMNTDT", "Cur_Due", 
                                            "Days_X", "Days_30", "Days_60", "Days_90", "Days_120", "Days_150", "Total_Due", 
                                            "State", "delqhist", "custot","CustStatus","FileDate","FileRowNo","IsReferred",
                                            "Pincode","Product","GPincodeId","AllocStatus","NoAllocResons","FileSchedulerId" });

            // insert data in database table
            cLinerDataTable.TableName = "C_Liner";
            var bulkCopySql = new BulkCopySql(ColloSysParam.WebParams.ConnectionString.ConnectionString);
            bulkCopySql.CopyDataIntoDbTable(cLinerDataTable);

        }

        private static FileScheduler CreateFileScheduler(FileInfo fileInfo)
        {

            // get file details
            var session = SessionManager.GetCurrentSession();
            var fileDetails = session.QueryOver<FileDetail>()
                                     .Where(x => x.AliasName == ColloSysEnums.FileAliasName.C_LINER_COLLAGE)
                                     .SingleOrDefault<FileDetail>();

            //schedule the file
            var fileschedule = new FileScheduler
            {
                FileDate = new DateTime(2013, 07, 31),
                FileDetail = fileDetails,
                FileDirectory = fileInfo.DirectoryName,
                FileName = fileInfo.Name,
                FileServer = "localhost",
                FileSize = (ulong)fileInfo.Length,
                ImmediateReason = "Actual Data Upload",
                IsImmediate = true,
                StartDateTime = DateTime.Now,
                UploadStatus = ColloSysEnums.UploadStatus.Done,
                TotalRows = 0,
                ValidRows = 0,
                StatusDescription = "scheduled",
                ScbSystems = ScbEnums.ScbSystems.CCMS,
                Category = ScbEnums.Category.Liner
            };

            // create file status
            var filestatus = new FileStatus
            {
                UploadStatus = ColloSysEnums.UploadStatus.Done,
                EntryDateTime = DateTime.Now,
                TotalRows = 0,
                ValidRows = 0,
                UploadedRows = 0,
                FileScheduler = fileschedule
            };

            // add cross reference
            fileschedule.FileStatuss.Add(filestatus);

            // save file scheduler
            session.SaveOrUpdate(fileschedule);
            
            return fileschedule;
        }

    }
}
