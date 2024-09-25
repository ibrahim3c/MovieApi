namespace Movie.Models
{
    public class UserPermission
    {

        public string UserId { get; set; } = default!;
        public int PermissionId { get; set; } = default!;
        public Permission Permission { get; set; }
        public AppUser AppUser { get; set; }
    }
}
