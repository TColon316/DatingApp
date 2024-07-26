using API.Data;
using API.Interface;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions;

public static class ApplicationServiceExtensions
{
  public static IServiceCollection AddApplicationServies(this IServiceCollection services, IConfiguration config)
  {
    services.AddControllers();

    // Add DbContext
    services.AddDbContext<DataContext>(options => {
      options.UseSqlite(config.GetConnectionString("DefaultConnection"));
    });

    // Adding CORS
    services.AddCors();
    services.AddScoped<ITokenService, TokenService>().Reverse();
    services.AddScoped<IUserRepository, UserRepository>().Reverse();

    // Adding AutoMapper
    services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

    return services;
  }
}
