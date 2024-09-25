using Microsoft.AspNetCore.Authorization;
using Movie.Authorization.PolicyBased;
using Movie.Data;
using Movie.Mapping;
using Movie.Models;
using Movie.Services.Implementations;
using Movie.Services.Interfaces;
using System.Reflection;

namespace Movie.Extensions
{
    public static class InjectionExtensions
    {
        public static IServiceCollection AddInjectionExtensions(this IServiceCollection services)
        {
            services.AddScoped<IFileService,FileService>();
            services.AddScoped<IGenreService, GenreService>();
            services.AddScoped<IMovieService, MovieService>();
            //services.AddAutoMapper(typeof(MovieProfile).Assembly);
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            //identity
            services.AddIdentity<AppUser,AppRole>().AddEntityFrameworkStores<AppDbContext>();
            services.AddScoped<IAuthService,AuthService>();
            services.AddSingleton<IAuthorizationHandler, DegreeeAuthorizationHander>();
            return services;

        }
    }
}
