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
    public class ClanRepositoryTests
    {
        [Fact]
        public async Task CreateAsync_Does_Not_Throw_Exception()
        {
            // Arrange
            var clan = new Clan(Guid.NewGuid(), Guid.NewGuid(), "Test Clan");
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

            var repository = new ClanRepository(mockFactory.Object);

            // Act & Assert
            await repository.CreateAsync(clan);
        }

        [Fact]
        public async Task GetByIdAsync_Returns_Clan()
        {
            // Arrange
            var clanId = Guid.NewGuid();
            var expectedClan = new Clan(Guid.NewGuid(), Guid.NewGuid(), "Test Clan")
            {
                Id = clanId,
                CreatedAt = DateTime.UtcNow
            };

            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(c => c.State).Returns(ConnectionState.Open);
            mockConnection.SetupDapperAsync<Clan>(c => c.QuerySingleOrDefaultAsync<Clan>(
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<IDbTransaction>(),
                    It.IsAny<int?>(),
                    It.IsAny<CommandType?>()))
                .ReturnsAsync(expectedClan);

            var mockFactory = new Mock<IDbConnectionFactory>();
            mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection.Object);

            var repository = new ClanRepository(mockFactory.Object);

            // Act
            var result = await repository.GetByIdAsync(clanId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedClan.Id, result.Id);
        }
    }
}
