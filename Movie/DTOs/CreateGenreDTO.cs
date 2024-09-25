namespace Movie.DTOs
{
    public class CreateGenreDTO
    {
        [DataType(DataType.Password)]
        [MaxLength(200)]
        public string Name { get; set; } = default!;
    }
}
