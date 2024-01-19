namespace BookSystem.UI.Models
{
    public class BookSearchModel
    {
        public required int Id { get; set; }
        public required string Title { get; set; }
        public required string Author { get; set; }
        public required bool IsAvailable {  get; set; }
    }
}
