using System;

namespace AzureWarriors.Domain.Entities
{
    public class Community
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime CreatedAt { get; set; }

        public Community() { }

        public Community(string name, string description)
        {
            Id = Guid.NewGuid();
            Name = name;
            Description = description;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
