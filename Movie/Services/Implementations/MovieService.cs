using Microsoft.EntityFrameworkCore;
using Movie.Data;
using Movie.DTOs;
using Movie.Models;
using Movie.Services.Interfaces;

namespace Movie.Services.Implementations
{
    public class MovieService : IMovieService
    {
        private readonly AppDbContext appDbContext;

        public MovieService(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
            List<Genre> genres = new List<Genre>
    {
        new Genre { Id = 1, Name = "Action" },
        new Genre { Id = 2, Name = "Adventure" }, // Changed to ensure unique IDs
        new Genre { Id = 3, Name = "Romance" }
    };

            List<MOvie> mOvies = new List<MOvie>
    {
        new MOvie()
        {
            Id = 1,
            Title = "title1",
            Rate = 1,
            GenreID = 1,
            Storyline = "story1",
            Year = 2001
        },
        new MOvie()
        {
            Id = 2,
            Title = "title2",
            Rate = 2,
            GenreID = 2,
            Storyline = "story2",
            Year = 2002
        },
        new MOvie()
        {
            Id = 3,
            Title = "title3",
            Rate = 3,
            GenreID = 3,
            Storyline = "story3",
            Year = 2003
        }
    };

            appDbContext.Genre.AddRange(genres);
            appDbContext.movies.AddRange(mOvies);
            appDbContext.SaveChanges();
        }

        public async Task<bool> AddAsync(MOvie movie)
        {
            await appDbContext.AddAsync(movie);
           var affectedRows= await appDbContext.SaveChangesAsync();
            return affectedRows>0;
        }

        public async Task<bool> DeleteAsync(MOvie movie)
        {
            appDbContext.Remove(movie);
            var affectedRows = await appDbContext.SaveChangesAsync();
            return affectedRows>0;
        }

        public async Task<bool> EditAsync(MOvie movie)
        {
            appDbContext.Update(movie);
            var affectedRows = appDbContext.SaveChanges();
            return affectedRows>0;
        }

        public async Task<IEnumerable<MOvie>> GetAllAsync()
        {
            return await appDbContext.movies.Include(m=>m.Genre).ToListAsync();

        }

        public async Task<MOvie> GetByIdAsync(int id)
        {
            return await appDbContext.movies.Include(m => m.Genre).FirstOrDefaultAsync(m => m.Id == id);


        }
    }
}
