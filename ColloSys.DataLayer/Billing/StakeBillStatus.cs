using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;

namespace ColloSys.DataLayer.Billing
{
    public class StakeBillStatus : Entity
    {
        public Stakeholders Stakeholders { get; set; }
        public DateTime BillGenerationDate { get; set; }
        public DateTime DispatchDate { get; set; }
        public DateTime RecivedDate { get; set; }
        public DateTime ChequeReceiptDate { get; set; }

        public ColloSysEnums.BillStatusStake Status { get; set; }
        public string Remark { get; set; } 

    }
}
