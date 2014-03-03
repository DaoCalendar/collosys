using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BillingService.DBLayer;
using ColloSys.DataLayer.Billing;
using ColloSys.DataLayer.Enumerations;

namespace BillingService.ViewModel
{
    public class FormulaBuilder
    {
        internal static dynamic SolveFormula<T>(ScbEnums.Products products, string formulaName, List<T> data)
        {
            var formula = BillingPolicyDbLayer.GetFormula(products, formulaName);

            if (formula == null)
                return 0;

            var conditions = formula.BConditions.OrderBy(x => x.Priority).ToList();

            if (formula.OutputType == ColloSysEnums.OutputType.Boolean)
            {
                return ExpressionBuilder.GetConditionExpression<T>(products, conditions, data);
            }

            return ExpressionBuilder.GetOutputExpression<T>(products, conditions, data);
        }
    }
}
