using System;

namespace AzureWarriors.Domain.Entities
{
    public class Clan
    {
        public Guid Id { get; set; }

        public Guid CommunityId { get; set; }

        public Guid LeaderUserId { get; set; }

        public string Name { get; set; }

        public DateTime CreatedAt { get; set; }

        public Clan() { }

        public Clan(Guid communityId, Guid leaderUserId, string name)
        {
            Id = Guid.NewGuid();
            CommunityId = communityId;
            LeaderUserId = leaderUserId;
            Name = name;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
