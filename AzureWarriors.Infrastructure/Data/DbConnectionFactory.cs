using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace AzureWarriors.Infrastructure.Data
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionStringName;

        public DbConnectionFactory(IConfiguration configuration, string connectionStringName = "SqlConnection")
        {
            _configuration = configuration;
            _connectionStringName = connectionStringName;
        }

        public IDbConnection CreateConnection()
        {
            var connString = _configuration.GetConnectionString(_connectionStringName);
            return new SqlConnection(connString);
        }
    }

    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
