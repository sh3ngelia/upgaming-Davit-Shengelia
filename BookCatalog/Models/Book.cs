namespace BookCatalog.Models
{
    public class Book
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public int AuthorID { get; set; } // Foreign key
        public int PublicationYear { get; set; }
    }
}
