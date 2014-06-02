using System;
using System.Collections.Generic;
using System.Globalization;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.FileUploader.RowCounter;
using ReflectionExtension.ExcelReader;

namespace ColloSys.FileUploader.AliasRecordCreator
{
    abstract class AliasDHFLrecordCreator :IAliasRecordCreator<DHFL_Liner>
    {
        private readonly uint _accountPosition;
        private readonly uint _accountLength;
        public FileScheduler FileScheduler { get; private set; }

        protected AliasDHFLrecordCreator(FileScheduler scheduler, uint accountPosition, uint accountLength)
        {
            FileScheduler = scheduler;
            _accountLength = accountLength;
            _accountPosition = accountPosition;
        }

        public bool ComputedSetter(DHFL_Liner obj, IExcelReader reader, ICounter counter)
        {
            try
            {
                obj.FileDate = FileScheduler.FileDate.Date;
                obj.AgentId = reader.GetValue(33).Substring(0, 6);
                obj.BillMonth = Convert.ToUInt32(FileScheduler.FileDate.ToString("yyyyMM"));
                obj.DisbMonth = obj.BillMonth;
                obj.Loancode = uint.Parse(reader.GetValue(_accountPosition)).ToString("D" + _accountLength.ToString(CultureInfo.InvariantCulture));
                if (obj.Occupcategory.ToUpper() == "SALARIED" || obj.Occupcategory.ToUpper() == "SELF EMPLOYED PROFESSIONAL")
                    obj.IsProfessional = "Y";
                else
                    obj.IsProfessional = "N";
                GetComputations(obj, reader);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public abstract bool GetComputations(DHFL_Liner obj, IExcelReader reader);

        public bool ComputedSetter(DHFL_Liner obj, DHFL_Liner yobj, IExcelReader reader, IEnumerable<FileMapping> mapplings)
        {
            return true;
        }

        public bool CheckBasicField(IExcelReader reader, ICounter counter)
        {
            // loan no should be a number
            ulong loanNumber;
            if (!ulong.TryParse(reader.GetValue(_accountPosition), out loanNumber) )
            {
                counter.IncrementIgnoreRecord();
                return false;
            }

            // loan number must be of 2 digits min
            return (true);
        }

        public bool IsRecordValid(DHFL_Liner record, ICounter counter)
        {
            return true;
        }
    }
}
