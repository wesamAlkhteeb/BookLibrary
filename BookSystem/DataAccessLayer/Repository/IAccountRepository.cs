using DataAccessLayer.Models.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
    public interface IAccountRepository
    {
        Task<bool> IsNameExists(string name);
        Task<bool> StoreAccount(AccountModel registerModel);
        Task<int> GetAccountId(AccountModel accountModel);
    }
}
