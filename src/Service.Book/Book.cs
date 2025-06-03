namespace Service.Book
{
    public class Book
    {
        public int Id { get; set; }
        public string Author { get; set; }
        public string Title { get; set; }
        public bool isAvailable { get; set; }
        public string? UserId { get; set; }
    }

    public class CreateBook
    {
        public string Title { get; set; }
        public string Author { get; set; }
    }
}
