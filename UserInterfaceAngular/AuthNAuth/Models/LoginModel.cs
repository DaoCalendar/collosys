using System.ComponentModel.DataAnnotations;

namespace ColloSys.UserInterface.AuthNAuth.Models
{
    public class LoginModel
    {

        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string UserPassword { get; set; }

    }
}