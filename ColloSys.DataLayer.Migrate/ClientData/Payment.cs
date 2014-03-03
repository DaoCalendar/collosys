using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColloSys.DataLayer.Domain;
using ColloSys.Shared.SharedUtils;
using FluentMigrator;

namespace ColloSys.DataLayer.Migrate.ClientData
{
    [Migration(201402171350)]
    public class Payment : Migration
    {
        public override void Up()
        {
            Alter.Table("BILL_DETAILS")
                 .AddColumn(ReflectionUtil.GetMemberName<BillDetail>(x => x.PaymentSource))
                 .AsString()
                 .WithDefaultValue("Fixed")
                 .NotNullable();
            //Alter.Table("C_Payment")
            //     .AddColumn(ReflectionUtil.GetMemberName<CPayment>(x => x.Products))
            //     .AsString()
            //     .WithDefaultValue("CC")
            //     .NotNullable();

            //Alter.Table("E_Payment")
            //    .AddColumn(ReflectionUtil.GetMemberName<EPayment>(x => x.Products))
            //    .AsString()
            // .NotNullable();

            //Alter.Table("R_Payment")
            //    .AddColumn(ReflectionUtil.GetMemberName<RPayment>(x => x.Products))
            //    .AsString()
            //  .NotNullable();
        }

        public override void Down()
        {
            Delete.Column(ReflectionUtil.GetMemberName<BillDetail>(x => x.PaymentSource))
                  .FromTable("BILL_DETAILS");

            //Delete.Column(ReflectionUtil.GetMemberName<CPayment>(x => x.Products))
            //   .FromTable("C_Payment");

            //Delete.Column(ReflectionUtil.GetMemberName<EPayment>(x => x.Products))
            //   .FromTable("E_Payment");

            //Delete.Column(ReflectionUtil.GetMemberName<RPayment>(x => x.Products))
            //   .FromTable("R_Payment");
        }

    }
}
