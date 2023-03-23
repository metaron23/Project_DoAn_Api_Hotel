using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using NuGet.Protocol;
using System.Security.Claims;

namespace Project_DoAn_Api_Hotel.Repository.NotifiHub
{

    [Authorize]
    public class ChatHub : Hub<IHubClient>
    {
        private readonly static ConnectionMapping<string> _connections =
            new ConnectionMapping<string>();

        public void SendChatMessage(string whoReceive, string message)
        {
            string name = Context.User.Identity.Name;
            Clients.Caller.ReceiveMessage(name, message);
            foreach (var connectionID in _connections.GetConnections(whoReceive))
            {
                Clients.Client(connectionID).ReceiveMessage(name, message);
            }
        }

        public void SendChatMessageAuto(string message)
        {
            string name = Context.User.Claims.SingleOrDefault(a => a.Type == "UserName").ToString();
            Clients.Caller.ReceiveMessage("admin", "Đã nhận tin nhắn");
        }

        public override Task OnConnectedAsync()
        {
            var name = Context.User.Claims.ToList();
            foreach(var claim in name)
            {
                if(claim.Type == "UserName")
                {
                    _connections.Add(claim.Value, Context.ConnectionId);
                }                
            }           

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            string name = Context.User.Identity.Name;

            _connections.Remove(name, Context.ConnectionId);

            return base.OnDisconnectedAsync(exception);
        }
    }
}

