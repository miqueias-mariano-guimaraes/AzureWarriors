using AzureWarriors.Application.DTOs;
using AzureWarriors.Application.Interfaces.Repositories;
using AzureWarriors.Application.Services;
using AzureWarriors.Domain.Entities;
using Moq;

namespace AzureWarriors.Tests.ApplicationTests
{
    public class CommunityServiceTests
    {
        public readonly Mock<ICommunityRepository> _mockCommunityRepository;
        public readonly CommunityService _mockCommunityService;
        public CommunityServiceTests()
        {
            _mockCommunityRepository = new Mock<ICommunityRepository>();
            _mockCommunityService = new CommunityService(
                _mockCommunityRepository.Object);
        }

        [Fact]
        public async Task CreateCommunityAsync_Should_Throw_When_Name_Is_Empty()
        {
            // Arrange
            var dto = new CreateCommunityDto { Name = "", Description = "Some description" };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _mockCommunityService.CreateCommunityAsync(dto));
        }

        [Fact]
        public async Task CreateCommunityAsync_Should_Create_Community_Successfully()
        {
            // Arrange
            var dto = new CreateCommunityDto { Name = "Community1", Description = "Desc" };

            _mockCommunityRepository.Setup(r => r.CreateAsync(It.IsAny<Community>())).Returns(Task.CompletedTask);

            // Act
            var community = await _mockCommunityService.CreateCommunityAsync(dto);

            // Assert
            Assert.NotNull(community);
            Assert.Equal(dto.Name, community.Name);
            Assert.Equal(dto.Description, community.Description);
            _mockCommunityRepository.Verify(r => r.CreateAsync(It.IsAny<Community>()), Times.Once);
        }

        [Fact]
        public async Task GetCommunityAsync_Should_Return_Community()
        {
            // Arrange
            var communityId = Guid.NewGuid();
            var expectedCommunity = new Community("Community1", "Desc") { Id = communityId };

            _mockCommunityRepository.Setup(r => r.GetByIdAsync(communityId)).ReturnsAsync(expectedCommunity);

            // Act
            var community = await _mockCommunityService.GetCommunityAsync(communityId);

            // Assert
            Assert.NotNull(community);
            Assert.Equal(expectedCommunity.Id, community.Id);
        }
    }
}
