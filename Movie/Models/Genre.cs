using System.ComponentModel.DataAnnotations.Schema;

namespace Movie.Models
{
    public class Genre
    {
        [Key]
        // make it identifier means autoIncrement
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } = default!;

        [DataType(DataType.Password)]
        [MaxLength(200)]
        public required string Name { get; set; } = default!;
        public IEnumerable<MOvie>? Movies { get; set; }
    }
}
