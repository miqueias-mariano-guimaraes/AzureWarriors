using System;
using System.Threading.Tasks;
using Moq;
using Xunit;
using AzureWarriors.Application.Services;
using AzureWarriors.Domain.Entities;
using AzureWarriors.Application.Interfaces.Repositories;

namespace AzureWarriors.Tests.ApplicationTests
{
    public class UserServiceTests
    {
        public readonly Mock<IUserRepository> _mockUserRepository;
        public readonly Mock<ICommunityRepository> _mockCommunityRepository;
        public readonly Mock<IClanRepository> _mockClanRepository;
        public readonly UserService _mockUserService;
        public UserServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockCommunityRepository = new Mock<ICommunityRepository>();
            _mockClanRepository = new Mock<IClanRepository>();
            _mockUserService = new UserService(
                _mockUserRepository.Object,
                _mockCommunityRepository.Object,
                _mockClanRepository.Object);
        }

        [Fact]
        public async Task CreateUserAsync_Should_Throw_When_Username_Is_Empty()
        {
            // Arrange, Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _mockUserService.CreateUserAsync(""));
        }

        [Fact]
        public async Task CreateUserAsync_Should_Throw_When_User_Already_Exists()
        {
            // Arrange
            var username = "TestUser";
            var existingUser = new User(username) { Id = Guid.NewGuid() };
            
            _mockUserRepository.Setup(r => r.GetByUsernameAsync(username)).ReturnsAsync(existingUser);
 
            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _mockUserService.CreateUserAsync(username));
        }

        [Fact]
        public async Task CreateUserAsync_Should_Create_User_Successfully()
        {
            // Arrange
            var username = "TestUser";
            
            _mockUserRepository.Setup(r => r.GetByUsernameAsync(username)).ReturnsAsync((User)null);
            _mockUserRepository.Setup(r => r.CreateAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
             
            // Act
            var user = await _mockUserService.CreateUserAsync(username);

            // Assert
            Assert.NotNull(user);
            Assert.Equal(username, user.Username);
            _mockUserRepository.Verify(r => r.CreateAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task JoinCommunityAsync_Should_Throw_When_User_Not_Found()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var communityId = Guid.NewGuid();
            
            _mockUserRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _mockUserService.JoinCommunityAsync(userId, communityId));
        }

        [Fact]
        public async Task JoinCommunityAsync_Should_Throw_When_Community_Not_Found()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var communityId = Guid.NewGuid();
            var user = new User("TestUser") { Id = userId };
            
            _mockUserRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
             
            _mockCommunityRepository.Setup(r => r.GetByIdAsync(communityId)).ReturnsAsync((Community)null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _mockUserService.JoinCommunityAsync(userId, communityId));
        }

        [Fact]
        public async Task JoinCommunityAsync_Should_Reset_Points_And_Clear_Clan_When_Changing_Community()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var oldCommunityId = Guid.NewGuid();
            var newCommunityId = Guid.NewGuid();
            var user = new User("TestUser")
            {
                Id = userId,
                CommunityId = oldCommunityId,
                ClanId = Guid.NewGuid(),
                Points = 100
            };
            var community = new Community("NewCommunity", "Desc") { Id = newCommunityId };
            
            _mockUserRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
             
            _mockCommunityRepository.Setup(r => r.GetByIdAsync(newCommunityId)).ReturnsAsync(community);
            _mockUserRepository.Setup(r => r.UpdateAsync(user)).Returns(Task.CompletedTask);

            // Act
            await _mockUserService.JoinCommunityAsync(userId, newCommunityId);

            // Assert
            Assert.Equal(newCommunityId, user.CommunityId);
            Assert.Null(user.ClanId);
            Assert.Equal(0, user.Points);
            _mockUserRepository.Verify(r => r.UpdateAsync(user), Times.Once);
        }

        [Fact]
        public async Task JoinClanAsync_Should_Throw_When_User_Not_Found()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var clanId = Guid.NewGuid();
            
            _mockUserRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _mockUserService.JoinClanAsync(userId, clanId));
        }

        [Fact]
        public async Task JoinClanAsync_Should_Throw_When_Clan_Not_Found()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var clanId = Guid.NewGuid();
            var user = new User("TestUser") { Id = userId, CommunityId = Guid.NewGuid() };
            
            _mockUserRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);           
            _mockClanRepository.Setup(r => r.GetByIdAsync(clanId)).ReturnsAsync((Clan)null);
            
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _mockUserService.JoinClanAsync(userId, clanId));
        }

        [Fact]
        public async Task JoinClanAsync_Should_Throw_When_User_Not_In_Same_Community()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var clanId = Guid.NewGuid();
            var user = new User("TestUser") { Id = userId, CommunityId = Guid.NewGuid() };
            var clan = new Clan(Guid.NewGuid(), Guid.NewGuid(), "ClanName") { Id = clanId, CommunityId = Guid.NewGuid() };
            
            _mockUserRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
            _mockClanRepository.Setup(r => r.GetByIdAsync(clanId)).ReturnsAsync(clan);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _mockUserService.JoinClanAsync(userId, clanId));
        }

        [Fact]
        public async Task JoinClanAsync_Should_Reset_Points_And_Update_User_When_Changing_Clan()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var oldClanId = Guid.NewGuid();
            var newClanId = Guid.NewGuid();
            var communityId = Guid.NewGuid();
            var user = new User("TestUser")
            {
                Id = userId,
                CommunityId = communityId,
                ClanId = oldClanId,
                Points = 100
            };
            var clan = new Clan(communityId, Guid.NewGuid(), "NewClan") { Id = newClanId, CommunityId = communityId };
            
            _mockUserRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
            _mockClanRepository.Setup(r => r.GetByIdAsync(newClanId)).ReturnsAsync(clan);
            _mockUserRepository.Setup(r => r.UpdateAsync(user)).Returns(Task.CompletedTask);

            // Act
            await _mockUserService.JoinClanAsync(userId, newClanId);

            // Assert
            Assert.Equal(newClanId, user.ClanId);
            Assert.Equal(0, user.Points);
            _mockUserRepository.Verify(r => r.UpdateAsync(user), Times.Once);
        }

        [Fact]
        public async Task LeaveClanAsync_Should_Throw_When_User_Not_Found()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var clanId = Guid.NewGuid();
            
            _mockUserRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _mockUserService.LeaveClanAsync(userId, clanId));
        }

        [Fact]
        public async Task LeaveClanAsync_Should_Throw_When_User_Not_In_Clan()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var clanId = Guid.NewGuid();
            var user = new User("TestUser") { Id = userId, ClanId = null };
            
            _mockUserRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _mockUserService.LeaveClanAsync(userId, clanId));
        }

        [Fact]
        public async Task LeaveClanAsync_Should_Throw_When_Clan_Not_Found()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var clanId = Guid.NewGuid();
            var user = new User("TestUser") { Id = userId, ClanId = clanId };
            
            _mockUserRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
            _mockClanRepository.Setup(r => r.GetByIdAsync(clanId)).ReturnsAsync((Clan)null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _mockUserService.LeaveClanAsync(userId, clanId));
        }

        [Fact]
        public async Task LeaveClanAsync_Should_Throw_When_User_Not_In_Specified_Clan()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var clanId = Guid.NewGuid();
            var user = new User("TestUser") { Id = userId, ClanId = Guid.NewGuid() };
            
            _mockUserRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
            _mockClanRepository.Setup(r => r.GetByIdAsync(clanId)).ReturnsAsync(new Clan(Guid.NewGuid(), Guid.NewGuid(), "Clan"));
             

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _mockUserService.LeaveClanAsync(userId, clanId));
        }

        [Fact]
        public async Task LeaveClanAsync_Should_Reset_Points_And_Remove_User_From_Clan()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var clanId = Guid.NewGuid();
            var user = new User("TestUser") { Id = userId, ClanId = clanId, Points = 50 };
            var clan = new Clan(Guid.NewGuid(), Guid.NewGuid(), "Clan") { Id = clanId };
            
            _mockUserRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
            _mockClanRepository.Setup(r => r.GetByIdAsync(clanId)).ReturnsAsync(clan);
            _mockUserRepository.Setup(r => r.UpdateAsync(user)).Returns(Task.CompletedTask);

            // Act
            await _mockUserService.LeaveClanAsync(userId, clanId);

            // Assert
            Assert.Null(user.ClanId);
            Assert.Equal(0, user.Points);
            _mockUserRepository.Verify(r => r.UpdateAsync(user), Times.Once);
        }
    }
}
