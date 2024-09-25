using Movie.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Movie.DTOs
{
    public class EditMovieDTO
    {
        public string Title { get; set; } = default!;
        public int Year { get; set; } = default!;
        public double Rate { get; set; } = default!;
        [MaxLength(2500)]
        public string Storyline { get; set; } = default!;

        public IFormFile Poster { get; set; } = default!;

        [ForeignKey(nameof(Genre))]
        public byte GenreID { get; set; } = default!;
    }
}
