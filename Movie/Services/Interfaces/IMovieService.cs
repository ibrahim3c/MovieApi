using Movie.Models;

namespace Movie.Services.Interfaces
{
    public interface IMovieService
    {
        Task<IEnumerable<MOvie>> GetAllAsync();
        Task<MOvie> GetByIdAsync(int id);
        Task<bool> AddAsync(MOvie movie);
        Task<bool> EditAsync(MOvie movie);
        Task<bool> DeleteAsync(MOvie movie);
    }
}
