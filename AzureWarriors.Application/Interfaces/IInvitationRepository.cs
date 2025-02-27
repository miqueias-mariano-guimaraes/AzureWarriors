using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AzureWarriors.Domain.Entities;
using AzureWarriors.Domain.Enums;

namespace AzureWarriors.Application.Interfaces
{
    public interface IInvitationRepository
    {
        Task CreateAsync(Invitation invitation);
        Task<Invitation> GetByIdAsync(Guid invitationId);
        Task<IEnumerable<Invitation>> GetPendingByClanAsync(Guid clanId);
        Task UpdateStatusAsync(Guid invitationId, InvitationStatus newStatus);
    }
}
