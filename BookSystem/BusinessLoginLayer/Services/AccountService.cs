using BusinessLayer.Helper.Security;
using DataAccessLayer;
using DataAccessLayer.Models.Account;
using DataAccessLayer.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace BusinessLayer.Services
{
    internal class AccountService : IAccountService
    {
        private readonly IAccountRepository accountRepository;
        private readonly JwtSettings jwtSettings;
        public AccountService(IAccountRepository accountRepository , IOptions<JwtSettings> options)
        {
            this.accountRepository = accountRepository;
            this.jwtSettings = options.Value;
        }
        public async Task<Dictionary<string, object>> Login(AccountModel loginModel)
        {
            loginModel.Password = JwtSecurity.securityData.getHashPassword(loginModel.Password);
            int id = await accountRepository.GetAccountId(loginModel);
            if(id == 0)
            {
                throw new BadHttpRequestException("Username or password is uncorrect.");
            }

            return new Dictionary<string, object>
            {
                {"message","Login has successfuly" },
                {"info",JwtSecurity.securityData.GenerateToken(loginModel.Name, id , Roles.user.ToString(), jwtSettings)}
            };
        }

        public async Task<Dictionary<string, object>> Register(AccountModel registerModel)
        {
            bool accountExists = await accountRepository.IsNameExists(registerModel.Name);
            if (accountExists)
            {
                throw new BadHttpRequestException("this name is already used.");
            }
            registerModel.Password = JwtSecurity.securityData.getHashPassword(registerModel.Password);
            bool isCompleted = await accountRepository.StoreAccount(registerModel);
            if(!isCompleted)
            {
                throw new BadHttpRequestException("Register is not completed. Please try again.");
            }
            return new Dictionary<string, object>
            {
                {"message","Register has successfully."}
            };
        }
    }
}
