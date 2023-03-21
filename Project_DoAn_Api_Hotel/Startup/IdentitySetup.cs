using Microsoft.AspNetCore.Identity;
using Project_DoAn_Api_Hotel.Data;

namespace Project_DoAn_Api_Hotel.Startup
{
    public static class IdentitySetup
    {
        public static IServiceCollection IdentityService(this IServiceCollection services)
        {
            var lockoutOptions = new LockoutOptions()
            {
                AllowedForNewUsers = true,
                DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1),
                MaxFailedAccessAttempts = 3
            };
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedEmail = true;
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequireLowercase = false;
                options.Password.RequiredLength = 6;
                options.Lockout = lockoutOptions;
            })
                .AddEntityFrameworkStores<MyDBContext>()
                .AddDefaultTokenProviders();
            return services;
        }
    }
}
