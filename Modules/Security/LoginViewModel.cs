using System.ComponentModel.DataAnnotations;

namespace IO.Modules.Security
{
    public class LoginViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please, provide the email")]
        public string? Email { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please, provide the password")]
        public string? Password { get; set; }
    }
}
