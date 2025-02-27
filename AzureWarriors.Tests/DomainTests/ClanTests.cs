using System;
using Xunit;
using AzureWarriors.Domain.Entities;

namespace AzureWarriors.Tests.DomainTests
{
    public class ClanTests
    {
        [Fact]
        public void Clan_Creation_Should_SetProperties()
        {
            // Arrange
            Guid communityId = Guid.NewGuid();
            Guid leaderUserId = Guid.NewGuid();
            string clanName = "Test Clan";

            // Act
            var clan = new Clan(communityId, leaderUserId, clanName);

            // Assert
            Assert.NotEqual(Guid.Empty, clan.Id);
            Assert.Equal(communityId, clan.CommunityId);
            Assert.Equal(leaderUserId, clan.LeaderUserId);
            Assert.Equal(clanName, clan.Name);
            Assert.True(clan.CreatedAt <= DateTime.UtcNow);
        }
    }
}
