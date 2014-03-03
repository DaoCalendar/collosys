using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColloSys.DataLayer.Enumerations;
using FluentMigrator;

namespace ColloSys.DataLayer.Migrate.Billing
{
    [Migration(20140221151515)]
    public class BMatrixMigration:Migration 
    {
        public override void Up()
        {
            Alter.Table("B_MATRIX")
                 .AddColumn("ColumnsOperator")
                 .AsAnsiString()
                 .WithDefaultValue(ColloSysEnums.Operators.None)
                 .NotNullable()

                 .AddColumn("RowsOperator")
                 .AsAnsiString()
                 .WithDefaultValue(ColloSysEnums.Operators.None)
                 .NotNullable();

        }

        public override void Down()
        {
           
        }
    }
}
