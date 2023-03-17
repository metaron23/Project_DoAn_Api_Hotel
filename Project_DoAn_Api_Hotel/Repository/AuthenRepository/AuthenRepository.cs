using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project_DoAn_Api_Hotel.Data;
using Project_DoAn_Api_Hotel.Model;
using Project_DoAn_Api_Hotel.Model.Authentication;
using Project_DoAn_Api_Hotel.Models;
using Project_DoAn_Api_Hotel.Repository.EmailRepository;
using Project_DoAn_Api_Hotel.Repository.TokenRepository;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Web;

namespace Project_DoAn_Api_Hotel.Repository.AuthenRepository
{
    public class AuthenRepository : ControllerBase, IAuthenRepository
    {
        private readonly MyDBContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ITokenRepository _tokenService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMailRepository _mailRepository;

        public AuthenRepository(MyDBContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ITokenRepository tokenService,
            SignInManager<ApplicationUser> signInManager,
            IMailRepository mailRepository
            )
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
            _mailRepository = mailRepository;
        }

        public async Task<LoginResponse> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email!);
            var check = await _signInManager.CheckPasswordSignInAsync(user, model.Password!, false);
            if (check.Succeeded)
            {

                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var token = _tokenService.GetAccessToken(authClaims);
                var refreshToken = _tokenService.GetRefreshToken();
                var tokenInfo = _context.TokenInfo.FirstOrDefault(a => a.Usename == user.UserName);

                if (tokenInfo == null)
                {
                    var info = new TokenInfo
                    {
                        Usename = user.UserName,
                        RefreshToken = refreshToken,
                        RefreshTokenExpiry = DateTime.Now.AddMinutes(5)
                    };
                    _context.TokenInfo.Add(info);
                }
                else
                {
                    tokenInfo.RefreshToken = refreshToken;
                    tokenInfo.RefreshTokenExpiry = DateTime.Now.AddMinutes(5);
                }
                try
                {
                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    new LoginResponse
                    {
                        StatusCode = 0,
                        Message = ex.Message,
                    };
                }

                return new LoginResponse
                {
                    Email = user.Email,
                    Username = user.UserName,
                    Token = token.TokenString,
                    RefreshToken = refreshToken,
                    Expiration = token.ValidTo,
                    StatusCode = 1,
                    Message = "Logged in"
                };
            }
            return
                new LoginResponse
                {
                    StatusCode = 0,
                    Message = "Invalid Username or Password",
                };
        }

        public async Task<Status> Registration([FromBody] RegistrationModel model)
        {
            var status = new Status();
            if (!ModelState.IsValid)
            {
                status.StatusCode = 0;
                status.Message = "Please pass all the required fields";
                return status;
            }
            // check if user exists
            var userExists = await _userManager.FindByNameAsync(model.UserName);
            if (userExists != null)
            {
                status.StatusCode = 0;
                status.Message = "Invalid username";
                return status;
            }
            var user = new ApplicationUser
            {
                UserName = model.UserName,
                SecurityStamp = Guid.NewGuid().ToString(),
                Email = model.Email,
                Name = model.Name
            };
            // create a user here
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                status.StatusCode = 0;
                status.Message = "User creation failed";
                return status;
            }

            if (!await _roleManager.RoleExistsAsync(UserRoles.User))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));

            if (await _roleManager.RoleExistsAsync(UserRoles.User))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.User);
            }
            status.StatusCode = 1;
            status.Message = "Sucessfully registered";

            string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            string codeHtmlVersion = HttpUtility.UrlEncode(code);

            string callbackUrl = "https://localhost:7075/api/Authorization/ConfirmEmailRegiste?email=" +
                user.Email + "&code=" + codeHtmlVersion;

            _mailRepository.Email(new EmailRequest
            {
                To = user.Email,
                Subject = "Mail confim registed",
                Body = "<a href=\"" + callbackUrl + "\">Link Confim</a>"
            });

            return status;
        }

        public async Task<bool> ConfirmEmailRegiste(string email, string code)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return false;
            var result = await _userManager.ConfirmEmailAsync(user, code);
            return result.Succeeded;
        }



        public async Task<Status> RegistrationAdmin([FromBody] RegistrationModel model)
        {
            var status = new Status();
            if (!ModelState.IsValid)
            {
                status.StatusCode = 0;
                status.Message = "Please pass all the required fields";
                return status;
            }
            // check if user exists
            var userExists = await _userManager.FindByNameAsync(model.UserName);
            if (userExists != null)
            {
                status.StatusCode = 0;
                status.Message = "Invalid username";
                return status;
            }
            var user = new ApplicationUser
            {
                UserName = model.UserName,
                SecurityStamp = Guid.NewGuid().ToString(),
                Email = model.Email,
                Name = model.Name
            };
            // create a user here
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                status.StatusCode = 0;
                status.Message = "User creation failed";
                return status;
            }

            if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));

            if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.Admin);
            }
            status.StatusCode = 1;
            status.Message = "Sucessfully admin registered";
            return status;
        }

        public async Task<Status> RequestResetPassword(string? email)
        {
            var status = new Status();

            var user = await _userManager.FindByEmailAsync(email);

            const string chars = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz0123456789";
            Random random = new Random();
            IEnumerable<string> string_Enumerable = Enumerable.Repeat(chars, 8);
            char[] arr = string_Enumerable.Select(s => s[random.Next(s.Length)]).ToArray();
            var password = "@T" + string.Join("", arr);

            await _userManager.RemovePasswordAsync(user);
            var check = await _userManager.AddPasswordAsync(user, password);

            if (check.Succeeded)
            {
                _mailRepository.Email(new EmailRequest
                {
                    To = user.Email,
                    Subject = "Mail confim change pass",
                    Body = "New password: " + password
                });
                status.StatusCode = 1;
                status.Message = "Please check email to change successfully";
                return status;
            }
            status.StatusCode = 0;
            status.Message = "Error while change password";
            return status;
        }

        public async Task<ChangePasswordResponse> RequestChangePassword(string? email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
            {
                return new ChangePasswordResponse { StatusCode = 0, Message = "Email not found" };
            }
            string token = await _userManager.GeneratePasswordResetTokenAsync(user);
            _mailRepository.Email(new EmailRequest
            {
                To = user.Email,
                Subject = "Mail confim change pass",
                Body = "Change password link: <a href=\"https://localhost:7075/api/Authorization/ConfirmChangePassword?code" + token + "&email = " + email + "\">Click Confirm</a>"
            });
            return new ChangePasswordResponse { StatusCode = 1, Message = "Please check mail to change pass", Token = token };
        }

        public async Task<Status> ConfirmChangePassword(string? code, string? email, ChangePasswordModel changePasswordModel)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var check = await _userManager.ResetPasswordAsync(user, code, changePasswordModel.NewPassword);
            if (check.Succeeded)
            {
                return new Status { StatusCode = 1, Message = "Change pass successfull" };
            }
            return new Status { StatusCode = 0, Message = "Change pass failed" };
        }
    }
}
