using System;
using System.Text;

namespace EMailServices
{
    public class StakeholderApproveEmail
    {
        public static bool Send(string emailTo, string name, string userid)
        {
            try
            {
                var emailer = new Emailer(emailTo);
                emailer.SetSubject("ColloSys - Access Granted");
                emailer.SetBody(GetBodyContents(name, userid).ToString());
                emailer.Send();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static StringBuilder GetBodyContents(string userName, string userId)
        {
            var body = new StringBuilder();
            body.Append("Dear  " + userName + ",");
            body.AppendLine();
            body.AppendLine();
            body.Append("Your have been granted access to collosys.");
            body.AppendLine();
            body.Append("The access details are mention below:");
            body.AppendLine();
            body.AppendLine();
            body.Append("UserID : " + userId);
            body.AppendLine();
            body.Append("Password : collosys");
            body.AppendLine();
            body.AppendLine();
            body.AppendLine("Regards,");
            body.AppendLine("ColloSys Support Team");
            body.AppendLine();

            return body;
        }
    }
}
