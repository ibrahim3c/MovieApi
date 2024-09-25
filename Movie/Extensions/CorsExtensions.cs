namespace Movie.Extensions
{
    public static class CorsExtensions
    {
        public static IServiceCollection AddCorsExtensions(this IServiceCollection services) {

            services.AddCors();
            return services;
        
        }
    }
}
