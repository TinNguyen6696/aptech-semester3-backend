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

            services.Configure<SmtpSettings>(configuration.GetSection("Smtp"));
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<SmtpSettings>>().Value);

            services.Configure<AppSettings>(configuration.GetSection("App"));
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<AppSettings>>().Value);

            services.AddScoped<JwtHelper>();

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IJwtDenylistRepository, JwtDenylistRepository>();
            services.AddScoped<IAchievementRepository, AchievementRepository>();
            services.AddScoped<IVideoRepository, VideoRepository>();
            services.AddScoped<IUserTokenRepository, UserTokenRepository>();
            services.AddScoped<ILikeRepository, LikeRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<IRatingRepository, RatingRepository>();
            services.AddScoped<IVideoViewRepository, VideoViewRepository>();
            services.AddScoped<IFollowRepository, FollowRepository>();
            services.AddScoped<ICommunityPostRepository, CommunityPostRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IOpportunityRepository, OpportunityRepository>();

            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IProvinceService, ProvinceService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IFileUploadService, FileUploadService>();
            services.AddScoped<IVideoService, VideoService>();
            services.AddScoped<ILikeService, LikeService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IRatingService, RatingService>();
            services.AddScoped<IVideoViewService, VideoViewService>();
            services.AddScoped<IFollowService, FollowService>();
            services.AddScoped<ICommunityService, CommunityService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<IOpportunityService, OpportunityService>();

            return services;
        }
    }
}
