using System;
using System.Data;
using System.Threading.Tasks;
using Moq;
using Moq.Dapper;
using Xunit;
using AzureWarriors.Domain.Entities;
using AzureWarriors.Domain.Enums;
using AzureWarriors.Infrastructure.Repositories;
using AzureWarriors.Infrastructure.Data;
using Dapper;

namespace AzureWarriors.Tests.InfrastructureTests
{
    public class InvitationRepositoryTests
    {
        [Fact]
        public async Task CreateAsync_Does_Not_Throw_Exception()
        {
            // Arrange
            var invitation = new Invitation(Guid.NewGuid(), Guid.NewGuid());
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

            var repository = new InvitationRepository(mockFactory.Object);

            // Act & Assert
            await repository.CreateAsync(invitation);
        }

        [Fact]
        public async Task GetByIdAsync_Returns_Invitation()
        {
            // Arrange
            var invitationId = Guid.NewGuid();
            var expectedInvitation = new Invitation(Guid.NewGuid(), Guid.NewGuid())
            {
                Id = invitationId,
                Status = InvitationStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(c => c.State).Returns(ConnectionState.Open);
            mockConnection.SetupDapperAsync<Invitation>(c => c.QuerySingleOrDefaultAsync<Invitation>(
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<IDbTransaction>(),
                    It.IsAny<int?>(),
                    It.IsAny<CommandType?>()))
                .ReturnsAsync(expectedInvitation);

            var mockFactory = new Mock<IDbConnectionFactory>();
            mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection.Object);

            var repository = new InvitationRepository(mockFactory.Object);

            // Act
            var result = await repository.GetByIdAsync(invitationId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedInvitation.Id, result.Id);
        }

        [Fact]
        public async Task UpdateStatusAsync_Does_Not_Throw_Exception()
        {
            // Arrange
            var invitationId = Guid.NewGuid();
            var newStatus = InvitationStatus.Accepted;
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

            var repository = new InvitationRepository(mockFactory.Object);

            // Act & Assert
            await repository.UpdateStatusAsync(invitationId, newStatus);
        }
    }
}
