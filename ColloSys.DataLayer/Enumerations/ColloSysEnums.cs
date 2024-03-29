﻿#region

using System;

#endregion

namespace ColloSys.DataLayer.Enumerations
{
    public static class ColloSysEnums
    {
        [Serializable]
        public enum NotificationType
        {
            StakeholderChange,
            StakeholderWorkingChange,
            StakeholderPaymentChange
        }

        [Serializable]
        public enum NotificationStatus
        {
            Active,
            Escalated,
            Archived
        }

        [Serializable]
        public enum NotifyHierarchy
        {
            Creator,
            Reporting
        }

        [Serializable]
        public enum PolicyOn
        {
            Stakeholder,
            Hierarchy,
            Product
        }

        [Serializable]
        public enum PolicyType
        {
            Payout,
            Capping,
            PF
        }

        [Serializable]
        public enum RuleForHolding
        {
            Value,
            Percentage
        }
        [Serializable]
        public enum ApplyOn
        {
            Fixed,
            Variable,
            TotalPreAdhoc,
            Total
        }

        [Serializable]
        public enum TaxType
        {
            Flat,
            Slab
        }
        [Serializable]
        public enum TaxApplicableTo
        {
            Individual,
            Agency,
            Both
        }

        [Serializable]
        public enum TaxApplyOn
        {
            GrossAmount,
            Tax
        }

        [Serializable]
        public enum BillPaymentStatus
        {
            None,
            BillingDone,
            Dispatched,
            Received,
            Accepted,
            PaymentDone,
            Closed
        }

        [Serializable]
        public enum GridScreenName
        {
            NotSpecified,
            ClientDataDownload,
            Allocation,
            Payment,
        }

        [Serializable]
        public enum BasicValueTypes
        {
            Unknown,
            Number,
            NumberWithPrecision,
            Text,
            Date,
            DateTime,
            Bool
        }

        [Serializable]
        public enum FileUploadBy
        {
            NotSpecified,
            TextReader,
            FileHelper,
            OleDbProvider,
            ExcelReader,
            CsvHelper,
            NPOIXlsReader,
            EpPlusXlsxReader
        }

        [Serializable]
        public enum FileAliasName
        {
            CACS_ACTIVITY,
            C_LINER_COLLAGE,
            C_LINER_UNBILLED,
            C_PAYMENT_LIT,
            C_PAYMENT_UIT,
            C_PAYMENT_VISA,
            C_WRITEOFF,
            E_LINER_AUTO,
            E_LINER_OD_SME,
            E_PAYMENT_LINER,
            E_PAYMENT_WO_AUTO,
            E_PAYMENT_WO_SMC,
            E_WRITEOFF_AUTO,
            E_WRITEOFF_SMC,
            R_WRITEOFF_SME,
            R_LINER_BFS_LOAN,
            R_LINER_MORT_LOAN,
            R_LINER_PL,
            R_MANUAL_REVERSAL,
            R_PAYMENT_LINER,
            R_PAYMENT_WO_AEB,
            R_PAYMENT_WO_PLPC,
            R_WRITEOFF_PL_AEB,
            R_WRITEOFF_PL_SCB,
            R_WRITEOFF_PL_GB,
            R_WRITEOFF_PL_LORDS,
            R_WRITEOFF_AUTO_AEB,
            R_WRITEOFF_AUTO_GB,
            R_WRITEOFF_AUTO_SCB
        }

        [Serializable]
        public enum Operators
        {
            None,
            GreaterThan,
            GreaterThanEqualTo,
            LessThan,
            LessThanEqualTo,
            NotEqualTo,
            EqualTo,
            StartsWith,
            EndsWith,
            Contains,
            Like,
            Plus,
            Minus,
            Multiply,
            Divide,
            ModuloDivide,
            IsIn
        }

        [Serializable]
        public enum FileDataType
        {
            Amount,
            Bool,
            Decimal,
            Number,
            String,
            Date,
            Guid
        }

        [Serializable]
        public enum FileMappingValueType
        {
            ComputedValue,
            DefaultValue,
            ExcelValue,
            MappedValue
        }

        [Serializable]
        public enum FileFrequency
        {
            Daily,
            Monthly,
            Weekly
        }

        [Serializable]
        public enum FileType
        {
            csv,
            txt,
            xls,
            xlsx
        }

        [Serializable]
        public enum Activities
        {
            AddEdit,
            Delete,
            Approve,
            Retry,
            FileUploader,
            CreateFile,
            ScheduleFile,
            CustomerData,
            ModifyPayment,
            UploadCustInfo,
            ErrorCorrection,
            Stakeholder,
            Allocation,
            AllocationPolicy,
            AllocationSubpolicy,
            CheckAllocation,
            SpecifyAllocPercentage,
            Billing,
            BillingPolicy,
            BillingSubpolicy,
            Formula,
            Matrix,
            AdhocPayout,
            HoldingPolicy,
            ManageHolidng,
            ReadyForBilling,
            PayoutStatus,
            Config,
            Permission,
            Product,
            KeyValue,
            Pincode,
            Taxlist,
            Taxmaster,
            EsclationMatrix,
            Developer,
            Hierarchy,
            Schedule,
            Status,
            User,
            Profile,
            Logout,
            ChangePassword,
            GenerateDb,
            SystemExplorer,
            DbTables,
            ExecuteQuery,
            Legal,
            RequisitionPreparation,
            RequsitionIntiation,
            FollowUp,
            LegalCaseexecution,
            Root,
            Performance,
            PerformanceParam,
            EncryptDecrypt
        }

        [Serializable]
        public enum ApproveStatus
        {
            NotApplicable,
            Saved,
            Submitted,
            Changed,
            Rejected,
            Approved,
        }

        [Serializable]
        public enum BillStatus
        {
            Unbilled,
            Billed,
            Paid
        }

        [Serializable]
        public enum ErrorStatus
        {
            DataError,
            Edited,
            Rejected,
            Submitted,
            Ignore,
            Approved,
            RetrySuccess
        }

        [Serializable]
        public enum AllocationPolicy
        {
            Monthly,
            Cyclewise
        }

        [Serializable]
        public enum BillingPolicy
        {
            Cyclewise,
            Monthly,
            MonthlyConfirmed
        }

        [Serializable]
        public enum PaymentSource
        {
            Fixed,
            Variable,
            Adhoc
        }

        [Serializable]
        public enum UploadStatus
        {
            ActInserting,
            Done,
            DoneWithError,
            Error,
            UploadStarted,
            UploadRequest,
            RetryUpload,
            Waiting,
            PostProcessing
        }
        [Serializable]
        public enum ReportingLevel
        {
            OneLevelUp = 1,
            TwoLevelUp = 2,
            ThreeLevelUp = 3,
            FourLevelUp = 4,
            AllLevels = 0
        }

        [Serializable]
        public enum Permissions
        {
            NoAccess,
            View,
            Modify,
            Approve
        }

        [Serializable]
        public enum HtmlInputType
        {
            date,
            number,
            text,
            checkbox,
            dropdown
        }

        [Serializable]
        public enum Gender
        {
            Male,
            Female
        }

        [Serializable]
        public enum DataProcessStatus
        {
            UploadDone,
            AllocationDone,
            BillingDone
        }

        [Serializable]
        public enum AllocBillingStatus
        {
            DataUploaded,
            AllocPartial,
            AllocDone,
            BillingPartial,
            BillingDone
        }

        [Serializable]
        public enum UsedFor
        {
            None,
            Allocation,
            Billing
        }

        [Serializable]
        public enum ConditionRelations
        {
            AND,
            OR
        }

        [Serializable]
        public enum AllocationType
        {
            HandleByTelecaller,
            DoNotAllocate,
            AllocateAsPerPolicy,
            AllocateToStkholder
        }

        [Serializable]
        public enum DelqFlag
        {
            N,
            O,
            R,
            Z
        }

        [Serializable]
        public enum DelqAccountStatus
        {
            PEND,
            Norm,
            BFL1,
            BFL2,
            BFL3,
            BFL4,
            BFL5,
            BFL6,
            BFL7,
        }

        [Serializable]
        public enum NoAllocResons
        {
            None,
            MissingPincode,
            NoMathcingPolicy,
            NoStakeholder
        }

        [Serializable]
        public enum AllocStatus
        {
            None,
            AllocationError,
            AlreadyAllocated,
            DoNotAllocate,
            AsPerWorking,
            AllocateToStakeholder,
            AllocateToTelecalling
        }

        [Serializable]
        public enum ChangeAllocReason
        {
            StakeholderOnLeave,
            StakeholderLeftJob
        }

        [Serializable]
        public enum ListOfEnums
        {
            AllocStatus,
            NoAllocResons,
            DelqAccountStatus,
            DelqFlag,
            AllocationType,
            UsedFor,
            Products
        }

        [Serializable]
        public enum PerformanceParam
        {
            Param_1,
            Param_2,
            Param_3,
            Param_4,
            Param_5,
        }

        [Serializable]
        public enum ParameterType
        {
            RankBased,
            TargetBased
        }
        [Serializable]
        public enum TargetOn
        {
            Bucket,
            Default
        }

        #region Billing

        [Serializable]
        public enum BillingStatus
        {
            Pending,
            InProcess,
            Done
        }

        [Serializable]
        public enum ConditionType
        {
            Condition,
            Output,
            OutputIf,
            OutputElse
        }

        [Serializable]
        public enum PayoutSubpolicyType
        {
            Formula,
            Subpolicy
        }

        [Serializable]
        public enum PayoutLRType
        {
            None,
            Value,
            Column,
            Table,
            Formula,
            Matrix
        }

        [Serializable]
        public enum Lsqlfunction
        {
            None,
            Sum,
            Min,
            Max,
            Count,
            Average
        }

        [Serializable]
        public enum OutputType
        {
            Number,
            Boolean,
            IfElse
        }

        #endregion
    }
}
