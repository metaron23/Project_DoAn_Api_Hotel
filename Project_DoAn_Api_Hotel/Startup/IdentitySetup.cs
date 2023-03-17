using Microsoft.AspNetCore.Identity;
using Project_DoAn_Api_Hotel.Data;

namespace Project_DoAn_Api_Hotel.Startup
{
    public static class IdentitySetup
    {
        public static IServiceCollection IdentityService(this IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, IdentityRole>(option =>
            {
                option.SignIn.RequireConfirmedEmail = true;
                option.User.RequireUniqueEmail = true;
                option.Password.RequireDigit = false;
                option.Password.RequireUppercase = false;
                option.Password.RequireNonAlphanumeric = false;
                option.Password.RequiredUniqueChars = 0;
            })
                .AddEntityFrameworkStores<MyDBContext>()
                .AddDefaultTokenProviders();
            return services;
        }
    }
}
