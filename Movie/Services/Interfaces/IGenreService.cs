using Movie.Models;

namespace Movie.Services.Interfaces
{
    public interface IGenreService
    {
        Task<IEnumerable<Genre>> GetAllAsync();
        Task <Genre> GetByIdAsync(int id);
        Task<Genre>AddAsync(Genre genre);
        Task<Genre>EditAsync(Genre genre);
        Task<Genre> DeleteAsync( Genre genre);
    }
}
