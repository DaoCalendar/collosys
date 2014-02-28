#region references

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Enumerations;
using ColloSys.Shared.SharedUtils;
using ColloSys.Shared.Types4Product;
using NHibernate.Criterion;

#endregion

namespace ColloSys.DataLayer.Services.Shared
{
    public class DownloadParams
    {
        public DownloadParams()
        {
            _pageNumber = 1;
            PageSize = 100;
        }
        // ReSharper disable UnusedAutoPropertyAccessor.Global
        // ReSharper disable MemberCanBePrivate.Global
        public string ShowDataBy { get; set; }
        public ScbEnums.Products? SelectedProduct { get; set; }
        public ScbEnums.ScbSystems? SelectedSystem { get; set; }
        public ScbEnums.Category SelectedCategory { get; set; }
        public DateTime SelectedDate { get; set; }
        private uint _pageNumber;
        public uint PageNumber { get { return _pageNumber; } set { _pageNumber = Math.Max(1, value); } }
        private uint _pageSize;
        public uint PageSize { get { return _pageSize; } set { _pageSize = Math.Max(1, value); } }
        // ReSharper restore MemberCanBePrivate.Global
        //public DateTime DownloadDate { get; set; }
        // ReSharper restore UnusedAutoPropertyAccessor.Global

        private bool IsObjectValid()
        {
            if (string.IsNullOrWhiteSpace(ShowDataBy))
            {
                return false;
            }

            if (ShowDataBy != "Product" && ShowDataBy != "System")
            {
                return false;
            }

            if (ShowDataBy == "Product" && SelectedProduct == null)
            {
                return false;
            }

            if (ShowDataBy == "System" && SelectedSystem == null)
            {
                return false;
            }

            return true;
        }

        public bool ShowingBySystem()
        {
            return ShowDataBy == "System";
        }

        public Type GetTypeForCriteria()
        {
            // ReSharper disable PossibleInvalidOperationException
            return ShowingBySystem()
                       ? ClassType.GetTypeBySystemCategory(SelectedSystem.Value,
                                                           SelectedCategory)
                       : ClassType.GetTypeByProductCategory(SelectedProduct.Value,
                                                            SelectedCategory);
            // ReSharper restore PossibleInvalidOperationException
        }

        public IList<string> GetListofExcludedColumns()
        {
            var type = GetTypeForCriteria();
            IList<string> exludeList = new List<string>();
            if (!type.GetInterfaces().Contains(typeof(IFileUploadable)))
            {
                return exludeList;
            }
            var entity = Activator.CreateInstance(type) as IFileUploadable;
            if (entity != null) exludeList = entity.GetExcludeInExcelProperties();
            return exludeList;
        }

        public DetachedCriteria GetCriteria()
        {
            if (!IsObjectValid())
            {
                throw new InvalidDataException("Provided parameters are not valid!!!");
            }

            var criteriaOnType = GetTypeForCriteria();

            var criteria = DetachedCriteria.For(criteriaOnType, criteriaOnType.Name);
            if (criteriaOnType.GetInterfaces().Contains(typeof(IFileUploadable)))
            {
                var filedatename = ReflectionUtil.GetMemberName<IFileUploadable>(x => x.FileDate);
                //var filerowno = ReflectionUtil.GetMemberName<IFileUploadable>(x => x.FileRowNo);
                criteria.Add(Restrictions.Eq(string.Format("{0}.{1}", criteriaOnType.Name, filedatename),
                                             SelectedDate));
                //criteria.AddOrder(new Order(filerowno, true));
            }
            if (!ShowingBySystem() && criteriaOnType.GetInterfaces().Contains(typeof(IDelinquentCustomer)))
            {
                var productname = ReflectionUtil.GetMemberName<IDelinquentCustomer>(x => x.Product);
                criteria.Add(Restrictions.Eq(string.Format("{0}.{1}", criteriaOnType.Name, productname),
                                             SelectedProduct));
            }

            return criteria;
        }
    }
}
