
using System.Data;
using Microsoft.Extensions.Configuration;
using Npgsql; 

namespace Infrastructure.Data
{
    public class DbConnectionFactory
    {
        private readonly string _connectionString;

        public DbConnectionFactory(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DB");
        }

        public IDbConnection CreateConnection() => new NpgsqlConnection(_connectionString);
        
    }
}
