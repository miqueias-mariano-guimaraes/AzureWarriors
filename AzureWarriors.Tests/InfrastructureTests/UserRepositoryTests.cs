using System;
using System.Data;
using System.Threading.Tasks;
using Moq;
using Moq.Dapper;
using Xunit;
using AzureWarriors.Domain.Entities;
using AzureWarriors.Infrastructure.Repositories;
using AzureWarriors.Infrastructure.Data;
using Dapper;

namespace AzureWarriors.Tests.InfrastructureTests
{
    public class UserRepositoryTests
    {
        [Fact]
        public async Task CreateAsync_Does_Not_Throw_Exception()
        {
            // Arrange
            var user = new User("TestUser");
            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(c => c.State).Returns(ConnectionState.Open);
            mockConnection.SetupDapperAsync<int>(c => c.ExecuteAsync(
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<IDbTransaction>(),
                    It.IsAny<int?>(),
                    It.IsAny<CommandType?>()))
                .ReturnsAsync(1);

            var mockFactory = new Mock<IDbConnectionFactory>();
            mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection.Object);

            var repository = new UserRepository(mockFactory.Object);

            // Act & Assert
            await repository.CreateAsync(user);
        }

        [Fact]
        public async Task GetByIdAsync_Returns_User()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var expectedUser = new User("TestUser")
            {
                Id = userId,
                Points = 0
            };

            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(c => c.State).Returns(ConnectionState.Open);
            mockConnection.SetupDapperAsync<User>(c => c.QuerySingleOrDefaultAsync<User>(
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<IDbTransaction>(),
                    It.IsAny<int?>(),
                    It.IsAny<CommandType?>()))
                .ReturnsAsync(expectedUser);

            var mockFactory = new Mock<IDbConnectionFactory>();
            mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection.Object);

            var repository = new UserRepository(mockFactory.Object);

            // Act
            var result = await repository.GetByIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedUser.Id, result.Id);
        }

        [Fact]
        public async Task UpdateAsync_Does_Not_Throw_Exception()
        {
            // Arrange
            var user = new User("TestUser") { Id = Guid.NewGuid(), Points = 10 };
            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(c => c.State).Returns(ConnectionState.Open);
            mockConnection.SetupDapperAsync<int>(c => c.ExecuteAsync(
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<IDbTransaction>(),
                    It.IsAny<int?>(),
                    It.IsAny<CommandType?>()))
                .ReturnsAsync(1);

            var mockFactory = new Mock<IDbConnectionFactory>();
            mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection.Object);

            var repository = new UserRepository(mockFactory.Object);

            // Act & Assert
            await repository.UpdateAsync(user);
        }
    }
}
