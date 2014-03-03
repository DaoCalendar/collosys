using System;
using ColloSys.DataLayer.Enumerations;

namespace ColloSys.UserInterface.Areas.Allocation.ViewModels
{
    public class DataModel2
    {
        public ScbEnums.Products Product { get; set; }
        public DateTime AllocDate { get; set; }
        public ScbEnums.Category Category { get; set; }
        public ColloSysEnums.AllocStatus SelecAllocType { get; set; }
    }
}