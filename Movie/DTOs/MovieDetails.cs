namespace Movie.DTOs
{
    public class MovieDetails
    {
        public int Id { get; set; } = default!;
        public string Title { get; set; } = default!;
        public double Rate { get; set; } = default!;
        public string Storyline { get; set; } = default!;
        public int Year { get; set; } = default!;
        public string genre {  get; set; }
    }
}
