using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillingService.ViewModel
{
    public class TraceLogs
    {
        private List<string> _logs;

        private List<string> _conditions;

        private IDictionary<string, decimal> _matrixs;

        private IDictionary<string, decimal> _formula;

        public string ConditionFor { get; set; }

        public TraceLogs()
        {
            _logs = new List<string>();
            _conditions = new List<string>();
            _matrixs = new Dictionary<string, decimal>();
            _formula = new Dictionary<string, decimal>();
        }

        #region Condition Matrix Formula

        public void AddCondition(string condition)
        {
            var withoutX = condition.Replace("x =>", "").Replace("x.", "");

            if (_conditions.Contains(withoutX))
                return;

            _conditions.Add(withoutX);
        }

        public void AddMatrixValue(string matrixName, decimal value)
        {
            _matrixs[matrixName] = value;
        }

        public void AddFormula(string formulaName, decimal value)
        {
            _formula[formulaName] = value;
        }

        public string GetConditionMatrixFormulaLog()
        {
            return string.Format("Condition : {0}," + "\n" + "Matrix :{1}," + "\n" + "Formula:{2}",
                string.Join("AndAlso", _conditions), string.Join(",", _matrixs), string.Join(",", _formula));
        }





        #endregion

        #region Logs

        public void SetLog(string log)
        {
            if (string.IsNullOrWhiteSpace(log))
                return;

            if (_logs.Contains(log))
                return;

            _logs.Add(log);
        }

        public string GetLog()
        {
            if (!_logs.Any())
                return string.Empty;

            return string.Join("\n", _logs);
        }

        #endregion
    }
}
