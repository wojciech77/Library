

namespace Library.Models
{
    public class BorrowDto
    {
        public int Id { get; set; }
        public User User { get; set; }
        public Guid UserId { get; set; }
        public string Status { get; set; }
        public List<Resource> Resources { get; set; } = new List<Resource> { };
        public DateTime ReturnDay { get; set; }
    }
}
