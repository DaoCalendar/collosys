#region references

using System.Collections.Generic;
using System.Web.Http;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.UserInterface.Shared;
using ColloSys.UserInterface.Shared.Attributes;
using UserInterfaceAngular.NgGrid;

#endregion

namespace ColloSys.UserInterface.Areas.Billing.apiController
{
    public class FormulaApiController : BaseApiController<BFormula>
    {
        #region Get

        [HttpGet]
        [HttpTransaction]
        public IEnumerable<ScbEnums.Products> GetProducts()
        {
            var session = SessionManager.GetCurrentSession();
            return session.QueryOver<ProductConfig>().Select(x => x.Product).List<ScbEnums.Products>();
        }

        [HttpGet]
        [HttpTransaction]
        public IEnumerable<BFormula> GetFormulas(ScbEnums.Products product, ScbEnums.Category category)
        {
            return Session.QueryOver<BFormula>().Where(x => x.Products == product && x.Category == category).List();
        }

        [HttpGet]
        public IEnumerable<ColumnDef> GetColumnNames(ScbEnums.Products product, ScbEnums.Category category)
        {
            return SharedViewModel.ConditionColumns(product, category);
        }

        #endregion
    }
}
