namespace Movie.DTOs
{
    public class UserRegisterationDTO
    {
        [Required]
        public string UserName { get; set; } = default!;

        [Required]
        [DataType(DataType.Password)]
        public string Password {  get; set; } = default!;

        [DataType(DataType.Password)]
        [Required]
        [Compare(nameof(Password))]
        public string ConfirmPassword {  get; set; } = default!;
        public string Email { get; set; } = default!;

    }
}
