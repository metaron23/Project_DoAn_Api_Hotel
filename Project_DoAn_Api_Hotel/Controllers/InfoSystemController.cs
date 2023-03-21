using Microsoft.AspNetCore.Mvc;
using Project_DoAn_Api_Hotel.Repository.InfoSystemRepository;

namespace Project_DoAn_Api_Hotel.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class InfoSystemController : ControllerBase
    {
        private readonly IInfoSystemRepository _infoSystemRepository;

        public InfoSystemController(IInfoSystemRepository infoSystemRepository)
        {
            _infoSystemRepository = infoSystemRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetRoleClaims()
        {
            var listRoleClaims = await _infoSystemRepository.GetRoleClaims();
            return Ok(listRoleClaims);
        }
    }
}
