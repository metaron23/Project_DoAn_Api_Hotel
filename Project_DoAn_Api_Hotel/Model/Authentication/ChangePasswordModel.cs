using System.ComponentModel.DataAnnotations;

namespace Project_DoAn_Api_Hotel.Model.Authentication
{
    public class ChangePasswordModel
    {
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? NewPassword { get; set; }
        [Required]
        [Compare("NewPassword")]
        public string? ConfirmNewPassword { get; set; }
    }
}
