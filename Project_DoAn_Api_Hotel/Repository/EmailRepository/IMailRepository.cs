

using Project_DoAn_Api_Hotel.Model;

namespace Project_DoAn_Api_Hotel.Repository.EmailRepository
{
    public interface IMailRepository
    {
        void Email(EmailRequest mailRequest);
    }
}
