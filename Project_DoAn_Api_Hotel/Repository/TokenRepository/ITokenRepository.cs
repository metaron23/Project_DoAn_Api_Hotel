using Project_DoAn_Api_Hotel.Model.Token;
using System.Security.Claims;

namespace Project_DoAn_Api_Hotel.Repository.TokenRepository
{
    public interface ITokenRepository
    {
        AccessTokenResponse GetAccessToken(IEnumerable<Claim> claim);
        string GetRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        TokenResponse RefreshToken(TokenRequest tokenRequest);
        bool Revoke(TokenRequest tokenRequest);
    }
}
