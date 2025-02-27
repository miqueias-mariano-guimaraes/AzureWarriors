using System;
using System.Threading.Tasks;
using AzureWarriors.Application.Interfaces.Repositories;
using AzureWarriors.Application.Interfaces.Services;
using AzureWarriors.Domain.Entities;
using AzureWarriors.Domain.Enums;

namespace AzureWarriors.Application.Services
{
    public class InvitationService : IInvitationService
    {
        private readonly IInvitationRepository _invitationRepository;
        private readonly IClanRepository _clanRepository;
        private readonly IUserRepository _userRepository;

        public InvitationService(
            IInvitationRepository invitationRepository,
            IClanRepository clanRepository,
            IUserRepository userRepository)
        {
            _invitationRepository = invitationRepository;
            _clanRepository = clanRepository;
            _userRepository = userRepository;
        }

        public async Task<Invitation> InviteUserToClanAsync(Guid clanId, Guid leaderUserId, Guid userId)
        {
            var clan = await _clanRepository.GetByIdAsync(clanId);
            if (clan == null)
                throw new ArgumentException("Clan não encontrado.");

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new ArgumentException("Usuário selecionado não encontrado.");

            var leaderUser = await _userRepository.GetByIdAsync(leaderUserId);
            if (leaderUser == null)
                throw new ArgumentException("Usuário \"Lider do Clan\" não encontrado.");

            if (leaderUser.Id != clan.LeaderUserId)
                throw new ArgumentException("Usuário \"Lider do Clan\" não é lider desse Clan.");

            if (user.CommunityId != clan.CommunityId)
                throw new InvalidOperationException("Usuário não pertence à mesma comunidade do Clan.");

            var invitation = new Invitation(clanId, userId);
            await _invitationRepository.CreateAsync(invitation);
            return invitation;
        }

        public async Task RespondInvitationAsync(Guid invitationId, bool accept)
        {
            var invitation = await _invitationRepository.GetByIdAsync(invitationId);
            if (invitation == null)
                throw new ArgumentException("Convite não encontrado.");

            if (accept)
                invitation.Accept();
            else
                invitation.Decline();

            await _invitationRepository.UpdateStatusAsync(invitationId, invitation.Status);

            if(invitation.Status.Equals(InvitationStatus.Accepted))
            {
                var user = await _userRepository.GetByIdAsync(invitation.UserId);

                user.Points = 0;
                user.ClanId = invitation.ClanId;

                await _userRepository.UpdateAsync(user);
            }
        }
    }
}
