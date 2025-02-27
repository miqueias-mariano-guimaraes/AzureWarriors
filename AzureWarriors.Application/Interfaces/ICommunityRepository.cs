using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AzureWarriors.Domain.Entities;

namespace AzureWarriors.Application.Interfaces
{
    public interface ICommunityRepository
    {
        Task CreateAsync(Community community);
        Task<Community?> GetByIdAsync(Guid id);
        Task<IEnumerable<Community>> GetAllAsync();
    }
}
