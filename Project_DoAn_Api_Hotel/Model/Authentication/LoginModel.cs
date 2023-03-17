using System.ComponentModel.DataAnnotations;

namespace Project_DoAn_Api_Hotel.Model.Authentication
{
    public class LoginModel
    {
        [Required]
        public string? Email { get; set; }

        [Required]
        public string? Password { get; set; }
    }
}
