using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator;

namespace ColloSys.DataLayer.Migrate.Stakeholder
{
    [Migration(201402101759)]
   public class StkhPaymentProduct : Migration
    {
        public override void Up()
        {
            //Alter.Table("STKH_PAYMENT")
            //    .AddColumn("Products")
            //    .AsAnsiString().WithDefaultValue("SME_BIL")
            //    .NotNullable();
        }

        public override void Down()
        {
            //Delete.Column(ReflectionUtil.GetMemberName<StkhHierarchy>(x => x.ReportingLevel))
            //    .FromTable("STKH_HIERARCHY");
        }
    }
}
