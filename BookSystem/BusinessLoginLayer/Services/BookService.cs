using DataAccessLayer.Models.Books;
using DataAccessLayer.Repository;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository bookRepository;
        public BookService(IBookRepository bookRepository)
        {
            this.bookRepository = bookRepository;
        }

        public async Task<Dictionary<string, object>> BorrowBooks(BookOrderModel bookOrder, int id)
        {
            string? result = await bookRepository.BorrowBooks(bookOrder, id);    
            if(result != null)
            {
                throw new BadHttpRequestException(result);
            }
            return new Dictionary<string, object>
            {
                {"message","All book is borrowed."}
            };
            
        }

        public async Task<Dictionary<string, object>> GetMyBorrowedBooks(int page, int id)
        {
            return await bookRepository.GetMyBorrowedBooks(page, id);
        }

        public Task<Dictionary<string, object>> ReturnBooks(BookOrderModel bookOrder, int id)
        {
            return bookRepository.ReturnBooks(bookOrder,id);
        }

        public async Task<Dictionary<string, object>> SearchByAuthor(string author,int page)
        {
            return await bookRepository.SearchByAuthor(author, page);
        }

        public async Task<Dictionary<string, object>> SearchByISBN(string isbn)
        {
            return await bookRepository.SearchByISBN(isbn);
        }

        public async Task<Dictionary<string, object>> SearchByTitle(string title)
        {
            return await bookRepository.SearchByTitle(title);
        }
    }
}
