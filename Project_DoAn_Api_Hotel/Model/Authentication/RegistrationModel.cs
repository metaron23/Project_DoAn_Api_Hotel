using Microsoft.Build.Framework;

namespace Project_DoAn_Api_Hotel.Model.Authentication
{
    public class RegistrationModel
    {
        [Required]
        public string? UserName { get; set; }
        [Required]
        public string? Name { get; set; }

        [Required]
        public string? Email { get; set; }

        [Required]
        public string? Password { get; set; }
    }
}
