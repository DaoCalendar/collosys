#region references

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using NHibernate.Criterion;

#endregion

namespace ColloSys.Shared.NgGrid
{
    public class GridQueryParams
    {
        #region properties

        // ReSharper disable MemberCanBePrivate.Global
        // ReSharper disable UnusedAutoPropertyAccessor.Global
        public byte[] Criteria { get; set; }
        public string CriteriaOnType { get; set; }
        public NgGridConfig GridConfig { get; set; }
        public IList<FilterParams> FiltersList { get; set; }
        // ReSharper restore UnusedAutoPropertyAccessor.Global
        // ReSharper restore MemberCanBePrivate.Global

        #endregion

        #region ctor

        public GridQueryParams()
        {
            GridConfig = new NgGridConfig();
            FiltersList = new List<FilterParams>();
        }

        #endregion

        #region helpers

        public DetachedCriteria GetCriteria()
        {
            var ms = new MemoryStream(Criteria);
            IFormatter formatter = new BinaryFormatter();
            return (DetachedCriteria)formatter.Deserialize(ms);
        }

        public Type GetCriteriaType()
        {
            return Type.GetType(CriteriaOnType);
        }

        #endregion

    }
}