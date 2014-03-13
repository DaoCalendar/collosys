#region references

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.QueryBuilder.FileUploadBuilder;
using Newtonsoft.Json.Linq;
using UserInterfaceAngular.NgGrid;

#endregion


namespace ColloSys.UserInterface.Areas.FileUploader.Models
{
    public class FileMappingViewModel
    {
        private static readonly FileMappingBuilder FileMappingBuilder=new FileMappingBuilder();
        private static readonly FileDetailBuilder FileDetailBuilder=new FileDetailBuilder();

        public static NgGridOptions GetNgGridOptions()
        {
            var gridOptions = new NgGridOptions
                {
                    data =FileMappingBuilder.GetAll().Select(JObject.FromObject)
                };

            var gridColumns = GetPropertyList(typeof(FileMapping))
                .Select(prop => new ColumnDef { field = prop.Name, displayName = prop.Name, });

            gridOptions.columnDefs.AddRange(gridColumns);

            return gridOptions;

        }

        public static IEnumerable<FileMapping> GetFileMappings(string actualTableName, string tempTableName)
        {
            var classTypes = typeof(CLiner).Assembly.GetTypes().Where(x => x.Name == actualTableName);

            var fileMappings =FileMappingBuilder.GetOnExpression(m => m.TempTable == tempTableName).ToList();

            foreach (var type in classTypes)
            {
                var propertyList = GetPropertyList(type);

                foreach (var propertyInfo in propertyList)
                {
                    var fileMapping = new FileMapping
                        {
                            ActualTable = type.Name,
                            ActualColumn = propertyInfo.Name,
                            TempTable = tempTableName,
                            StartDate = DateTime.Now,
                            EndDate = DateTime.Now.AddYears(100),
                            ValueType = ColloSysEnums.FileMappingValueType.DefaultValue
                        };

                    var matchFileMappings = fileMappings.Where(
                         c => c.ActualTable == fileMapping.ActualTable && c.ActualColumn == fileMapping.ActualColumn);

                    if (matchFileMappings.Any())
                    {
                        continue;
                    }


                    fileMappings.Add(fileMapping);
                }
            }

            return fileMappings.OrderBy(x => x.OutputPosition);
        }

        public static IEnumerable<FileMapping> GetFileMappings(Guid fileDetailId)
        {
            var fileDetail = FileDetailBuilder.GetOnExpression(c => c.Id == fileDetailId).FirstOrDefault();

            var classTypes = UploadableClass(fileDetail.AliasName);

            var fileMappings = FileMappingBuilder.GetOnExpression(m => m.TempTable == fileDetail.TempTable).ToList();

            //var fileMappings =
            //  session.Query<FileMapping>()
            //         .Where(m => m.TempTable == fileDetail.TempTable)
            //         .OrderBy(x => x.OutputPosition)
            //         .ToList();

            foreach (var type in classTypes)
            {
                var propertyList = GetPropertyList(type);

                foreach (var propertyInfo in propertyList)
                {
                    var fileMapping = new FileMapping
                    {
                        ActualTable = type.Name,
                        ActualColumn = propertyInfo.Name,
                        TempTable = fileDetail.TempTable,
                        StartDate = DateTime.Now,
                        EndDate = DateTime.Now.AddYears(100),
                        ValueType = ColloSysEnums.FileMappingValueType.DefaultValue
                    };

                    var matchFileMappings = fileMappings.Where(
                         c => c.ActualTable == fileMapping.ActualTable && c.ActualColumn == fileMapping.ActualColumn);

                    if (matchFileMappings.Any())
                    {
                        continue;
                    }

                    PropertyInfo info = propertyInfo;
                    var matchTempColumns = fileDetail.FileColumns.Where(c => c.TempColumnName.Substring(0, 3) == info.Name.Substring(0, 3));

                    var tempColumns = matchTempColumns as FileColumn[] ?? matchTempColumns.ToArray();
                    if (tempColumns.Any())
                    {
                        fileMapping.TempColumn = tempColumns.First().TempColumnName;
                        fileMapping.Position = tempColumns.First().Position;
                        fileMapping.ValueType = ColloSysEnums.FileMappingValueType.ExcelValue;
                    }

                    fileMappings.Add(fileMapping);
                }
            }

            return fileMappings.OrderBy(x => x.OutputPosition);
        }

        private static IEnumerable<PropertyInfo> GetPropertyList(Type p)
        {
            var propertyList = p.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            //var test1 =
            //    typeof(Entity).GetProperties(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public).ToList();

            return from property in propertyList
                   let type = property.PropertyType
                   where type.BaseType == null || type.BaseType != typeof(Entity)
                   where type.BaseType == null || type.BaseType.BaseType == null || type.BaseType.BaseType != typeof(Entity)
                   where !type.IsGenericType || type.GetGenericTypeDefinition() == typeof(Nullable<>)
                   where !typeof(Entity).GetProperties(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public).Select(c => c.Name).Contains(property.Name)
                   select property;

            //var test2 = test.ToList();
            //return test;
        }

        private static IEnumerable<Type> UploadableClass(ColloSysEnums.FileAliasName aliasName)
        {
            var types = Assembly.GetAssembly(typeof(CacsActivity)).GetTypes();

            switch (aliasName)
            {
                case ColloSysEnums.FileAliasName.E_LINER_AUTO:
                case ColloSysEnums.FileAliasName.E_LINER_OD_SME:
                    return types.Where(t => t == typeof(ELiner));

                case ColloSysEnums.FileAliasName.E_WRITEOFF_AUTO:
                case ColloSysEnums.FileAliasName.E_WRITEOFF_SMC:
                    return types.Where(t => t == typeof(EWriteoff));

                case ColloSysEnums.FileAliasName.E_PAYMENT_LINER:
                case ColloSysEnums.FileAliasName.E_PAYMENT_WO_AUTO:
                case ColloSysEnums.FileAliasName.E_PAYMENT_WO_SMC:
                case ColloSysEnums.FileAliasName.R_PAYMENT_WO_AEB:
                case ColloSysEnums.FileAliasName.R_PAYMENT_WO_PLPC:
                case ColloSysEnums.FileAliasName.R_MANUAL_REVERSAL:
                case ColloSysEnums.FileAliasName.R_PAYMENT_LINER:
                case ColloSysEnums.FileAliasName.C_PAYMENT_LIT:
                case ColloSysEnums.FileAliasName.C_PAYMENT_UIT:
                case ColloSysEnums.FileAliasName.C_PAYMENT_VISA:
                    return types.Where(t => t == typeof(ColloSys.DataLayer.ClientData.Payment));

                case ColloSysEnums.FileAliasName.R_LINER_BFS_LOAN:
                case ColloSysEnums.FileAliasName.R_LINER_MORT_LOAN:
                case ColloSysEnums.FileAliasName.R_LINER_PL:
                    return types.Where(t => t == typeof(RLiner));

                case ColloSysEnums.FileAliasName.R_WRITEOFF_PL_AEB:
                case ColloSysEnums.FileAliasName.R_WRITEOFF_PL_SCB:
                case ColloSysEnums.FileAliasName.R_WRITEOFF_PL_LORDS:
                case ColloSysEnums.FileAliasName.R_WRITEOFF_PL_GB:
                case ColloSysEnums.FileAliasName.R_WRITEOFF_SME:
                case ColloSysEnums.FileAliasName.R_WRITEOFF_AUTO_AEB:
                case ColloSysEnums.FileAliasName.R_WRITEOFF_AUTO_GB:
                case ColloSysEnums.FileAliasName.R_WRITEOFF_AUTO_SCB:
                    return types.Where(t => t == typeof(RWriteoff));

                case ColloSysEnums.FileAliasName.CACS_ACTIVITY:
                case ColloSysEnums.FileAliasName.C_LINER_COLLAGE:
                case ColloSysEnums.FileAliasName.C_LINER_UNBILLED:
                
                case ColloSysEnums.FileAliasName.C_WRITEOFF:
                    return types.Where(t => t == typeof(string)); // for blank value return

                default:
                    throw new InvalidOperationException("Alias Name is not Valid");
            }
        }
    }
}