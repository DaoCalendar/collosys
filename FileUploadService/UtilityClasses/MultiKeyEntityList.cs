#region references

using System;
using System.Collections.Generic;
using System.Linq;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Components;

#endregion

namespace ColloSys.FileUploadService.UtilityClasses
{
    public class MultiKeyEntityList<T> where T : Entity, IUniqueKey
    {
        #region ctor

        //private IList<DateTime> _datesList;

        private readonly Dictionary<string, Dictionary<DateTime, T>> _entityList;

        public MultiKeyEntityList()
        {
            //_datesList = new List<DateTime>();
            _entityList = new Dictionary<string, Dictionary<DateTime, T>>();
        }

        #endregion

        #region add entity

        public void AddEntity(T entity)
        {
            // if not present by account no
            if (!_entityList.ContainsKey(entity.AccountNo))
            {
                var accnonotexist = new Dictionary<DateTime, T> { { entity.FileDate.Date.Date, entity } };
                _entityList.Add(entity.AccountNo, accnonotexist);
                return;
            }

            // if present NOT by account but by DATE 
            var recordDates2 = _entityList[entity.AccountNo];
            if (recordDates2.ContainsKey(entity.FileDate.Date))
            {
                recordDates2.Remove(entity.FileDate.Date);
            }

            // if present NOT by account and NOT BY DATE 
            recordDates2.Add(entity.FileDate.Date, entity);
        }

        public void AddEntities(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                AddEntity(entity);
            }
        }

        #endregion

        #region check if exist

        public bool DoesEntityExist(T entity)
        {
            return _entityList.ContainsKey(entity.AccountNo) &&
            _entityList[entity.AccountNo].ContainsKey(entity.FileDate.Date);
        }

        public bool DoesAccountExist(T entity)
        {
            return _entityList.ContainsKey(entity.AccountNo);
        }

        #endregion

        #region get entity

        public IEnumerable<T> GetEntities(DateTime date)
        {
            IList<T> list = new List<T>();
            foreach (var entity in _entityList)
            {
                var dateslist = entity.Value;
                if (dateslist.ContainsKey(date.Date))
                    list.Add(dateslist[date.Date]);
            }
            return list;
        }

        public T GetEntity(string accountNo, DateTime date)
        {
            if (!_entityList.ContainsKey(accountNo))
            {
                return null;
            }
            var dateslist = _entityList[accountNo];
            return dateslist.ContainsKey(date.Date) ? dateslist[date.Date] : null;
        }

        public T GetEntity(string accountNo)
        {
            if (!_entityList.ContainsKey(accountNo))
            {
                return null;
            }
            var dateslist = _entityList[accountNo];
            return dateslist.First().Value;
        }

        #endregion
    }
}
