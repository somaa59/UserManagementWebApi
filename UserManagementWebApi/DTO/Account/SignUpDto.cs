using System.ComponentModel.DataAnnotations;

namespace UserManagementWebApi.DTO.Account
{
    public class SignUpDto
    {
        public string? UserName { get; set; }
        [Required]
        [EmailAddress, StringLength(128)]
        public string Email { get; set; }
        [Required]
        [StringLength(256)]
        public string Password { get; set; }
    }
}
