using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AzureWarriors.Domain.Entities;

namespace AzureWarriors.Application.Interfaces.Repositories
{
    public interface IClanRepository
    {
        Task CreateAsync(Clan clan);
        Task<Clan> GetByIdAsync(Guid id);
        Task<IEnumerable<Clan>> GetByCommunityAsync(Guid communityId);
    }
}
