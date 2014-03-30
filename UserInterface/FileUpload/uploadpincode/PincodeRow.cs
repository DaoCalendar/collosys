#region references

using System;
using System.Globalization;

#endregion

namespace ColloSys.UserInterface.Areas.OtherUploads.Helper
{
    public class PincodeRow
    {
        public string AccountNo { get; private set; }
        // ReSharper disable MemberCanBePrivate.Global
        // ReSharper disable UnusedAutoPropertyAccessor.Global
        public string CustomerName { get; set; }
        // ReSharper restore UnusedAutoPropertyAccessor.Global
        // ReSharper restore MemberCanBePrivate.Global
        public uint Pincode { get; private set; }
        public readonly bool IsRecordvalid;

        public PincodeRow(string accno, string cname, string pincode)
        {
            AccountNo = string.Empty;
            CustomerName = string.Empty;
            Pincode = 0;
            IsRecordvalid = false;
            if (string.IsNullOrWhiteSpace(accno) || string.IsNullOrWhiteSpace(pincode))
            {
                return;
            }
            try
            {
                AccountNo = Convert.ToUInt64(accno).ToString(string.Format("D{0}", accno.Length));
                Pincode = Convert.ToUInt32(pincode);
                CustomerName = string.IsNullOrWhiteSpace(cname) ? string.Empty : cname.Trim();
                if (Pincode != 0 && AccountNo != string.Empty && Pincode.ToString(CultureInfo.InvariantCulture).Length == 6)
                    IsRecordvalid = true;
            }
            catch (Exception)
            {
                // ReSharper disable RedundantJumpStatement
                return;
                // ReSharper restore RedundantJumpStatement
            }
        }
    }
}