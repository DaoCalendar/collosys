#region references

using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.SharedDomain;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UserInterfaceAngular.NgGrid;

#endregion

namespace ColloSys.UserInterface.Areas.CustomerData.ViewModels
{
    public class CustomerDataViewModel
    {

        public static NgGridOptions GetNgGrid(ScbEnums.ScbSystems system, ScbEnums.Category category)
        {
            switch (system)
            {
                case ScbEnums.ScbSystems.EBBS:
                    switch (category)
                    {
                        case ScbEnums.Category.Payment:
                            return GetPaymentNgGrid<EPayment>();
                        case ScbEnums.Category.Liner:
                            return GetELinerNgGrid();
                        case ScbEnums.Category.WriteOff:
                            return GetEWriteoffNgGrid();
                        default:
                            throw new InvalidOperationException("Category is not valid");
                    }
                case ScbEnums.ScbSystems.RLS:
                    switch (category)
                    {
                        case ScbEnums.Category.Payment:
                            return GetPaymentNgGrid<RPayment>();
                        case ScbEnums.Category.Liner:
                            return GetRLinerNgGrid();
                        case ScbEnums.Category.WriteOff:
                            return GetRWriteoffNgGrid();
                        default:
                            throw new InvalidOperationException("Category is not valid");
                    }
                case ScbEnums.ScbSystems.CCMS:
                    switch (category)
                    {
                        case ScbEnums.Category.Payment:
                            return GetPaymentNgGrid<CPayment>();
                        case ScbEnums.Category.Liner:
                            return GetCLinerNgGrid();
                        case ScbEnums.Category.WriteOff:
                            return GetCWriteoffNgGrid();
                        default:
                            throw new InvalidOperationException("Category is not valid");
                    }
                default:
                    throw new InvalidOperationException("Scb System is not valid");
            }
        }

        #region Credit Card

        public static NgGridOptions GetCLinerNgGrid()
        {
            var session = SessionManager.GetCurrentSession();
            var liners = session.QueryOver<CLiner>().List();

            foreach (var liner in liners)
            {
                liner.FileScheduler = null;
            }

            var gridOptions = new NgGridOptions
            {
                data = liners.Select(JObject.FromObject).ToList()
            };


            var eLinerGridColumns = GetPropertyList(typeof(CLiner))
                                   .Select(prop => new ColumnDef()
                                   {
                                       field = prop.Name,
                                       displayName = prop.Name,
                                   });

            gridOptions.columnDefs.AddRange(eLinerGridColumns);

            return gridOptions;
        }

        public static NgGridOptions GetCWriteoffNgGrid()
        {
            var session = SessionManager.GetCurrentSession();
            var writeoffs = session.QueryOver<CWriteoff>().List();

            foreach (var writeoff in writeoffs)
            {
                writeoff.FileScheduler = null;
            }

            var gridOptions = new NgGridOptions
            {
                data = writeoffs.Select(JObject.FromObject).ToList()
            };


            var eWriteoffGridColumns = GetPropertyList(typeof(CWriteoff))
                                   .Select(prop => new ColumnDef()
                                   {
                                       field = prop.Name,
                                       displayName = prop.Name,
                                   });

            gridOptions.columnDefs.AddRange(eWriteoffGridColumns);

            return gridOptions;
        }

        #endregion

        #region EBBS

        public static NgGridOptions GetELinerNgGrid()
        {
            var session = SessionManager.GetCurrentSession();
            var liners = session.QueryOver<ELiner>().List();

            foreach (var liner in liners)
            {
                liner.FileScheduler = null;
            }

            var gridOptions = new NgGridOptions
            {
                data = liners.Select(JObject.FromObject).ToList()
            };

            var eLinerGridColumns = GetPropertyList(typeof(ELiner))
                                   .Select(prop => new ColumnDef()
                                   {
                                       field = prop.Name,
                                       displayName = prop.Name,
                                   });

            gridOptions.columnDefs.AddRange(eLinerGridColumns);

            return gridOptions;
        }

        public static NgGridOptions GetEWriteoffNgGrid()
        {
            var session = SessionManager.GetCurrentSession();
            var writeoffs = session.QueryOver<EWriteoff>().List();

            foreach (var writeoff in writeoffs)
            {
                writeoff.FileScheduler = null;
            }

            var gridOptions = new NgGridOptions
            {
                data = writeoffs.Select(JObject.FromObject).ToList()
            };

            var eWriteoffGridColumns = GetPropertyList(typeof(EWriteoff))
                                   .Select(prop => new ColumnDef()
                                   {
                                       field = prop.Name,
                                       displayName = prop.Name,
                                   });

            gridOptions.columnDefs.AddRange(eWriteoffGridColumns);

            return gridOptions;
        }

        #endregion

        #region RLS

        public static NgGridOptions GetRLinerNgGrid()
        {
            var session = SessionManager.GetCurrentSession();
            var liners = session.QueryOver<RLiner>().List();

            foreach (var liner in liners)
            {
                liner.FileScheduler = null;
            }

            var gridOptions = new NgGridOptions
            {
                data = liners.Select(JObject.FromObject).ToList()
            };

            var eLinerGridColumns = GetPropertyList(typeof(RLiner))
                                   .Select(prop => new ColumnDef()
                                   {
                                       field = prop.Name,
                                       displayName = prop.Name,
                                   });

            gridOptions.columnDefs.AddRange(eLinerGridColumns);

            return gridOptions;
        }

        public static NgGridOptions GetRWriteoffNgGrid()
        {
            var session = SessionManager.GetCurrentSession();
            var writeoffs = session.QueryOver<RWriteoff>().List();

            foreach (var writeoff in writeoffs)
            {
                writeoff.FileScheduler = null;
            }

            var gridOptions = new NgGridOptions
            {
                data = writeoffs.Select(JObject.FromObject).ToList()
            };

            var eWriteoffGridColumns = GetPropertyList(typeof(RWriteoff))
                                   .Select(prop => new ColumnDef()
                                   {
                                       field = prop.Name,
                                       displayName = prop.Name,
                                   });

            gridOptions.columnDefs.AddRange(eWriteoffGridColumns);

            return gridOptions;
        }

        #endregion

        #region All System Payment

        public static NgGridOptions GetPaymentNgGrid<T>() where T : SharedPayment, new()
        {
            var payments = SessionManager.GetRepository<T>().GetAll();

            foreach (var payment in payments)
            {
                payment.FileScheduler = null;
            }

            var gridOptions = new NgGridOptions
            {
                data = payments.Select(JObject.FromObject)
            };

            var gridColumns = GetPropertyList(typeof(T))
                                   .Select(prop => new ColumnDef()
                                   {
                                       field = prop.Name,
                                       displayName = prop.Name,
                                   });

            gridOptions.columnDefs.AddRange(gridColumns);

            return gridOptions;
        }

        #endregion

        private static IEnumerable<PropertyInfo> GetPropertyList(Type p)
        {
            var propertyList = p.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            return from property in propertyList
                   let type = property.PropertyType
                   where type.BaseType == null || type.BaseType != typeof(Entity)
                   where type.BaseType == null || type.BaseType.BaseType == null || type.BaseType.BaseType != typeof(Entity)
                   where !type.IsGenericType || type.GetGenericTypeDefinition() == typeof(Nullable<>)
                   where !typeof(Entity).GetProperties(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public).Select(c => c.Name).Contains(property.Name)
                   select property;
        }
    }
}