namespace AuthService.Repository.Models
{
    public class UsersTable
    {
        public long IDUser { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string Avatar { get; set; }

        public DateTime Date { get; set; }

        public List<UserRolesTable> UserRoles { get; set; }
    }
}
