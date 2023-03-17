using Project_DoAn_Api_Hotel.Model;

namespace Project_DoAn_Api_Hotel.Model.Authentication
{
    public class LoginResponse : Status
    {
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? Expiration { get; set; }
        public string? Email { get; set; }
        public string? Username { get; set; }
    }
}
