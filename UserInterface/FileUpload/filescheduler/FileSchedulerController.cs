
//        public void Upload(FileUploadViewModel scheduledFiles)
//        {
//            // init schedule info from db, just to be safe ;)
//            scheduledFiles = FileUploadHelper.InitFileInfo(scheduledFiles);
//            var duplicateName = GetDuplicateFileNames();
//            _log.Info("File Upload path is: " + ColloSysParam.WebParams.UploadPath);

//            foreach (string requestFile in Request.Files)
//            {
//                HttpPostedFileBase file;
//                if (requestFile.IndexOf("_", StringComparison.Ordinal) < 1)
//                {
//                    throw new InvalidProgramException("Invalid alias name. it must be prepended with index.");
//                }

//                var aliasName = requestFile.Substring(requestFile.IndexOf("_", StringComparison.Ordinal) + 1);
//                if (IsInvalidFile(scheduledFiles, requestFile, aliasName, duplicateName, out file))
//                    continue;

//                // schedule the file
//                var path = ColloSysParam.WebParams.UploadPath;
//                var directory = Path.IsPathRooted(path) ? path : Server.MapPath(path);
//                FileUploadHelper.ScheduleFile(requestFile, aliasName, directory, file, scheduledFiles);
//            }

//            //return Json(scheduledFiles.ScheduleInfo, "text/plain");
//        }

//        #region supporting methods


//        #endregion
//    }
//}
