using ColloSys.DataLayer.BaseEntity;
using NHibernate;

namespace ColloSys.DataLayer.Generic
{
    public class GReportsMap : EntityMap<GReports>
    {
        public GReportsMap()
        {
            Property(x => x.ReportName);
            Property(x => x.User);
            Property(x => x.Description);
            Property(x => x.ScreenName);
            Property(x => x.ReportJson, map => map.Type(NHibernateUtil.StringClob));
            Property(x => x.DoEmailReport);
            Property(x => x.EmailId);
            Property(x => x.Frequency);
            Property(x => x.FrequencyParam);
            Property(x => x.NextSendingDateTime);
            Property(x => x.UseFieldName4Header);
            Property(x => x.SendOnlyIfData);
            Property(x => x.Send2Hierarchy);
            Property(x => x.StakeholderIds);
        }
    }
}