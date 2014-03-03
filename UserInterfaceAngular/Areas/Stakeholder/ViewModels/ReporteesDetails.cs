using System.Collections.Generic;
using ColloSys.DataLayer.Domain;

namespace ColloSys.UserInterface.Areas.Stakeholder.ViewModels
{
    public class ReporteesDetails
    {
        public ReporteesDetails()
        {
            ReportsToList = new List<Stakeholders>();
        }

        public IList<Stakeholders> ReportsToList { get; set; }
        public bool HasServiceCharge { get; set; }

    }
}