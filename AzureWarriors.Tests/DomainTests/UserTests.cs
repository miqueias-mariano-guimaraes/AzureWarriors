using System;
using Xunit;
using AzureWarriors.Domain.Entities;

namespace AzureWarriors.Tests.DomainTests
{
    public class UserTests
    {
        [Fact]
        public void User_Creation_Should_SetProperties()
        {
            // Arrange
            string username = "TestUser";

            // Act
            var user = new User(username);

            // Assert
            Assert.NotEqual(Guid.Empty, user.Id);
            Assert.Equal(username, user.Username);
            Assert.Equal(0, user.Points);
            Assert.Null(user.CommunityId);
            Assert.Null(user.ClanId);
        }
    }
}
