using DataAccessLayer.Models.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DataAccessLayer.DatabaseContext.DbContext;

namespace DataAccessLayer.DatabaseContext
{
    public class DbContext
    {
        private readonly DatabaseSettings databaseSettings;
        public delegate void QueryContext<T>(T t);
        public delegate Task<string> QueryContextTransaction<T>(T t);
        public DbContext(IOptions<DatabaseSettings> option)
        {
            this.databaseSettings = option.Value;
        }

        // for select
        public async Task<DataSet> DoQueryWithData(string query, QueryContext<SqlDataAdapter>? queryContext = null)
        {
            using (SqlConnection sqlConnection = new SqlConnection(databaseSettings.ConnectionString))
            {
                await sqlConnection.OpenAsync();
                SqlDataAdapter dataAdapter = new SqlDataAdapter(query,sqlConnection);
                if(queryContext != null)
                    queryContext!.Invoke(dataAdapter);
                DataSet dataSet = new DataSet();
                await Task.Run(() => dataAdapter.Fill(dataSet));
                return dataSet;
            }
        }


        // for insert - update - delete
        public async Task<int> DoQueryWithoutData(string query, QueryContext<SqlCommand>? queryContext = null)
        {
            using (SqlConnection sqlConnection = new SqlConnection(databaseSettings.ConnectionString))
            {
                await sqlConnection.OpenAsync();
                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    if (queryContext != null)
                        queryContext!.Invoke(sqlCommand);

                    return await sqlCommand.ExecuteNonQueryAsync();
                }
                    
            }
        }

        public async Task<string> TransactionQuery(QueryContextTransaction<SqlTransaction> queryContext)
        {
            using (SqlConnection sqlConnection = new SqlConnection(databaseSettings.ConnectionString))
            {
                await sqlConnection.OpenAsync();
                using (SqlTransaction st = sqlConnection.BeginTransaction())
                {
                    return await queryContext(st);
                }
            }
        }

    }
}
