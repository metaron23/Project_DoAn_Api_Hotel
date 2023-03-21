namespace Project_DoAn_Api_Hotel.Repository.InfoSystemRepository
{
    public interface IInfoSystemRepository
    {
        void GetUsers();
        void GetRoles();
        Task<List<object>> GetRoleClaims();
        void GetUserClaims();
    }
}
