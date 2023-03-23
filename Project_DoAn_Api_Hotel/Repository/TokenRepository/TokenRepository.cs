using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Project_DoAn_Api_Hotel.Data;
using Project_DoAn_Api_Hotel.Model;
using Project_DoAn_Api_Hotel.Model.Token;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Project_DoAn_Api_Hotel.Repository.TokenRepository
{
    public class TokenRepository : ITokenRepository
    {
        private readonly MyDBContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private IConfiguration _configuration;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMapper _mapper;

        public TokenRepository(MyDBContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration, SignInManager<ApplicationUser> signInManager, IMapper mapper
            )
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _signInManager = signInManager;
            _mapper = mapper;
        }
        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]!)),
                ValidateLifetime = false
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            //principal
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");
            return principal;
        }

        public string GetRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public AccessTokenResponse GetAccessToken(IEnumerable<Claim> claim)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]!));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddMinutes(1),
                claims: claim,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            string tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return new AccessTokenResponse { TokenString = tokenString, ValidTo = token.ValidTo };
        }

        public object RefreshToken(TokenRequest tokenRequest)
        {
            var tokenResponse = _mapper.Map<TokenResponse>(tokenRequest);

            string accessToken = tokenRequest.AccessToken;
            string refreshToken = tokenRequest.RefreshToken;

            var principal = GetPrincipalFromExpiredToken(accessToken);
            var username = principal.FindFirstValue("UserName");

            var user = _context.TokenInfo.SingleOrDefault(u => u.Usename == username);

            if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiry <= DateTime.Now)
            {
                return new Status
                {
                    StatusCode = 0,
                    Message = "refreshToken sai hoặc đã hết hạn"
                };
            }

            tokenResponse.AccessToken = GetAccessToken(principal.Claims).TokenString!;

            var newRefreshToken = GetRefreshToken();
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiry = DateTime.Now.AddMinutes(5);
            _context.SaveChanges();

            tokenResponse.RefreshToken = newRefreshToken;
            return tokenResponse;
        }

        public bool Revoke(TokenRequest tokenRequest)
        {
            string accessToken = tokenRequest.AccessToken;
            string refreshToken = tokenRequest.RefreshToken;

            var principal = GetPrincipalFromExpiredToken(accessToken);
            try
            {
                var username = principal.FindFirstValue("UserName");
                var user = _context.TokenInfo.SingleOrDefault(u => u.Usename == username);
                if (user is null)
                {
                    return false;
                }
                user.RefreshToken = null;
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
