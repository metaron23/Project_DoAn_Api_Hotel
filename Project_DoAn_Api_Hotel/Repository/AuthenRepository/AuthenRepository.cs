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

        public async Task<object> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email!);

            if (user != null)
            {
                if (user.LockoutEnd >= DateTime.Now)
                {
                    return
                    new Status
                    {
                        StatusCode = 0,
                        Message = "Tài khoản đã bị khoá vì nhập sai 3 lần! Thời gian mở khoá là: " + user.LockoutEnd.Value.AddHours(7),
                    };
                }
                var check = await _signInManager.CheckPasswordSignInAsync(user, model.Password!, true);

                if (check.Succeeded)
                {
                    var userRoles = await _userManager.GetRolesAsync(user);

                    var authClaims = new List<Claim>
                {
                    new Claim("UserName", user.UserName!),
                    new Claim("Email", user.Email!),
                    new Claim("Id", Guid.NewGuid().ToString()),
                };

                    foreach (var userRole in userRoles)
                    {
                        authClaims.Add(new Claim("Roles", userRole));
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
                        new Status
                        {
                            StatusCode = 0,
                            Message = ex.Message,
                        };
                    }

                    return new LoginResponse
                    {
                        AccessToken = token.TokenString,
                        RefreshToken = refreshToken,
                    };
                }
                else
                {
                    await _userManager.AccessFailedAsync(user);
                }
            }
            return new Status
            {
                StatusCode = 0,
                Message = "Sai email hoặc mật khẩu!",
            };
        }

        public async Task<Status> Registration([FromBody] RegistrationModel model)
        {
            var status = new Status();
            if (!ModelState.IsValid)
            {
                status.StatusCode = 0;
                status.Message = "Vui lòng điền đủ thông tin đăng ký";
                return status;
            }
            // check if user exists
            var userExistsEmail = await _userManager.FindByEmailAsync(model.Email);
            var userExistsUserName = await _userManager.FindByNameAsync(model.UserName);

            if (userExistsEmail != null)
            {
                status.StatusCode = 0;
                status.Message = "Email đã tồn tại trong hệ thống";
                return status;
            }
            if (userExistsUserName != null)
            {
                status.StatusCode = 0;
                status.Message = "UseName đã tồn tại trong hệ thống";
                return status;
            }

            var user = new ApplicationUser
            {
                UserName = model.UserName,
                SecurityStamp = Guid.NewGuid().ToString(),
                Email = model.Email,
                Name = model.Name,
                PhoneNumber = model.PhoneNumber
            };
            user.EmailConfirmed = true;
            // create a user here
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                status.StatusCode = 0;
                status.Message = "Dữ liệu đủ, nhưng tạo mới tài khoản User lỗi!";
                return status;
            }

            if (!await _roleManager.RoleExistsAsync(UserRoles.User))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));

            if (await _roleManager.RoleExistsAsync(UserRoles.User))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.User);
            }

            status.StatusCode = 1;
            status.Message = "Tạo mới thành công";


            //string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            //string codeHtmlVersion = HttpUtility.UrlEncode(code);

            //string callbackUrl = "https://localhost:7075/api/Authorization/ConfirmEmailRegiste?email=" +
            //    user.Email + "&code=" + codeHtmlVersion;

            //_mailRepository.Email(new EmailRequest
            //{
            //    To = user.Email,
            //    Subject = "Mail confim registed",
            //    Body = "<a href=\"" + callbackUrl + "\">Link Confim</a>"
            //});

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
                    Subject = "Mail reset mật khẩu tài khoản!",
                    Body = "Chào bạn, " + user.Name + "! Mật khẩu mới của bạn là: " + password + ". Vui lòng không đổi mật khẩu mới sau khi đăng nhập tài khoản!"
                });
                status.StatusCode = 1;
                status.Message = "Vui lòng kiểm tra hòm thử để nhận mật khẩu mới!";
                return status;
            }
            status.StatusCode = 0;
            status.Message = "Có lỗi trong quá trình reset mật khẩu mới! Vui lòng thử lại!";
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
