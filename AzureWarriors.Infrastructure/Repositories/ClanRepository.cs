using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data;
using Dapper;
using AzureWarriors.Domain.Entities;
using AzureWarriors.Infrastructure.Data;
using AzureWarriors.Application.Interfaces.Repositories;

namespace AzureWarriors.Infrastructure.Repositories
{
    public class ClanRepository : IClanRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ClanRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task CreateAsync(Clan clan)
        {
            using var conn = _connectionFactory.CreateConnection();
            var sql = @"
                INSERT INTO Clans (Id, CommunityId, LeaderUserId, Name, CreatedAt)
                VALUES (@Id, @CommunityId, @LeaderUserId, @Name, @CreatedAt);
            ";
            await conn.ExecuteAsync(sql, clan);
        }

        public async Task<Clan> GetByIdAsync(Guid id)
        {
            using var conn = _connectionFactory.CreateConnection();
            var sql = @"
                SELECT Id, CommunityId, LeaderUserId, Name, CreatedAt
                FROM Clans
                WHERE Id = @id;
            ";
            return await conn.QuerySingleOrDefaultAsync<Clan>(sql, new { id });
        }

        public async Task<IEnumerable<Clan>> GetByCommunityAsync(Guid communityId)
        {
            using var conn = _connectionFactory.CreateConnection();
            var sql = @"
                SELECT Id, CommunityId, LeaderUserId, Name, CreatedAt
                FROM Clans
                WHERE CommunityId = @communityId;
            ";
            return await conn.QueryAsync<Clan>(sql, new { communityId });
        }

    }
}
