namespace Movie.DTOs
{
    public class EditGenreDTO
    {
        [DataType(DataType.Password)]
        [MaxLength(200)]
        public string Name { get; set; } = default!;
    }
}
