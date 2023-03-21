using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Project_DoAn_Api_Hotel.Repository.NotifiHub
{
    public class CustomEmailProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            return connection.User?.FindFirst(ClaimTypes.Email)?.Value;
        }
    }
}
