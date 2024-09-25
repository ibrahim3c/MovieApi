using Microsoft.EntityFrameworkCore;
using Movie.Data;

namespace Movie.Extensions
{
    public static class AppDbContextExtensions
    {
        public static IServiceCollection MyAddDbContext(this IServiceCollection services,IConfiguration configuration) {
            var ConnectionString = configuration.GetConnectionString("DefaultConnection" ?? throw new InvalidOperationException("No Connection String was found"));

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(ConnectionString);
            });

            return services;
        }
    }
}
