using AutoMapper;
using Project_DoAn_Api_Hotel.Model.Token;

namespace Project_DoAn_Api_Hotel.Helper
{
    public class ApplicationMapper : Profile
    {
        public ApplicationMapper()
        {
            CreateMap<TokenRequest, TokenResponse>().ReverseMap();
        }
    }
}
