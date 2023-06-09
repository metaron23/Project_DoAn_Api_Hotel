﻿using Microsoft.AspNetCore.Mvc;
using Project_DoAn_Api_Hotel.Model;
using Project_DoAn_Api_Hotel.Model.Token;
using Project_DoAn_Api_Hotel.Repository.TokenRepository;

namespace Project_DoAn_Api_Hotel.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly ITokenRepository _service;

        public TokenController(ITokenRepository service)
        {
            _service = service;
        }

        [HttpPost]
        public IActionResult Refresh(TokenRequest tokenRequest)
        {
            var token = _service.RefreshToken(tokenRequest);
            if (token is Status)
            {
                return BadRequest(token);
            }
            return Ok(token);
        }

        [HttpPost]
        public IActionResult Revoke(TokenRequest tokenRequest)
        {
            bool check = _service.Revoke(tokenRequest);
            if (!check)
            {
                return BadRequest();
            }
            return Ok();
        }
    }
}
