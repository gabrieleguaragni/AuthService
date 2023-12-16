namespace AuthService.Shared.DTO.Response
{
    public class ValidateTokenResponse
    {
        public long IDUser { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string Avatar { get; set; }

        public List<string?> Roles { get; set; }
    }
}
