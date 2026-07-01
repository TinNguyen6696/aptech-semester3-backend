using Microsoft.Extensions.Options;
using TalentShowcase.Api.Helpers;
using TalentShowcase.Api.Repositories.Implementations;
using TalentShowcase.Api.Repositories.Interfaces;
using TalentShowcase.Api.Services.Implementations;
using TalentShowcase.Api.Services.Interfaces;

namespace TalentShowcase.Api.Extensions
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<JwtSettings>>().Value);

            services.AddScoped<JwtHelper>();

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IJwtDenylistRepository, JwtDenylistRepository>();
            services.AddScoped<IAchievementRepository, AchievementRepository>();

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IProvinceService, ProvinceService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IFileUploadService, FileUploadService>();

            return services;
        }
    }
}
