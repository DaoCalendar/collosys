namespace AngularUI.Stakeholder.addedit.Working
{
    public class DisplayManager
    {
        public DisplayManager()
        {
            ShowCountry = true;
            ShowRegion = false;
            ShowState = false;
            ShowDistrict = false;
            ShowCity = false;
            ShowCluster = false;
            ShowArea = false;
        }
        public bool ShowCountry { get; set; }
        public bool ShowRegion { get; set; }
        public bool ShowState { get; set; }
        public bool ShowDistrict { get; set; }
        public bool ShowCluster { get; set; }
        public bool ShowCity { get; set; }
        public bool ShowArea { get; set; }
    }
}