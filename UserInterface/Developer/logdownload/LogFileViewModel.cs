﻿using System.Collections.Generic;

namespace AngularUI.Developer.logdownload
{
    public class LogFileViewModel
    {
        public List<LogFileModel> LogFiles { get; set; }
        public string Path { get; set; }
        public List<LogFileModel> Directories { get; set; }
        public string Drive { get; set; }
    }
}