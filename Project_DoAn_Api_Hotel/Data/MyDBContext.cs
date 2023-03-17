using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Project_DoAn_Api_Hotel.Data
{
    public class MyDBContext : IdentityDbContext<ApplicationUser>
    {
        public MyDBContext(DbContextOptions<MyDBContext> options) : base(options)
        {
        }

        public DbSet<TokenInfo> TokenInfo { get; set; }
    }
}
