﻿namespace Library.Models
{
    public class Resource
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string? Description { get; set; }
        public int Quantity { get; set; }
        public string Type { get; set; }
        public string Category { get; set; }

        public List<BorrowDto> Borrows { get; set; } = new List<BorrowDto> { };


    }
}
