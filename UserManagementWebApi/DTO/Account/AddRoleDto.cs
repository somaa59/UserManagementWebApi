using System.ComponentModel.DataAnnotations;

namespace UserManagementWebApi.DTO.Account
{
    public class AddRoleDto
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string Role { get; set; }
    }
}
