

namespace Library.Models
{
    public class BorrowDto
    {
        public int Id { get; set; }
        public string Status { get; set; }
        
        public DateTime ReturnDay { get; set; }

        // Foreign key property
        public Guid UserId { get; set; }

        // Navigation property
        public User User { get; set; }


        public List<Resource> Resources { get; set; } = new List<Resource> { };
    }
}
