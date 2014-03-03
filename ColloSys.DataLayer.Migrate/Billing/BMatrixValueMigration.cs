using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColloSys.DataLayer.Enumerations;
using FluentMigrator;

namespace ColloSys.DataLayer.Migrate.Billing
{
    [Migration(20140221152015)]
    public class BMatrixValueMigration : Migration
    {
        public override void Up()
        {
            Alter.Table("B_MATRIX_VALUES")
                 .AddColumn("RowOperator")
                 .AsAnsiString()
                 .WithDefaultValue(ColloSysEnums.Operators.None)
                 .NotNullable()

                 .AddColumn("ColumnOperator")
                 .AsAnsiString()
                 .WithDefaultValue(ColloSysEnums.Operators.None)
                 .NotNullable();
        }

        public override void Down()
        {
            
        }
    }
}
