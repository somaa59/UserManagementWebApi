using System.ComponentModel.DataAnnotations;

namespace UserManagementWebApi.DTO.Account
{
    public class LoginDto
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
