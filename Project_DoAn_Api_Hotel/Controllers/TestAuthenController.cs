using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Project_DoAn_Api_Hotel.Repository.NotifiHub;

namespace Project_DoAn_Api_Hotel.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TestAuthenController : ControllerBase
    {
        private readonly IHubContext<ChatHub> _hubContext;

        public TestAuthenController(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpGet]
        public async Task<IActionResult> SendMessage(string message)
        {
            await _hubContext.Clients.All.SendAsync("MessageReceived", message);
            return Ok();
        }
    }
}
