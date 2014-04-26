using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BillingService.DBLayer;
using ColloSys.DataLayer.Billing;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;

namespace BillingService.ViewModel
{
    public class FormulaBuilder
    {
        internal static dynamic SolveFormula<T>(BillDetail billDetail, string formulaName, List<T> data,TraceLogs traceLogs)
        {
            var formula = BillingPolicyDbLayer.GetFormula(billDetail.Products, formulaName);

            if (formula == null)
                return 0;

            var conditions = formula.BConditions.OrderBy(x => x.Priority).ToList();

            if (formula.OutputType == ColloSysEnums.OutputType.Boolean)
            {
                return ExpressionBuilder.GetConditionExpression<T>(billDetail, conditions, data, traceLogs);
            }

            return ExpressionBuilder.GetOutputExpression<T>(billDetail, conditions, data, traceLogs);
        }
    }
}
