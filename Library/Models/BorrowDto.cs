namespace Library.Models
{
    public class BorrowDto
    {
        public int Id { get; set; }
        public Guid userId { get; set; }
        public List<Resource> Resources { get; set; }
    }
}
