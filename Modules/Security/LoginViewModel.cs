using System.ComponentModel.DataAnnotations;

namespace IO.Modules.Security
{
    public class LoginViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Proszę podać email")]
        public string? Email { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Proszę podać hasło")]
        public string? Password { get; set; }
    }
}
