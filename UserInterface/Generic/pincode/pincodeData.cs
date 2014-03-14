using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Enumerations;


namespace ColloSys.UserInterface.Areas.Generic.ViewModels
{
    public class PincodeData
    {
        public virtual string Country { get; set; }

        public virtual string Cluster { get; set; }

        public virtual string State { get; set; }

        public virtual string Region { get; set; }

        public virtual string District { get; set; }

        public virtual string City { get; set; }

        public virtual string Area { get; set; }

        public virtual uint Pincode { get; set; }

        public virtual ColloSysEnums.CityCategory CityCategory { get; set; }

    }
}