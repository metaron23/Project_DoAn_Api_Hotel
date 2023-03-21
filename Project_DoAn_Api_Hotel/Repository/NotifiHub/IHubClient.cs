namespace Project_DoAn_Api_Hotel.Repository.NotifiHub
{
    public interface IHubClient
    {
        Task ReceiveMessage(string sender, string message);
    }
}
