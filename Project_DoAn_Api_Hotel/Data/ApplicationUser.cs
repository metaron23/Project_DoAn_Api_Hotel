using Microsoft.AspNetCore.Identity;

namespace Project_DoAn_Api_Hotel.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; } = null!;
    }
}
