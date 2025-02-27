using AzureWarriors.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureWarriors.Application.Interfaces.Services
{
    public interface IInvitationService
    {
        Task<Invitation> InviteUserToClanAsync(Guid clanId, Guid leaderUserId, Guid userId);
        Task RespondInvitationAsync(Guid invitationId, bool accept);
    }
}
