using System.ComponentModel.DataAnnotations;

namespace Project_DoAn_Api_Hotel.Model.Token
{
    public class TokenRequest
    {
        [Required]
        public string? AccessToken { get; set; }
        [Required]
        public string? RefreshToken { get; set; }
    }
}
