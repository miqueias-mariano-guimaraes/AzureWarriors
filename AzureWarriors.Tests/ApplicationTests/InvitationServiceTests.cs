using System;
using System.Threading.Tasks;
using Moq;
using Xunit;
using AzureWarriors.Application.Services;
using AzureWarriors.Domain.Entities;
using AzureWarriors.Domain.Enums;
using AzureWarriors.Application.Interfaces.Repositories;

namespace AzureWarriors.Tests.ApplicationTests
{
    public class InvitationServiceTests
    {
        public readonly Mock<IInvitationRepository> _mockInvitationRepository;
        public readonly Mock<IClanRepository> _mockClanRepository;
        public readonly Mock<IUserRepository> _mockUserRepository;
        public readonly InvitationService _mockInvitationService;
        public InvitationServiceTests()
        {
            _mockInvitationRepository = new Mock<IInvitationRepository>();
            _mockClanRepository = new Mock<IClanRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockInvitationService = new InvitationService(
                _mockInvitationRepository.Object,
                _mockClanRepository.Object,
                _mockUserRepository.Object);
        }

        [Fact]
        public async Task InviteUserToClanAsync_Should_Throw_When_Clan_Not_Found()
        {
            // Arrange
            var clanId = Guid.NewGuid();
            var leaderUserId = Guid.NewGuid();
            var userId = Guid.NewGuid();
             
            _mockClanRepository.Setup(r => r.GetByIdAsync(clanId)).ReturnsAsync((Clan)null);
             
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _mockInvitationService.InviteUserToClanAsync(clanId, leaderUserId, userId));
        }

        [Fact]
        public async Task InviteUserToClanAsync_Should_Throw_When_User_Not_Found()
        {
            // Arrange
            var clanId = Guid.NewGuid();
            var leaderUserId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var clan = new Clan(Guid.NewGuid(), leaderUserId, "ClanName") { Id = clanId, CommunityId = Guid.NewGuid() };
             
             
            _mockClanRepository.Setup(r => r.GetByIdAsync(clanId)).ReturnsAsync(clan);
            _mockUserRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _mockInvitationService.InviteUserToClanAsync(clanId, leaderUserId, userId));
        }

        [Fact]
        public async Task InviteUserToClanAsync_Should_Throw_When_Leader_Not_Found()
        {
            // Arrange
            var clanId = Guid.NewGuid();
            var leaderUserId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var clan = new Clan(Guid.NewGuid(), leaderUserId, "ClanName") { Id = clanId, CommunityId = Guid.NewGuid() };
             
             
            _mockClanRepository.Setup(r => r.GetByIdAsync(clanId)).ReturnsAsync(clan);
            _mockUserRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(new User("User") { Id = userId, CommunityId = clan.CommunityId });
            _mockUserRepository.Setup(r => r.GetByIdAsync(leaderUserId)).ReturnsAsync((User)null);
             
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _mockInvitationService.InviteUserToClanAsync(clanId, leaderUserId, userId));
        }

        [Fact]
        public async Task InviteUserToClanAsync_Should_Throw_When_Leader_Is_Not_Leader()
        {
            // Arrange
            var clanId = Guid.NewGuid();
            var leaderUserId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            // Leader ID does not match the one in clan.
            var clan = new Clan(Guid.NewGuid(), Guid.NewGuid(), "ClanName") { Id = clanId, CommunityId = Guid.NewGuid() };
             
            _mockClanRepository.Setup(r => r.GetByIdAsync(clanId)).ReturnsAsync(clan);
            _mockUserRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(new User("User") { Id = userId, CommunityId = clan.CommunityId });
            _mockUserRepository.Setup(r => r.GetByIdAsync(leaderUserId)).ReturnsAsync(new User("Leader") { Id = leaderUserId, CommunityId = clan.CommunityId });
             
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _mockInvitationService.InviteUserToClanAsync(clanId, leaderUserId, userId));
        }

        [Fact]
        public async Task InviteUserToClanAsync_Should_Throw_When_User_Not_In_Same_Community()
        {
            // Arrange
            var clanId = Guid.NewGuid();
            var leaderUserId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var clan = new Clan(Guid.NewGuid(), leaderUserId, "ClanName") { Id = clanId, CommunityId = Guid.NewGuid() };
             
            _mockClanRepository.Setup(r => r.GetByIdAsync(clanId)).ReturnsAsync(clan);
            // User has a different community
            _mockUserRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(new User("User") { Id = userId, CommunityId = Guid.NewGuid() });
            _mockUserRepository.Setup(r => r.GetByIdAsync(leaderUserId)).ReturnsAsync(new User("Leader") { Id = leaderUserId, CommunityId = clan.CommunityId });

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _mockInvitationService.InviteUserToClanAsync(clanId, leaderUserId, userId));
        }

        [Fact]
        public async Task InviteUserToClanAsync_Should_Create_Invitation_Successfully()
        {
            // Arrange
            var clanId = Guid.NewGuid();
            var leaderUserId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var clan = new Clan(Guid.NewGuid(), leaderUserId, "ClanName") { Id = clanId, CommunityId = Guid.NewGuid() };
            var user = new User("User") { Id = userId, CommunityId = clan.CommunityId };
            var leader = new User("Leader") { Id = leaderUserId, CommunityId = clan.CommunityId };
             
            _mockClanRepository.Setup(r => r.GetByIdAsync(clanId)).ReturnsAsync(clan);
            _mockUserRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
            _mockUserRepository.Setup(r => r.GetByIdAsync(leaderUserId)).ReturnsAsync(leader);
            _mockInvitationRepository.Setup(r => r.CreateAsync(It.IsAny<Invitation>())).Returns(Task.CompletedTask);
             
            // Act
            var invitation = await _mockInvitationService.InviteUserToClanAsync(clanId, leaderUserId, userId);

            // Assert
            Assert.NotNull(invitation);
            Assert.Equal(clanId, invitation.ClanId);
            Assert.Equal(userId, invitation.UserId);
            _mockInvitationRepository.Verify(r => r.CreateAsync(It.IsAny<Invitation>()), Times.Once);
        }

        [Fact]
        public async Task RespondInvitationAsync_Should_Throw_When_Invitation_Not_Found()
        {
            // Arrange
            var invitationId = Guid.NewGuid();
             
            _mockInvitationRepository.Setup(r => r.GetByIdAsync(invitationId)).ReturnsAsync((Invitation)null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _mockInvitationService.RespondInvitationAsync(invitationId, true));
        }

        [Fact]
        public async Task RespondInvitationAsync_Should_Update_Status_And_User_When_Accepted()
        {
            // Arrange
            var invitationId = Guid.NewGuid();
            var clanId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var invitation = new Invitation(clanId, userId) { Id = invitationId };
            var user = new User("User") { Id = userId };           
             
            _mockInvitationRepository.Setup(r => r.GetByIdAsync(invitationId)).ReturnsAsync(invitation);
            _mockInvitationRepository.Setup(r => r.UpdateStatusAsync(invitationId, invitation.Status)).Returns(Task.CompletedTask);
            _mockUserRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
            _mockUserRepository.Setup(r => r.UpdateAsync(user)).Returns(Task.CompletedTask);

            // Act
            await _mockInvitationService.RespondInvitationAsync(invitationId, true);

            // Assert
            Assert.Equal(InvitationStatus.Accepted, invitation.Status);
            _mockInvitationRepository.Verify(r => r.UpdateStatusAsync(invitationId, InvitationStatus.Accepted), Times.Once);
            _mockUserRepository.Verify(r => r.UpdateAsync(It.Is<User>(u => u.Id == userId && u.ClanId == clanId && u.Points == 0)), Times.Once);
        }

        [Fact]
        public async Task RespondInvitationAsync_Should_Update_Status_When_Declined_And_Not_Update_User()
        {
            // Arrange
            var invitationId = Guid.NewGuid();
            var clanId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var invitation = new Invitation(clanId, userId) { Id = invitationId };             

            _mockInvitationRepository.Setup(r => r.GetByIdAsync(invitationId)).ReturnsAsync(invitation);
            _mockInvitationRepository.Setup(r => r.UpdateStatusAsync(invitationId, invitation.Status)).Returns(Task.CompletedTask);             

            // Act
            await _mockInvitationService.RespondInvitationAsync(invitationId, false);

            // Assert
            Assert.Equal(InvitationStatus.Declined, invitation.Status);
            _mockInvitationRepository.Verify(r => r.UpdateStatusAsync(invitationId, InvitationStatus.Declined), Times.Once);
            // Verify that UpdateAsync on user is never called for a declined invitation.
            _mockUserRepository.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
        }
    }
}
