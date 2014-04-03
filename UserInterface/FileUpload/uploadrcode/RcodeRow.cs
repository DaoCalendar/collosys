#region references

using System;
using System.Globalization;

#endregion

namespace ColloSys.UserInterface.Areas.OtherUploads.Helper
{
    public class RcodeRow
    {
        public string AccountNo { get; private set; }
        // ReSharper disable MemberCanBePrivate.Global
        // ReSharper disable UnusedAutoPropertyAccessor.Global
        public string CustomerName { get; set; }
        // ReSharper restore UnusedAutoPropertyAccessor.Global
        // ReSharper restore MemberCanBePrivate.Global
        public string Rcode { get; private set; }
        public readonly bool IsRecordvalid;

        public RcodeRow(string accno, string cname, string rcode)
        {
            AccountNo = string.Empty;
            CustomerName = string.Empty;
            Rcode = string.Empty;
            IsRecordvalid = false;
            if (string.IsNullOrWhiteSpace(accno) || string.IsNullOrWhiteSpace(rcode))
            {
                return;
            }
            try
            {
                AccountNo = Convert.ToUInt64(accno.Trim()).ToString(CultureInfo.InvariantCulture);
                Rcode = rcode.Trim();
                CustomerName = string.IsNullOrWhiteSpace(cname) ? string.Empty : cname.Trim();
                if (AccountNo != string.Empty)
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