using System;
using ColloSys.DataLayer.Enumerations;

namespace ColloSys.UserInterface.Areas.Allocation.ViewModels
{
    public class DataModel
    {
        public ScbEnums.Products Product { get; set; }
        public DateTime AllocDate { get; set; }
        public ScbEnums.Category Category { get; set; }
        public Guid StaekholderId { get; set; } 
        //public DateTime StartDate { get; set; }
        //public DateTime EndDate { get; set; }
    }
}