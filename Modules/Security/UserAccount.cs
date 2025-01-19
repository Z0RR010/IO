using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IO.Modules.Security
{
    [Table("users")]
    public class UserAccount
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("email")]
        public string Email { get; set; }

        [Column("password")]
        [MaxLength(256)]
        public string? Password { get; set; }

        [Column("role")]
        [MaxLength(20)]
        public string? Role { get; set; }

        [Column("emailVerified")]
        public bool EmailVerified { get; set; }
    }
}
