using Microsoft.EntityFrameworkCore;
using Movie.Data;
using Movie.Models;
using Movie.Services.Interfaces;

namespace Movie.Services.Implementations
{
    public class GenreService : IGenreService
    {
        private readonly AppDbContext appDbContext;

        public GenreService(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
            List<Genre> genres = new List<Genre>
            {

                new Genre { Id=10,Name="Action" },
                new Genre { Id=20,Name="Action" },
                new Genre { Id=30,Name="Romance"}

            };


            appDbContext.Genre.AddRange(genres);
            appDbContext.SaveChangesAsync(); //please don't forget it

            this.appDbContext = appDbContext;
        }
        public async Task<Genre> AddAsync(Genre genre)
        {
            await appDbContext.Genre.AddAsync(genre);
            await appDbContext.SaveChangesAsync(); // Make sure to save the changes           
            return genre;
        }

        public async Task<Genre> DeleteAsync(Genre genre)
        {
            appDbContext.Remove(genre);
            await appDbContext.SaveChangesAsync();
            return genre;

        }

        public async Task<Genre> EditAsync(Genre genre)
        {
            appDbContext.Genre.Update(genre);
            await appDbContext.SaveChangesAsync(); // Make sure to save the changes
            return genre;
        }

        public async Task<IEnumerable<Genre>> GetAllAsync()
        {
            var genres = await appDbContext.Genre.Include(g => g.Movies).ToListAsync();
            return genres;
        }

        public Task<Genre> GetByIdAsync(int id)
        {
            var genre = appDbContext.Genre.Include(g => g.Movies).FirstOrDefaultAsync(g => g.Id == id);
            return genre;
        }
    }
}
