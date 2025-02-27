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
    public class CommunityRepositoryTests
    {
        [Fact]
        public async Task CreateAsync_Does_Not_Throw_Exception()
        {
            // Arrange
            var community = new Community("Test Community", "Test Description");
            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(c => c.State).Returns(ConnectionState.Open);
            // Simulate successful execution by returning 1.
            mockConnection.SetupDapperAsync<int>(c => c.ExecuteAsync(
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<IDbTransaction>(),
                    It.IsAny<int?>(),
                    It.IsAny<CommandType?>()))
                .ReturnsAsync(1);

            var mockFactory = new Mock<IDbConnectionFactory>();
            mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection.Object);

            var repository = new CommunityRepository(mockFactory.Object);

            // Act & Assert: simply ensure no exception is thrown.
            await repository.CreateAsync(community);
        }

        [Fact]
        public async Task GetByIdAsync_Returns_Community()
        {
            // Arrange
            var communityId = Guid.NewGuid();
            var expectedCommunity = new Community("Test Community", "Test Description")
            {
                Id = communityId,
                CreatedAt = DateTime.UtcNow
            };

            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(c => c.State).Returns(ConnectionState.Open);
            mockConnection.SetupDapperAsync<Community>(c => c.QuerySingleOrDefaultAsync<Community>(
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<IDbTransaction>(),
                    It.IsAny<int?>(),
                    It.IsAny<CommandType?>()))
                .ReturnsAsync(expectedCommunity);

            var mockFactory = new Mock<IDbConnectionFactory>();
            mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection.Object);

            var repository = new CommunityRepository(mockFactory.Object);

            // Act
            var result = await repository.GetByIdAsync(communityId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedCommunity.Id, result.Id);
        }
    }
}
