namespace AuthService.Repository.Models
{
    public class RolesTable
    {
        public long IDRole { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public List<UserRolesTable> UserRoles { get; set; }
    }
}
