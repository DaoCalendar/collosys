#region references

using System;
using System.Configuration.Provider;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;
using System.Web.Security;
using ColloSys.Shared.ConfigSectionReader;
using ColloSys.UserInterface.AuthNAuth.Models;
using ColloSys.UserInterface.Shared.Attributes;
using NLog;

#endregion

namespace ColloSys.UserInterface.Controllers
{
    public class AccountController : BaseController
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        #region login

        private string ManageReturnUrl(string returnUrl)
        {
            if (!string.IsNullOrWhiteSpace(returnUrl) && returnUrl.EndsWith(".cshtml"))
            {
                returnUrl = string.Empty;
            }

            if (!string.IsNullOrWhiteSpace(returnUrl) && !returnUrl.Contains("LogOff"))
            {
                ViewBag.ReturnUrl = returnUrl;
            }

            return returnUrl;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            returnUrl = ManageReturnUrl(returnUrl);
            return HttpContext.User.Identity.IsAuthenticated ? RedirectToLocal(returnUrl) : View();
        }

        [HttpPost]
        [AllowAnonymous]
        [MvcTransaction]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            returnUrl = ManageReturnUrl(returnUrl);

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToLocal(returnUrl);
            }

            try
            {
                if (ModelState.IsValid)
                {
                    if (!ValidateLogin(model))
                    {
                        return View();
                    }

                    FormsAuthentication.SetAuthCookie(model.UserName, false);
                    _log.Info("User '" + model.UserName + "' has logged in.");
                    return RedirectToLocal(returnUrl);
                }
            }
            catch (ProviderException e)
            {
                ModelState.AddModelError("", "The user name or password provided is incorrect.");
                _log.Error(e.Message);
                throw;
            }

            return View(model);
        }

        #endregion

        #region logoff

        [HttpGet]
        public ActionResult LogOff()
        {
            try
            {
                _log.Info("User '" + HttpContext.User.Identity.Name + " has logged out!");
                FormsAuthentication.SignOut();
                Session.Abandon();
            }
            catch (Exception e)
            {
                _log.Error(e.Message);
                throw;
            }
            return RedirectToAction("Login", "Account");
        }

        #endregion

        #region change-password

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ForgotPassword(ManageMessageId? message)
        {
            if (message != null)
            {
                ViewBag.ResetStatusMessage =
                 message == ManageMessageId.SetPasswordSuccess ? "Your password has been reset and sent to your mail id."
                 : message == ManageMessageId.SetPasswordError ? "Your password could not be reset contact your administrator."
                 : message == ManageMessageId.Exception ? TempData["ResetExceptionMessage"].ToString()
                 : "";
            }

            return View("ForgotPassword", new ForgotPasswordModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [MvcTransaction]
        public ActionResult ResetPassword(ForgotPasswordModel model)
        {
            var loginmodel = new LoginModel();
            bool passwordReset = false;
            if (ModelState.IsValid)
            {
                try
                {
                    string newPassword = Membership.Provider.ResetPassword(model.UserName,
                                                                           model.JoiningDate.ToString("yyyyMMdd"));

                    passwordReset = !string.IsNullOrWhiteSpace(newPassword);

                    if (passwordReset)
                    {
                        loginmodel.UserName = model.UserName;
                        loginmodel.UserPassword = newPassword;
                        SendRecoveryMail(loginmodel);
                    }
                }
                catch (Exception e)
                {
                    TempData["ResetExceptionMessage"] = e.Message;
                    return RedirectToAction("ForgotPassword", new { Message = ManageMessageId.Exception });

                }
            }

            TempData["IsResetPassword"] = passwordReset;
            if (passwordReset)
            {
                return RedirectToAction("ForgotPassword", new { Message = ManageMessageId.SetPasswordSuccess });
            }
            return RedirectToAction("ForgotPassword", new { Message = ManageMessageId.SetPasswordError });
        }

        [HttpGet]
        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : "";
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                return View("Login");
            }

            ViewBag.HasLocalPassword = HttpContext.User.Identity.IsAuthenticated;
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [MvcTransaction]
        public ActionResult Manage(LocalPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                // ChangePassword will throw an exception rather than return false in certain failure scenarios.
                bool changePasswordSucceeded;
                try
                {
                    changePasswordSucceeded = Membership.Provider.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                }
                catch (Exception e)
                {
                    _log.Error(e.Message);
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                }
                ModelState.AddModelError("", "Current password entered is incorrect or new password is invalid");
            }
            else
            {
                _log.Info("Insufficient Information provided for password change by the user " + HttpContext.User.Identity.Name);

            }


            // If we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion

        #region helpers

        private bool ValidateLogin(LoginModel model)
        {
            try
            {
                var userInfo = Membership.GetUser(model.UserName);
                if (userInfo == null)
                {
                    _log.Info("User Name:-" + model.UserName + " not exist and trying to login ");
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }

                else
                {
                    if (!userInfo.IsApproved)
                    {
                        _log.Info("User Name:-" + model.UserName + "'s account has not yet been approved and user tryig to login");
                        ModelState.AddModelError("", "Your account has not yet been approved by the site's administrators. Please try again later...");
                    }
                    else if (userInfo.IsLockedOut)
                    {
                        _log.Info("User Name " + model.UserName + "'s account has been locked out(exceeded incorrect login attempts)");
                        ModelState.AddModelError("", "Your account has been locked out(exceeded incorrect login attempts). Please contact Site administrators");
                    }
                    else
                    {
                        if (!Membership.ValidateUser(model.UserName, model.UserPassword))
                        {
                            ModelState.AddModelError("", "Invalid Username or Password");
                            _log.Info("Login Failed for :User Name:-" + model.UserName);
                        }
                    }
                }
                return ModelState.IsValid;

            }
            catch (Exception e)
            {
                _log.Error(e.Message);
                throw;
            }

        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account", returnUrl);
            }

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            SetPasswordError,
            AccountLocked,
            Exception
        }

        private void SendRecoveryMail(LoginModel model)
        {
            try
            {
                var user = Membership.Provider.GetUser(model.UserName, false);

                if (user == null) return;
                var mail = new MailMessage();
                mail.To.Add(user.Email);
                mail.From = new MailAddress("collosys@sc.com");

                mail.Subject = "Your password has been reset for ColloSys";
                mail.IsBodyHtml = true;
                mail.Body =  "Dear " + model.UserName + ",<br/>" +
                   "<br/> As you requested, your password has now been reset.<br/>" +
                   "<br/> Your new password :<b><pre> " + model.UserPassword + "</pre></b> <br/> Regards,<br/>" +
                   "<br/>ColloSys Support Team<br/>";
                mail.Priority = MailPriority.High;

                //TODO : generic by web config to send mail diff mailid 
                var smtpServer = ColloSysParam.WebParams.SmtpClient;

                smtpServer.Send(mail);
                _log.Info("Password recovery mail send to " + model.UserName);
            }
            catch (SmtpException e)
            {
                _log.Error(e.Message);
            }
        }

        #endregion
    }
}


        //private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        //{
        //    // See http://go.microsoft.com/fwlink/?LinkID=177550 for
        //    // a full list of status codes.
        //    switch (createStatus)
        //    {
        //        case MembershipCreateStatus.DuplicateUserName:
        //            return "User name already exists. Please enter a different user name.";

        //        case MembershipCreateStatus.DuplicateEmail:
        //            return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

        //        case MembershipCreateStatus.InvalidPassword:
        //            return "The password provided is invalid. Please enter a valid password value.";

        //        case MembershipCreateStatus.InvalidEmail:
        //            return "The e-mail address provided is invalid. Please check the value and try again.";

        //        case MembershipCreateStatus.InvalidAnswer:
        //            return "The password retrieval answer provided is invalid. Please check the value and try again.";

        //        case MembershipCreateStatus.InvalidQuestion:
        //            return "The password retrieval question provided is invalid. Please check the value and try again.";

        //        case MembershipCreateStatus.InvalidUserName:
        //            return "The user name provided is invalid. Please check the value and try again.";

        //        case MembershipCreateStatus.ProviderError:
        //            return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

        //        case MembershipCreateStatus.UserRejected:
        //            return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

        //        default:
        //            return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
        //    }
        //}



//[HttpPost]
//public void CloseSession()
//{
//    FormsAuthentication.SignOut();
//    Session.Abandon();
//}


