using DataAccessLayer.Models.Books;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
    public interface IBookRepository
    {
        Task<Dictionary<string, object>> SearchByAuthor(string author, int page);
        Task<Dictionary<string, object>> SearchByTitle(string title);
        Task<Dictionary<string, object>> SearchByISBN(string isbn);
        Task<Dictionary<string, object>> GetMyBorrowedBooks(int page, int id);
        Task<string?> BorrowBooks(BookOrderModel bookOrder, int id);
        Task<Dictionary<string, object>> ReturnBooks(BookOrderModel bookOrder, int id);


    }
}
