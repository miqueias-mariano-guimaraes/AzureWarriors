using AzureWarriors.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureWarriors.Application.Interfaces.Services
{
    public interface IClanService
    {
        Task<Clan> CreateClanAsync(Guid communityId, Guid leaderUserId, string clanName);
        Task<Clan> GetClanAsync(Guid clanId);
        Task KickMemberAsync(Guid clanId, Guid leaderUserId, Guid targetUserId);
    }
}
