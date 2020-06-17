using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace SamLearnsAzure.Service.Dapper
{
    public class BaseDataAccess<T>
    {
        public string? ConnectionString = null;

        public void SetupConnectionString(IConfiguration configuration)
        {
            string sqlConnectionStringName = "ConnectionStrings:SamsAppConnectionString" + configuration["AppSettings:Environment"];
            ConnectionString = configuration[sqlConnectionStringName];
        }

        public async Task<IEnumerable<T>> GetList(string query, DynamicParameters? parameters = null)
        {
            if (ConnectionString == null)
            {
                throw new Exception("ConnectionString not set");
            }

            IEnumerable<T> results;
            SqlConnection connection = new SqlConnection(ConnectionString);
            try
            {
                await connection.OpenAsync();
                results = await connection.QueryAsync<T>(query, parameters, commandType: CommandType.StoredProcedure);
            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    await connection.CloseAsync();
                }
            }
            return results;//.ToList<T>();
        }

        public async Task<T> GetItem(string query, DynamicParameters? parameters = null)
        {
            if (ConnectionString == null)
            {
                throw new Exception("ConnectionString not set");
            }

            T result;
            SqlConnection connection = new SqlConnection(ConnectionString);
            try
            {
                await connection.OpenAsync();
                IEnumerable<T> results = await connection.QueryAsync<T>(query, parameters, commandType: CommandType.StoredProcedure);
                result = results.FirstOrDefault<T>();
            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    await connection.CloseAsync();
                }
            }
            return result;
        }

        public async Task<bool> SaveItem(string query, DynamicParameters? parameters = null)
        {
            if (ConnectionString == null)
            {
                throw new Exception("ConnectionString not set");
            }

            bool result;
            SqlConnection connection = new SqlConnection(ConnectionString);
            try
            {
                await connection.OpenAsync();
                await connection.ExecuteScalarAsync<bool>(query, parameters, commandType: CommandType.StoredProcedure);
                result = true;
            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    await connection.CloseAsync();
                }
            }
            return result;
        }
    }
}
