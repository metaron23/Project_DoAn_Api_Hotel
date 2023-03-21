using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_DoAn_Api_Hotel.Model;
using Project_DoAn_Api_Hotel.Model.Authentication;
using Project_DoAn_Api_Hotel.Repository.AuthenRepository;
using Project_DoAn_Api_Hotel.Repository.EmailRepository;

namespace Project_DoAn_Api_Hotel.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class AuthorizationController : ControllerBase
    {
        private readonly IAuthenRepository _authenRepository;
        private readonly IMailRepository _mailRepository;

        public AuthorizationController(IAuthenRepository authenRepository, IMailRepository mailRepository)
        {
            _authenRepository = authenRepository;
            _mailRepository = mailRepository;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var result = await _authenRepository.Login(model);
            if (result is LoginResponse)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Registration([FromBody] RegistrationModel model)
        {
            Status result = await _authenRepository.Registration(model);
            if (result.StatusCode == 1)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RegistrationAdmin([FromBody] RegistrationModel model)
        {
            Status result = await _authenRepository.RegistrationAdmin(model);
            if (result.StatusCode == 1)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet]
        public async Task<IActionResult> RequestResetPassword(string? email)
        {
            return Ok(await _authenRepository.RequestResetPassword(email));
        }

        [HttpPost]
        public IActionResult sendMail(EmailRequest emailRequest)
        {
            _mailRepository.Email(emailRequest);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmailRegiste(string email, string code)
        {
            return Ok(await _authenRepository.ConfirmEmailRegiste(email, code));
        }

        [HttpGet]
        public async Task<IActionResult> RequestChangePassword(string? email)
        {
            return Ok(await _authenRepository.RequestChangePassword(email));
        }
        [HttpPost]
        public async Task<IActionResult> ConfirmChangePassword(string? code, string? email, ChangePasswordModel changePasswordModel)
        {
            return Ok(await _authenRepository.ConfirmChangePassword(code, email, changePasswordModel));
        }
    }
}
