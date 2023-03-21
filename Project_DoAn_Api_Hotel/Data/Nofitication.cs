using System.ComponentModel.DataAnnotations;

namespace Project_DoAn_Api_Hotel.Data
{
    public class Nofitication
    {
        [Key]
        public int Id { get; set; }
        public string? UserName { get; set; }
        public string? Title { get; set; }
        public string? Message { get; set; }
    }
}
