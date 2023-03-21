using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.SignalR;

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

        public override Task OnConnectedAsync()
        {
            string name = Context.User.Identity.Name;

            _connections.Add(name, Context.ConnectionId);

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

