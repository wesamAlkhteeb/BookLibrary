using BookSystem.UI.Models;
using BookSystem.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookSystem.UI.Controllers
{
    public class UserController:Controller
    {
        private readonly AccountService accountService;

        public UserController(AccountService accountService)
        {
            this.accountService = accountService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            string token = Request.Cookies["token"]!;
            if (!String.IsNullOrEmpty(token))
            {
                return RedirectToAction("search", "book");
            }
            return View();
        }

        [HttpPost]
        [ActionName("Login")]
        public async Task<IActionResult> SubmitLogin(string name , string password)
        {
            if (String.IsNullOrEmpty(name) && String.IsNullOrEmpty(password) ) {
                return View();
            }
            Dictionary<string,object> data = await accountService.Login(new AccountModel
            {
                name = name,
                Password = password
            },Response);
            int code = int.Parse(data["code"].ToString()!);
            if (code==400)
            {
                ViewData["message"]= data["message"];
                return View();
            }
            return RedirectToAction("search","book");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ActionName("Register")]
        public async Task<IActionResult> SubmitRegister(string name , string password)
        {
            if(string.IsNullOrEmpty(name) && string.IsNullOrEmpty(password))
            {
                return View();
            }
            Dictionary<string, object> response = await accountService.Register(new AccountModel
            {
                name = name, Password = password
            });
            int code = int.Parse(response["code"].ToString()!);
            if (code == 400)
            {
                ViewData["message"] = response["message"];
                return View();
            }
            return RedirectToAction("login");
            
        }

    }
}
