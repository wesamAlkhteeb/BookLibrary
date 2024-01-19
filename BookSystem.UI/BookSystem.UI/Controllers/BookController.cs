using BookSystem.UI.Models;
using BookSystem.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookSystem.UI.Controllers
{
    public class BookController : Controller
    {
        public List<int> booksId = new();
        public List<int> booksIdReturn = new();
        private readonly BookService bookService;

        
        private List<BorrowedBooksModel> borrowedbooks;
        public BookController(BookService bookService)
        {
            this.bookService = bookService;
            borrowedbooks = new List<BorrowedBooksModel>();
        }

        private bool isLogin()
        {
            return Request.Cookies["token"] != null;
        }

        [HttpGet]
        public IActionResult Search()
        {
            if (!isLogin())
            {
                return RedirectToAction("login", "user");
 ;          }
            ViewBag.books = bookService.books;
            return View();
        }
        
        [HttpGet]
        public async Task<IActionResult> Borrowed()
        {
            if (!isLogin())
            {
                return RedirectToAction("login", "user");
                ;
            }
            try
            {
                borrowedbooks = await bookService.GetBorrowedBooks(Request.Cookies["token"]!, 1);
                ViewBag.borrowed = borrowedbooks;
            }
            catch (Exception e)
            {
                ViewBag.borrowed = borrowedbooks;
                ViewData["message"] = e.Message;
            }
            
            return View();
        }

        public async Task<IActionResult> ReturnIt(List<int> selecttions)
        {
            try
            {
                Dictionary<string,object> data = await bookService.ReturnIt(Request.Cookies["token"]!, selecttions);
                TempData["isCurrect"] = "custom-success";
                TempData["message"] = data["message"];
            }
            catch (Exception e)
            {
                TempData["isCurrect"] = "custom-alert";
                TempData["message"] = e.Message;
            }
            try
            {
                borrowedbooks = await bookService.GetBorrowedBooks(Request.Cookies["token"]!, 1);
                ViewBag.borrowed = borrowedbooks;
            }
            catch (Exception e)
            {
                ViewBag.borrowed = borrowedbooks;
            }
            return View("Borrowed");
        }



        public async Task<IActionResult> SearchButton(string data ,int index)
        {
            if (!isLogin())
            {
                return RedirectToAction("login", "user");
                ;
            }
            ViewData["load"] = true;
            string path;
            if (index == 1)
            {
                path = $"book/search/title/{data}";
                bookService.books = await bookService.SearchAndGetOne(Request.Cookies["token"]!.ToString(), path);
            }
            else if (index == 2)
            {
                path = $"book/search/author/{data}/1";
                bookService.books = await bookService.SearchAndGetList(Request.Cookies["token"]!.ToString(), path);
            }
            else
            {
                path = $"book/search/isbn/{data}";
                bookService.books = await bookService.SearchAndGetOne(Request.Cookies["token"]!.ToString(), path);
            }
            ViewBag.searchText = data;
            ViewBag.books = bookService.books;
            ViewData["load"] = null;
            return View("Search");
        }

        public async Task<IActionResult> SelectBooks(List<int> selecttions) {
            if (!isLogin())
            {
                return RedirectToAction("login", "user");
                ;
            }
            booksId.AddRange(selecttions);
            
            Dictionary<string,object> data = await bookService.BorrowBooks(Request.Cookies["token"]!,selecttions);
            int code = int.Parse(data["code"].ToString()!);
            if (code == 200)
            {
                return RedirectToAction("borrowed");
            }
            ViewBag.books = bookService.books;
            ViewData["message"] = data["message"];
            return View("Search");
        }        
    }
}