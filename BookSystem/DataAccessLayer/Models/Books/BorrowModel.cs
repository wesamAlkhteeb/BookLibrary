using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Models.Books
{
    public class BookOrderModel
    {
        [Required(ErrorMessage ="You must to add Books.")]
        public required List<int> BooksId { get; set; }
        
    }
}
