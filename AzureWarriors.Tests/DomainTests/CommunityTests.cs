using System;
using Xunit;
using AzureWarriors.Domain.Entities;

namespace AzureWarriors.Tests.DomainTests
{
    public class CommunityTests
    {
        [Fact]
        public void Community_Creation_Should_SetProperties()
        {
            // Arrange
            string name = "Test Community";
            string description = "This is a test community.";

            // Act
            var community = new Community(name, description);

            // Assert
            Assert.NotEqual(Guid.Empty, community.Id);
            Assert.Equal(name, community.Name);
            Assert.Equal(description, community.Description);
            Assert.True(community.CreatedAt <= DateTime.UtcNow);
        }
    }
}
