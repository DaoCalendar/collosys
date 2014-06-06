using ColloSys.DataLayer.Stakeholder;

namespace AngularUI.Stakeholder.AddEdit2.BasicInfo
{
    public static class AddEditStakeholder
    {
        public static void SetStakeholderObj(Stakeholders data)
        {
            //add stakeholder reference in address
            foreach (var address in data.GAddress)
            {
                address.Stakeholder = data;
            }

            //add stakeholder reference in registration
            foreach (var registration in data.StkhRegistrations)
            {
                registration.Stakeholder = data;
            }
        }
    }
}