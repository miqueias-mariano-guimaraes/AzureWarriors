using AzureWarriors.Application.Interfaces.Repositories;
using AzureWarriors.Application.Services;
using AzureWarriors.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Moq;
using System.Text;

namespace AzureWarriors.Tests.ApplicationTests
{
    public class ClanServiceTests
    {
        public readonly Mock<IClanRepository> _mockClanRepository;
        public readonly Mock<ICommunityRepository> _mockCommunityRepository;
        public readonly Mock<IUserRepository> _mockUserRepository;
        public readonly ClanService _mockClanService;
        public ClanServiceTests()
        {
            _mockClanRepository = new Mock<IClanRepository>();
            _mockCommunityRepository = new Mock<ICommunityRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockClanService = new ClanService(
                _mockClanRepository.Object,
                _mockCommunityRepository.Object,
                _mockUserRepository.Object);
        }

        [Fact]
        public async Task CreateClanAsync_Should_Throw_When_Community_Not_Found()
        {
            // Arrange
            var communityId = Guid.NewGuid();
            var leaderUserId = Guid.NewGuid();

            _mockCommunityRepository.Setup(r => r.GetByIdAsync(communityId)).ReturnsAsync((Community)null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _mockClanService.CreateClanAsync(communityId, leaderUserId, "ClanName"));
        }

        [Fact]
        public async Task CreateClanAsync_Should_Throw_When_Leader_Not_Found()
        {
            // Arrange
            var communityId = Guid.NewGuid();
            var leaderUserId = Guid.NewGuid();
            var community = new Community("Comm", "Desc") { Id = communityId };

            _mockCommunityRepository.Setup(r => r.GetByIdAsync(communityId)).ReturnsAsync(community);
            _mockUserRepository.Setup(r => r.GetByIdAsync(leaderUserId)).ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _mockClanService.CreateClanAsync(communityId, leaderUserId, "ClanName"));
        }

        [Fact]
        public async Task CreateClanAsync_Should_Throw_When_Leader_Not_In_Community()
        {
            // Arrange
            var communityId = Guid.NewGuid();
            var leaderUserId = Guid.NewGuid();
            var community = new Community("Comm", "Desc") { Id = communityId };

            // Leader belongs to another community.
            var leader = new User("Leader") { Id = leaderUserId, CommunityId = Guid.NewGuid() };

            _mockCommunityRepository.Setup(r => r.GetByIdAsync(communityId)).ReturnsAsync(community);
            _mockUserRepository.Setup(r => r.GetByIdAsync(leaderUserId)).ReturnsAsync(leader);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _mockClanService.CreateClanAsync(communityId, leaderUserId, "ClanName"));
        }

        [Fact]
        public async Task CreateClanAsync_Should_Create_Clan_Successfully()
        {
            // Arrange
            var communityId = Guid.NewGuid();
            var leaderUserId = Guid.NewGuid();

            var community = new Community("Comm", "Desc") { Id = communityId };
            var leader = new User("Leader") { Id = leaderUserId, CommunityId = communityId };

            _mockCommunityRepository.Setup(r => r.GetByIdAsync(communityId)).ReturnsAsync(community);
            _mockUserRepository.Setup(r => r.GetByIdAsync(leaderUserId)).ReturnsAsync(leader);
            _mockClanRepository.Setup(r => r.CreateAsync(It.IsAny<Clan>())).Returns(Task.CompletedTask);
            _mockUserRepository.Setup(r => r.UpdateAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
            var _mockClanService = new ClanService(_mockClanRepository.Object, _mockCommunityRepository.Object, _mockUserRepository.Object);

            // Act
            var clan = await _mockClanService.CreateClanAsync(communityId, leaderUserId, "ClanName");

            // Assert
            Assert.NotNull(clan);
            Assert.Equal(communityId, clan.CommunityId);
            Assert.Equal(leaderUserId, clan.LeaderUserId);
            _mockClanRepository.Verify(r => r.CreateAsync(It.IsAny<Clan>()), Times.Once);
            _mockUserRepository.Verify(r => r.UpdateAsync(It.Is<User>(u => u.Id == leaderUserId && u.ClanId == clan.Id)), Times.Once);
        }

        [Fact]
        public async Task GetClanAsync_Should_Return_Clan()
        {
            // Arrange
            var clanId = Guid.NewGuid();
            var expectedClan = new Clan(Guid.NewGuid(), Guid.NewGuid(), "Test Clan") { Id = clanId };

            _mockClanRepository.Setup(r => r.GetByIdAsync(clanId)).ReturnsAsync(expectedClan);

            // Act
            var clan = await _mockClanService.GetClanAsync(clanId);

            // Assert
            Assert.NotNull(clan);
            Assert.Equal(expectedClan.Id, clan.Id);
        }

        [Fact]
        public async Task KickMemberAsync_Should_Reset_TargetUser_And_Remove_From_Clan()
        {
            // Arrange
            var clanId = Guid.NewGuid();
            var leaderUserId = Guid.NewGuid();
            var targetUserId = Guid.NewGuid();

            // Setup target user is in the clan.
            var targetUser = new User("Target") { Id = targetUserId, ClanId = clanId, Points = 100 };
            // Setup leader user is in the clan and is leader.
            var leaderUser = new User("Leader") { Id = leaderUserId, ClanId = clanId };
            // The clan with leaderUser as leader.
            var clan = new Clan(Guid.NewGuid(), leaderUserId, "ClanName") { Id = clanId };

            _mockUserRepository.Setup(r => r.GetByIdAsync(targetUserId)).ReturnsAsync(targetUser);
            _mockUserRepository.Setup(r => r.GetByIdAsync(leaderUserId)).ReturnsAsync(leaderUser);
            _mockUserRepository.Setup(r => r.UpdateAsync(targetUser)).Returns(Task.CompletedTask);

            _mockClanRepository.Setup(r => r.GetByIdAsync(clanId)).ReturnsAsync(clan);

            // Act
            await _mockClanService.KickMemberAsync(clanId, leaderUserId, targetUserId);

            // Assert
            Assert.Equal(0, targetUser.Points);
            Assert.Null(targetUser.ClanId);
            _mockUserRepository.Verify(r => r.UpdateAsync(targetUser), Times.Once);
        }

        [Fact]
        public async Task KickMemberAsync_Should_Throw_When_TargetUser_Not_In_Clan()
        {
            // Arrange
            var clanId = Guid.NewGuid();
            var leaderUserId = Guid.NewGuid();
            var targetUserId = Guid.NewGuid();
            // Target user is not associated with any clan.
            var targetUser = new User("Target") { Id = targetUserId, ClanId = null };

            _mockUserRepository.Setup(r => r.GetByIdAsync(targetUserId)).ReturnsAsync(targetUser);


            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _mockClanService.KickMemberAsync(clanId, leaderUserId, targetUserId));
        }
    }
}
