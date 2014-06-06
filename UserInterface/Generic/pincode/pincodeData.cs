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

        public virtual string CityCategory { get; set; }

    }
}