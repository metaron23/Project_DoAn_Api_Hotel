using Microsoft.AspNetCore.Mvc;
using Project_DoAn_Api_Hotel.Model;
using Project_DoAn_Api_Hotel.Model.Authentication;
using Project_DoAn_Api_Hotel.Models;

namespace Project_DoAn_Api_Hotel.Repository.AuthenRepository
{
    public interface IAuthenRepository
    {
        //Login
        Task<LoginResponse> Login([FromBody] LoginModel model);
        //Register and send mail confirm
        Task<Status> Registration([FromBody] RegistrationModel model);
        //Register with admin
        Task<Status> RegistrationAdmin([FromBody] RegistrationModel model);
        //Confirm successful registration by mail
        Task<bool> ConfirmEmailRegiste(string email, string code);
        // Confirm successful change pass
        Task<Status> RequestResetPassword(string? email);
        Task<ChangePasswordResponse> RequestChangePassword(string? email);
        Task<Status> ConfirmChangePassword(string? code, string? email, ChangePasswordModel changePasswordModel);
    }
}
