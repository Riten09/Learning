using API.Data;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Extension;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationService(this IServiceCollection services, IConfiguration config)
    {
        
        services.AddDbContext<DataContext>(opt =>
            {
                opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
            });
            services.AddCors();
            //Adding  our services lifetime 
            services.AddScoped<ITokenService, TokenService>();

         return services;
    }
}
