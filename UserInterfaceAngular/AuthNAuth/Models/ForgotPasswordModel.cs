using System;
using System.ComponentModel.DataAnnotations;

namespace ColloSys.UserInterface.AuthNAuth.Models
{
    public class ForgotPasswordModel
    {
        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Joining Date")]
        public DateTime JoiningDate { get; set; }
    }
}