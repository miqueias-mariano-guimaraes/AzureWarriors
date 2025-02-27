using System;
using System.Threading.Tasks;
using AzureWarriors.Domain.Entities;

namespace AzureWarriors.Application.Interfaces
{
    public interface IUserRepository
    {
        Task CreateAsync(User user);
        Task<User> GetByIdAsync(Guid userId);
        Task<User> GetByUsernameAsync(string username);
        Task UpdateAsync(User user);
    }
}
