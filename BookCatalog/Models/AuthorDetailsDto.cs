namespace BookCatalog.Models
{
    public class AuthorDetailsDto
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public List<Book> Books { get; set; }
    }
}
