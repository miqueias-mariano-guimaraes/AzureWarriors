using System;
using AzureWarriors.Domain.Enums;

namespace AzureWarriors.Domain.Entities
{
    public class Invitation
    {
        public Guid Id { get; set; }
        public Guid ClanId { get; set; }
        public Guid UserId { get; set; }
        public InvitationStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public Invitation() { }

        public Invitation(Guid clanId, Guid userId)
        {
            Id = Guid.NewGuid();
            ClanId = clanId;
            UserId = userId;
            Status = InvitationStatus.Pending;
            CreatedAt = DateTime.UtcNow;
        }

        public void Accept()
        {
            Status = InvitationStatus.Accepted;
        }

        public void Decline()
        {
            Status = InvitationStatus.Declined;
        }
    }
}
