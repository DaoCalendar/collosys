using ColloSys.DataLayer.Generic;
using ColloSys.Shared.SharedUtils;
using FluentMigrator;

namespace ColloSys.DataLayer.Migrate.Stakeholder
{
    //[Migration(201401141322)]
    //public class ReportingChange : Migration
    //{
    //    public override void Up()
    //    {
    //        Alter.Table(typeof(GReports).Name)
    //            .AddColumn(ReflectionUtil.GetMemberName<GReports>(x => x.SendOnlyIfData))
    //            .AsBoolean()
    //            .NotNullable();
    //        Alter.Table(typeof(GReports).Name)
    //            .AddColumn(ReflectionUtil.GetMemberName<GReports>(x => x.Send2Hierarchy))
    //            .AsBoolean()
    //            .NotNullable();
    //        Alter.Table(typeof(GReports).Name)
    //            .AddColumn(ReflectionUtil.GetMemberName<GReports>(x => x.HierarchyId))
    //            .AsGuid()
    //            .NotNullable();
    //    }

    //    public override void Down()
    //    {
    //        Delete.Column(ReflectionUtil.GetMemberName<GReports>(x => x.SendOnlyIfData))
    //            .FromTable(typeof(GReports).Name);
    //        Delete.Column(ReflectionUtil.GetMemberName<GReports>(x => x.Send2Hierarchy))
    //            .FromTable(typeof(GReports).Name);
    //        Delete.Column(ReflectionUtil.GetMemberName<GReports>(x => x.HierarchyId))
    //            .FromTable(typeof(GReports).Name);
    //    }
    //}
}
