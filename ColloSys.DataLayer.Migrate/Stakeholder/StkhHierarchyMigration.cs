using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Generic;
using ColloSys.Shared.SharedUtils;
using FluentMigrator;

namespace ColloSys.DataLayer.Migrate.Stakeholder
{
    //[Migration(201401171322)]
    //public class StkhHierarchyMigration : Migration
    //{
    //    public override void Up()
    //    {
    //        Alter.Table("STKH_HIERARCHY")
    //            .AddColumn("ReportingLevel")
    //            .AsAnsiString().WithDefaultValue("OneLevelUp")
    //            .NotNullable();
        //}

    //    public override void Down()
    //    {
    //        //Delete.Column(ReflectionUtil.GetMemberName<StkhHierarchy>(x => x.ReportingLevel))
    //        //    .FromTable("STKH_HIERARCHY");
    //    }
    //}
}
