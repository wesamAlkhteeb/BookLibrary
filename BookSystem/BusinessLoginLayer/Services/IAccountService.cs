using DataAccessLayer.Models.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public interface IAccountService
    {
        Task<Dictionary<string,object>> Register(AccountModel registerModel);
        Task<Dictionary<string,object>> Login(AccountModel LoginModel);
    }
}
