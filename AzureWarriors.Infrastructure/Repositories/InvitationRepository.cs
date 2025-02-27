using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data;
using Dapper;
using AzureWarriors.Domain.Entities;
using AzureWarriors.Infrastructure.Data;
using AzureWarriors.Domain.Enums;
using AzureWarriors.Application.Interfaces.Repositories;

namespace AzureWarriors.Infrastructure.Repositories
{
    public class InvitationRepository : IInvitationRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public InvitationRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task CreateAsync(Invitation invitation)
        {
            using var conn = _connectionFactory.CreateConnection();
            var sql = @"
                INSERT INTO Invitations
                (Id, ClanId, UserId, Status, CreatedAt)
                VALUES (@Id, @ClanId, @UserId, @Status, @CreatedAt);
            ";
            await conn.ExecuteAsync(sql, invitation);
        }

        public async Task<Invitation> GetByIdAsync(Guid invitationId)
        {
            using var conn = _connectionFactory.CreateConnection();
            var sql = @"
                SELECT Id, ClanId, UserId, Status, CreatedAt
                FROM Invitations
                WHERE Id = @invitationId;
            ";
            return await conn.QuerySingleOrDefaultAsync<Invitation>(sql, new { invitationId });
        }

        public async Task<IEnumerable<Invitation>> GetPendingByClanAsync(Guid clanId)
        {
            using var conn = _connectionFactory.CreateConnection();
            var sql = @"
                SELECT Id, ClanId, UserId, Status, CreatedAt
                FROM Invitations
                WHERE ClanId = @clanId
                  AND Status = @status;
            ";
            return await conn.QueryAsync<Invitation>(sql, new
            {
                clanId,
                status = InvitationStatus.Pending
            });
        }

        public async Task UpdateStatusAsync(Guid invitationId, InvitationStatus newStatus)
        {
            using var conn = _connectionFactory.CreateConnection();
            var sql = @"
                UPDATE Invitations
                SET Status = @newStatus
                WHERE Id = @invitationId;
            ";
            await conn.ExecuteAsync(sql, new { invitationId, newStatus });
        }

    }
}
