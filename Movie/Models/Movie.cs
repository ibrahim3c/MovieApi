using System.ComponentModel.DataAnnotations.Schema;

namespace Movie.Models
{
    public class MOvie
    {
        public int Id { get; set; } = default!;
        [MaxLength(250)]
        public string Title { get; set; } = default!;
        public int Year { get; set; } = default!;
        public double Rate { get; set; } = default!;
        [MaxLength(2500)]
        public string Storyline {  get; set; } = default!;

        public string? Poster { get; set; } = default!;

        [ForeignKey(nameof(Genre))]
        public int GenreID { get; set; } = default!;
        public Genre Genre { get; set; } = default!;

    }
}
