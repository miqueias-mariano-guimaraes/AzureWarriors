using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Xunit;
using AzureWarriors.Infrastructure.Data;

namespace AzureWarriors.Tests.InfrastructureTests
{
    public class DbConnectionFactoryTests
    {
        private readonly DbConnectionFactory _factory;
        private readonly string _expectedConnectionString;

        public DbConnectionFactoryTests()
        {
            _expectedConnectionString = "Server=tcp:test.database.windows.net,1433;Database=TestDB;User ID=test;Password=test;";

            // Build an in-memory configuration with the connection string.
            var inMemorySettings = new Dictionary<string, string>
            {
                {"ConnectionStrings:SqlConnection", _expectedConnectionString}
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _factory = new DbConnectionFactory(configuration, "SqlConnection");
        }

        [Fact]
        public void CreateConnection_Returns_SqlConnection_With_CorrectConnectionString()
        {
            // Act
            var connection = _factory.CreateConnection();

            // Assert
            Assert.IsType<SqlConnection>(connection);
            Assert.Equal(_expectedConnectionString, connection.ConnectionString);
        }
    }
}
