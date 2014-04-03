using System;
using System.Text;

namespace EMailServices
{
    public static class ResetPasswordEmail
    {
        public static bool Send(string emailTo, string password)
        {
            try
            {
                var emailer = new Emailer(emailTo);
                emailer.SetSubject("Reset Password");
                emailer.SetBody(GetBodyContents(password).ToString());
                emailer.Send();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static StringBuilder GetBodyContents(string password)
        {
            var body = new StringBuilder();
            body.Append("Hi,");
            body.AppendLine();
            body.AppendLine();
            body.Append("You recently requested to reset your ColloSys password.");
            body.AppendLine();
            body.Append("Your new password is : " + password);
            body.AppendLine();
            body.AppendLine();
            body.AppendLine("Thanks,");
            body.AppendLine("ColloSys Team");
            body.AppendLine();
            return body;
        }
    }
}
