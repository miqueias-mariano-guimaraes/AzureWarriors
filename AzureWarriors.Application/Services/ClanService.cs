using System;
using System.Threading.Tasks;
using AzureWarriors.Application.Interfaces.Repositories;
using AzureWarriors.Application.Interfaces.Services;
using AzureWarriors.Domain.Entities;

namespace AzureWarriors.Application.Services
{
    public class ClanService : IClanService
    {
        private readonly IClanRepository _clanRepository;
        private readonly ICommunityRepository _communityRepository;
        private readonly IUserRepository _userRepository;

        public ClanService(
            IClanRepository clanRepository,
            ICommunityRepository communityRepository,
            IUserRepository userRepository)
        {
            _clanRepository = clanRepository;
            _communityRepository = communityRepository;
            _userRepository = userRepository;
        }

        public async Task<Clan> CreateClanAsync(Guid communityId, Guid leaderUserId, string clanName)
        {
            var community = await _communityRepository.GetByIdAsync(communityId);
            if (community == null)
                throw new ArgumentException("Comunidade não encontrada.");

            var leaderUser = await _userRepository.GetByIdAsync(leaderUserId);
            if (leaderUser == null)
                throw new ArgumentException("Usuário líder não encontrado.");

            if (leaderUser.CommunityId != communityId)
                throw new InvalidOperationException("Usuário líder não pertence à mesma comunidade.");

            var clan = new Clan(communityId, leaderUserId, clanName);


            await _clanRepository.CreateAsync(clan);
            leaderUser.ClanId = clan.Id;
            await _userRepository.UpdateAsync(leaderUser);
            return clan;
        }

        public async Task<Clan> GetClanAsync(Guid clanId)
        {
            return await _clanRepository.GetByIdAsync(clanId);
        }

        public async Task KickMemberAsync(Guid clanId, Guid leaderUserId, Guid targetUserId)
        {
            var targetUser = await _userRepository.GetByIdAsync(targetUserId);
            if (targetUser == null)
                throw new ArgumentException("Usuário selecionado não encontrado.");
            if (!targetUser.ClanId.HasValue)
                throw new ArgumentException("Usuário selecionado não está associado a um Clan.");

            var leaderUser = await _userRepository.GetByIdAsync(leaderUserId);
            if (leaderUser == null)
                throw new ArgumentException("Usuário \"Lider do Clan\" não encontrado.");
            if (!leaderUser.ClanId.HasValue)
                throw new ArgumentException("Usuário \"Lider do Clan\" não está associado a um Clan.");

            var clan = await _clanRepository.GetByIdAsync(clanId);
            if (clan == null)
                throw new ArgumentException("Clan não encontrado.");

            if (leaderUser.Id != clan.LeaderUserId)
                throw new ArgumentException("Usuário \"Lider do Clan\" não é lider desse Clan.");

            if (targetUser.ClanId != clan.Id)
                throw new ArgumentException("Usuário selecionado não é desse Clan.");


            targetUser.Points = 0;
            targetUser.ClanId = null;


            await _userRepository.UpdateAsync(targetUser);
        }
    }
}
