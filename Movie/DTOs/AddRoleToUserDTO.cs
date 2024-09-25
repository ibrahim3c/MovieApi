namespace Movie.DTOs
{
    public class AddRoleToUserDTO
    {

        [Required]
        public string UserID { get; set; } = default!;
        [Required]
        public string RoleName { get; set; }=default!;
        // or u can put roleID =>as u with :)
    }
}
