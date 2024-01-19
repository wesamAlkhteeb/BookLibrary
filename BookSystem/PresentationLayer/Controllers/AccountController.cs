
using BusinessLayer.Services;
using DataAccessLayer.Models.Account;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController:ControllerBase
    {
        private readonly IAccountService accountService;

        public AccountController(IAccountService accountService)
        {
            this.accountService = accountService;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AccountModel registerModel)
        {
            return Ok(await accountService.Register(registerModel));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AccountModel loginModel)
        {
            return Ok(await accountService.Login(loginModel));
        }
    }
}
