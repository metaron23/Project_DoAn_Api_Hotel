namespace Project_DoAn_Api_Hotel.Model.Token
{
    public class AccessTokenResponse
    {
        public string? TokenString { get; set; }
        public DateTime ValidTo { get; set; }
    }
}
