namespace Movie.Models
{
    public class Permission
    {
        public int Id { get; set; }=default!;
        public string PermissionName { get; set; } = default!;
        public ICollection<UserPermission> UserPermissions { get; set; }
    }

  
    public enum PermissionEnum
    {
        GetMovies=1,
        AddMovie,
        DeleteMovie,
        EditMovie
    }
}
