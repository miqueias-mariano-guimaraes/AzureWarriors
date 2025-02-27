using System;

namespace AzureWarriors.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }

        public string Username { get; set; }

        public Guid? CommunityId { get; set; }

        public Guid? ClanId { get; set; }

        public int Points { get; set; }

        public User() { }

        public User(string username)
        {
            Id = Guid.NewGuid();
            Username = username;
            Points = 0;
        }
    }
}
