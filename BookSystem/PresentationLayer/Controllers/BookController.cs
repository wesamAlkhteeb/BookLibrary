using BusinessLayer.Helper.Security;
using BusinessLayer.Services;
using DataAccessLayer.Models.Books;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace PresentationLayer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookController:ControllerBase
    {
        private readonly IBookService bookService;

        public BookController(IBookService bookService)
        {
            this.bookService = bookService;
        }

        [Authorize]
        [HttpGet("borrowed/{page}")]
        public async Task<IActionResult> GetMyBorrowedBooks([FromHeader] string authorization, int page)
        {
            int id = JwtSecurity.securityData.getIdToken(authorization);
            return Ok(await bookService.GetMyBorrowedBooks(page,id));
        }

        [Authorize]
        [HttpPost("borrow")]
        public async Task<IActionResult> BorrowBooks([FromHeader]string authorization ,[FromBody] BookOrderModel bookOrder)
        {
            int id = JwtSecurity.securityData.getIdToken(authorization);
            return Ok(await bookService.BorrowBooks(bookOrder,id));
        }

        [Authorize]
        [HttpPost("return")]
        public async Task<IActionResult> ReturnBooks([FromHeader] string authorization, [FromBody] BookOrderModel bookOrder)
        {
            int id = JwtSecurity.securityData.getIdToken(authorization);
            return Ok(await bookService.ReturnBooks(bookOrder, id));
        }

        [Authorize]
        [HttpGet("search/author/{authorName}/{page}")]
        public async Task<IActionResult> SearchByAuthor(string authorName,int page)
        {
            return Ok(await bookService.SearchByAuthor(authorName,page));
        }

        [Authorize]
        [HttpGet("search/title/{title}")]
        public async Task<IActionResult> SearchByTitle(string title)
        {
            return Ok(await bookService.SearchByTitle(title));
        }

        [Authorize]
        [HttpGet("search/ISBN/{isbn}")]
        public async Task<IActionResult> SearchByISBN(string isbn)
        {
            return Ok(await bookService.SearchByISBN(isbn));
        }
    }
}
