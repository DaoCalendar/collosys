using System.Collections.Generic;

namespace ColloSys.UserInterface.Shared
{
    public class ErrorModel
    {
        public string Summary { get; set; }

        public string Priority { get; set; }

        public string ExceptionType { get; set; }

        public string ExceptionSummary { get; set; }

        public string ExceptionStackTrace { get; set; }

        public IEnumerable<string> ErrorPriority
        {
            get
            {
                return new List<string>() { "Normal", "High", "Low" };;
            }
        }

        public string Message { get; set; }
    }
}