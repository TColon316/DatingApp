using System.Text;
using API.Data;
using API.DTOs;
using API.Helpers;
using API.Interfaces;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            // Add services to the container.
            services.AddControllers();

            // Setup DataContext
            services.AddDbContext<DataContext>(options =>
            {
                // Tell the app what database technology we're using (in this case Sqlite)
                options.UseSqlite(config.GetConnectionString("DefaultConnection"));
            });

            // Add CORS to the app
            services.AddCors();

            // Add Services by specifying service lifetimes
            services.AddScoped<IPhotoService, PhotoService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUserRepository, UserRepository>();

            // Add AutoMapper Profiles
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // Add Cloudinary Settings
            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));

            return services;
        }
    }
}