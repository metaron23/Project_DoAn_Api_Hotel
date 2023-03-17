using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project_DoAn_Api_Hotel.Data;
using Project_DoAn_Api_Hotel.Model;
using Project_DoAn_Api_Hotel.Model.Authentication;
using Project_DoAn_Api_Hotel.Models;
using Project_DoAn_Api_Hotel.Repository.TokenRepository;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Project_DoAn_Api_Hotel.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class TestAuthenController : ControllerBase
    {
        private readonly MyDBContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public TestAuthenController(MyDBContext context,
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

        [HttpGet]
        public async Task<IActionResult> GetData()
        {
            var user = await _userManager.FindByNameAsync("admin");
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var userRole in userRoles)
            {
                await _userManager.AddClaimAsync(user,new Claim(ClaimTypes.Role, userRole));
            }
            var role = await _roleManager.FindByNameAsync("Admin");

            await _userManager.AddClaimsAsync(user, new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Email, user.Email),
            });

            await _roleManager.AddClaimAsync(role, new Claim("Permission", "Blog.Create"));

            return Ok(User.Claims);
        }
    }
}
