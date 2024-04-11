using Library.Models;

namespace Library.ViewModels
{
    public class BorrowViewModel
    {
        public BorrowDto Borrow { get; set; }
        public List<Resource> Resources { get; set; }
    }
}