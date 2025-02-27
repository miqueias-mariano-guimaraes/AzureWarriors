using System;
using Xunit;
using AzureWarriors.Domain.Entities;
using AzureWarriors.Domain.Enums;

namespace AzureWarriors.Tests.DomainTests
{
    public class InvitationTests
    {
        [Fact]
        public void Invitation_Creation_Should_SetProperties()
        {
            // Arrange
            Guid clanId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();

            // Act
            var invitation = new Invitation(clanId, userId);

            // Assert
            Assert.NotEqual(Guid.Empty, invitation.Id);
            Assert.Equal(clanId, invitation.ClanId);
            Assert.Equal(userId, invitation.UserId);
            Assert.Equal(InvitationStatus.Pending, invitation.Status);
            Assert.True(invitation.CreatedAt <= DateTime.UtcNow);
        }

        [Fact]
        public void Invitation_Accept_Should_Update_Status()
        {
            // Arrange
            var invitation = new Invitation(Guid.NewGuid(), Guid.NewGuid());

            // Act
            invitation.Accept();

            // Assert
            Assert.Equal(InvitationStatus.Accepted, invitation.Status);
        }

        [Fact]
        public void Invitation_Decline_Should_Update_Status()
        {
            // Arrange
            var invitation = new Invitation(Guid.NewGuid(), Guid.NewGuid());

            // Act
            invitation.Decline();

            // Assert
            Assert.Equal(InvitationStatus.Declined, invitation.Status);
        }
    }
}
