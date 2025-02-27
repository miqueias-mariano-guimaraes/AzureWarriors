using System;
using System.Threading.Tasks;
using AzureWarriors.Application.Interfaces;
using AzureWarriors.Domain.Entities;

namespace AzureWarriors.Application.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICommunityRepository _communityRepository;
        private readonly IClanRepository _clanRepository;

        public UserService(IUserRepository userRepository, ICommunityRepository communityRepository, IClanRepository clanRepository)
        {
            _userRepository = userRepository;
            _communityRepository = communityRepository;
            _clanRepository = clanRepository;
        }

        public async Task<User> CreateUserAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Nome de usuário é obrigatório.");

            var existing = await _userRepository.GetByUsernameAsync(username);
            if (existing != null)
                throw new InvalidOperationException("Nome de usuário já está em uso.");

            var user = new User(username);

            await _userRepository.CreateAsync(user);
            return user;
        }

        public async Task JoinCommunityAsync(Guid userId, Guid communityId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new ArgumentException("Usuário não encontrado.");

            var community = await _communityRepository.GetByIdAsync(communityId);
            if (community == null)
                throw new ArgumentException("Comunidade não encontrada.");

            if (user.CommunityId.HasValue && user.CommunityId.Value != communityId)
            {
                user.Points = 0;
                user.ClanId = null;
            }

            user.CommunityId = communityId;
            await _userRepository.UpdateAsync(user);
        }

        public async Task JoinClanAsync(Guid userId, Guid clanId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new ArgumentException("Usuário não encontrado.");

            var clan = await _clanRepository.GetByIdAsync(clanId);
            if (clan == null)
                throw new ArgumentException("Clan não encontrada.");

            if (user.CommunityId != clan.CommunityId)
                throw new ArgumentException("Usuário não é da mesma Comunidade do Clan.");

            if (user.ClanId.HasValue && user.ClanId.Value != clanId)
            {
                user.Points = 0;
                user.ClanId = null;
            }

            user.ClanId = clanId;
            await _userRepository.UpdateAsync(user);
        }

        public async Task LeaveClanAsync(Guid userId, Guid clanId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new ArgumentException("Usuário não encontrado.");
            if (!user.ClanId.HasValue)
                throw new ArgumentException("Usuário não está associado a um Clan.");

            var clan = await _clanRepository.GetByIdAsync(clanId);
            if (clan == null)
                throw new ArgumentException("Clan não encontrado.");

            if (user.ClanId != clan.Id)
                throw new ArgumentException("Usuário não é desse Clan.");

            user.Points = 0;
            user.ClanId = null;

            await _userRepository.UpdateAsync(user);
        }
    }
}
