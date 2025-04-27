namespace UserManagementWebApi.DTO.Account
{
    public class AuthModel
    {
        public StateDto State { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public List<string>?Roles { get; set; }
        public string Token { get; set; }
        public DateTime ExpiresOn { get; set; }

    }
}
