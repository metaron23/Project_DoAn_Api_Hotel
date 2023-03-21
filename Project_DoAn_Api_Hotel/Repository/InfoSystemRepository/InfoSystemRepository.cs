using Microsoft.AspNetCore.Identity;
using Project_DoAn_Api_Hotel.Data;

namespace Project_DoAn_Api_Hotel.Repository.InfoSystemRepository
{
    public class InfoSystemRepository : IInfoSystemRepository
    {
        private readonly MyDBContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public InfoSystemRepository(MyDBContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<ApplicationUser> signInManager
            )
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }
        public async Task<List<object>> GetRoleClaims()
        {
            //var roles = _roleManager.Roles.ToList();
            return null;
        }

        public void GetRoles()
        {
            throw new NotImplementedException();
        }

        public void GetUserClaims()
        {
            throw new NotImplementedException();
        }

        public void GetUsers()
        {
            throw new NotImplementedException();
        }
    }
}
