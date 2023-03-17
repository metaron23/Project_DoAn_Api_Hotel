﻿namespace Project_DoAn_Api_Hotel.Model.Token
{
    public class TokenResponse
    {
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
        public DateTime Expired { get; set; }
        public bool Status { get; set; }
    }
}
