using System.Collections.Generic;
using ColloSys.UserInterface.Areas.Developer.Models;

namespace ColloSys.UserInterface.Areas.Developer.ViewModels
{
    public class LogFileViewModel
    {
        public List<LogFileModel> LogFiles { get; set; }
        public string Path { get; set; }
        public List<LogFileModel> Directories { get; set; }
        public string Drive { get; set; }
    }
}