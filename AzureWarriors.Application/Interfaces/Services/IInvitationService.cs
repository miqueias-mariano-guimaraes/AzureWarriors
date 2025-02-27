using AzureWarriors.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureWarriors.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<User> CreateUserAsync(string username);
        Task JoinCommunityAsync(Guid userId, Guid communityId);
        Task JoinClanAsync(Guid userId, Guid clanId);
        Task LeaveClanAsync(Guid userId, Guid clanId);
    }
}
