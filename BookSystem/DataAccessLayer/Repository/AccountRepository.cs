using DataAccessLayer.DatabaseContext;
using DataAccessLayer.Models.Account;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DataAccessLayer.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly DbContext database;

        public AccountRepository(DbContext database)
        {
            this.database = database;
        }

        public async Task<int> GetAccountId(AccountModel accountModel)
        {
            DataSet ds = await database.DoQueryWithData(
                    "Select id from UserLibrary where username=@name and password=@password",
                    (sda) =>
                    {
                        sda.SelectCommand.Parameters.AddWithValue("@name", accountModel.Name);
                        sda.SelectCommand.Parameters.AddWithValue("@password", accountModel.Password);
                    });
            if (ds.Tables.Count == 0 ||ds.Tables[0].Rows.Count == 0)
                return 0; // cannot found id 0 => to handle
            DataRow dr = ds.Tables[0].Rows[0];
            return int.Parse(dr["id"].ToString()!);
        }

        public async Task<bool> IsNameExists(string name)
        {
            DataSet ds = await database.DoQueryWithData(
                    "Select id from UserLibrary where username=@name",
                    (sda) =>
                    {
                        sda.SelectCommand.Parameters.AddWithValue("@name", name);
                    });
            return ds.Tables.Count>0 && ds.Tables[0].Rows.Count == 1;
        }

        public async Task<bool> StoreAccount(AccountModel registerModel)
        {
            int rowAffected = await database.DoQueryWithoutData(
                    "INSERT INTO UserLibrary VALUES (@name,@password)",
                    (sda) =>
                    {
                        sda.Parameters.AddWithValue("@name", registerModel.Name);
                        sda.Parameters.AddWithValue("@password", registerModel.Password);
                    });
            return rowAffected>0;
        }
    }
}
