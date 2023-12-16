namespace AuthService.Repository.Models
{
    public class UserRolesTable
    {
        public long IDUserRole { get; set; }

        public long IDUser { get; set; }

        public UsersTable User { get; set; }

        public long IDRole { get; set; }

        public RolesTable Role { get; set; }
    }
}
