using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColloSys.DataLayer.Enumerations;
using FluentMigrator;

namespace ColloSys.DataLayer.Migrate.Billing
{
    [Migration(20140224114040)]
    public class BMatrixMigrationForMatrixPerTypeMatrixPerTypeName : Migration
    {
        public override void Up()
        {
            Alter.Table("B_MATRIX")
                 .AddColumn("MatrixPerType")
                 .AsAnsiString()
                 .WithDefaultValue(ColloSysEnums.PayoutLRType.None)
                 .NotNullable()

                 .AddColumn("MatrixPerTypeName")
                 .AsAnsiString()
                 .WithDefaultValue("")
                 .NotNullable();

        }

        public override void Down()
        {

        }
    }
}
