using Project_DoAn_Api_Hotel.Repository.AuthenRepository;
using Project_DoAn_Api_Hotel.Repository.EmailRepository;
using Project_DoAn_Api_Hotel.Repository.TokenRepository;

namespace Project_DoAn_Api_Hotel.Startup
{
    public static class AddRepositorySetup
    {
        public static IServiceCollection RepositoryService(this IServiceCollection services)
        {
            services.AddScoped<ITokenRepository, TokenRepository>();
            services.AddScoped<IAuthenRepository, AuthenRepository>();
            services.AddScoped<IMailRepository, MailRepository>();
            return services;
        }
    }
}
