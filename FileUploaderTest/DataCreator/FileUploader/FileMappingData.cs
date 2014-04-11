#region References

using System;
using System.Collections.Generic;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Domain;

#endregion

namespace ReflectionExtension.Tests.DataCreator.FileUploader
{
    public class FileMappingData
    {

        private FileMapping GetDefaultMapping()
        {
            return new FileMapping
            {
                CreatedOn = DateTime.Now,
                CreatedBy = "testproject",
                CreateAction = "Insert",
                Version = 0,
                Id = default(Guid),
                EndDate = DateTime.Today.AddMonths(1),
                FileDetail = null,
                FileValueMappings = null,
                OutputColumnName = string.Empty,
                OutputPosition = 0,
                Position = 0,
                StartDate = DateTime.Today.AddMonths(-1),
                TempColumn = string.Empty,
                DefaultValue = string.Empty,
                ActualColumn = string.Empty,
                ValueType = default(ColloSysEnums.FileMappingValueType),
                ActualTable = string.Empty,
                TempTable = string.Empty
            };
        }

        public IEnumerable<FileMapping> ExcelMapper()
        {
            IList<FileMapping> mappings = new List<FileMapping>();

            var mapping1 = GetDefaultMapping();
            mapping1.ActualColumn = "AccountNo";
            mapping1.Position = 3;
            mappings.Add(mapping1);

            var mapping2 = GetDefaultMapping();
            mapping2.ActualColumn = "DebitAmount";
            mapping2.Position = 4;
            mappings.Add(mapping2);

            var mapping3 = GetDefaultMapping();
            mapping3.ActualColumn = "CreditAmount";
            mapping3.Position = 5;
            mappings.Add(mapping3);

            return mappings;
        }

        public IEnumerable<FileMapping> ExcelMapper_PassingTransCodeAndDesc()
        {
            IList<FileMapping> mappings = new List<FileMapping>();

            var mapping1 = GetDefaultMapping();
            mapping1.ActualColumn = "TransCode";
            mapping1.Position = 1;
            mappings.Add(mapping1);

            var mapping2 = GetDefaultMapping();
            mapping2.ActualColumn = "TransDesc";
            mapping2.Position = 2;
            mappings.Add(mapping2);

            return mappings;
        }

        public IEnumerable<FileMapping> ExcelMapper_PassingInvlidPosition()
        {
            IList<FileMapping> mappings = new List<FileMapping>();

            var mapping1 = GetDefaultMapping();
            mapping1.ActualColumn = "DebitAmount";
            mapping1.Position = 2;
            mappings.Add(mapping1);

            return mappings;
        }


        public IEnumerable<FileMapping> DefaultMapper()
        {
            IList<FileMapping> mappings = new List<FileMapping>();

            var mapping1 = GetDefaultMapping();
            mapping1.ActualColumn = "BillStatus";
            mapping1.DefaultValue = "Unbilled";
            mappings.Add(mapping1);

            var mapping2 = GetDefaultMapping();
            mapping2.ActualColumn = "IsExcluded";
            mapping2.DefaultValue = "True";
            mappings.Add(mapping2);

            return mappings;
        }

        public IList<FileMapping> CreateRecord()
        {
            IList<FileMapping> mappings = new List<FileMapping>();
            var mapping1 = GetDefaultMapping();
            mapping1.ActualColumn = "AccountNo";
            mapping1.Position = 3;
            mapping1.ValueType = ColloSysEnums.FileMappingValueType.ExcelValue;
            mappings.Add(mapping1);

            var mapping2 = GetDefaultMapping();
            mapping2.ActualColumn = "BillStatus";
            mapping2.ValueType = ColloSysEnums.FileMappingValueType.DefaultValue;
            mapping2.DefaultValue = "Unbilled";
            mappings.Add(mapping2);

            var mapping3 = GetDefaultMapping();
            mapping3.ActualColumn = "DebitAmount";
            mapping3.ValueType = ColloSysEnums.FileMappingValueType.ExcelValue;
            mapping3.Position = 4;
            mappings.Add(mapping3);

            var mapping4 = GetDefaultMapping();
            mapping4.ActualColumn = "CreditAmount";
            mapping4.ValueType = ColloSysEnums.FileMappingValueType.ExcelValue;
            mapping4.Position = 5;
            mappings.Add(mapping4);

            var mapping5 = GetDefaultMapping();
            mapping5.ActualColumn = "IsExcluded";
            mapping5.ValueType = ColloSysEnums.FileMappingValueType.ComputedValue;
            mappings.Add(mapping5);

            return mappings;
        }

        public IEnumerable<FileMapping> GetMappings()
        {
            IList<FileMapping> mappings = new List<FileMapping>();
            var mapping1 = GetDefaultMapping();
            mapping1.ActualColumn = "AccountNo";
            mapping1.ValueType = ColloSysEnums.FileMappingValueType.ExcelValue;
            mapping1.Position = 3;
            mappings.Add(mapping1);

            var mapping2 = GetDefaultMapping();
            mapping2.ActualColumn = "BillStatus";
            mapping2.ValueType = ColloSysEnums.FileMappingValueType.DefaultValue;
            mapping2.DefaultValue = "Billed";
            mappings.Add(mapping2);

            var mapping3 = GetDefaultMapping();
            mapping3.ActualColumn = "DebitAmount";
            mapping3.ValueType = ColloSysEnums.FileMappingValueType.ExcelValue;
            mapping3.Position = 4;
            mappings.Add(mapping3);

            var mapping4 = GetDefaultMapping();
            mapping4.ActualColumn = "CreditAmount";
            mapping4.ValueType = ColloSysEnums.FileMappingValueType.ExcelValue;
            mapping4.Position = 5;
            mappings.Add(mapping4);

            var mapping5 = GetDefaultMapping();
            mapping5.ActualColumn = "IsExcluded";
            mapping5.ValueType = ColloSysEnums.FileMappingValueType.ComputedValue;
            mappings.Add(mapping5);

            return mappings;
        }
    }
}
