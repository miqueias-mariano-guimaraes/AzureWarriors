using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data;
using Dapper;
using AzureWarriors.Domain.Entities;
using AzureWarriors.Application.Interfaces; // <-- Interfaces definidas na Application
using AzureWarriors.Infrastructure.Data;

namespace AzureWarriors.Infrastructure.Repositories
{
    public class CommunityRepository : ICommunityRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public CommunityRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task CreateAsync(Community community)
        {
            using var conn = _connectionFactory.CreateConnection();
            var sql = @"
                INSERT INTO Communities (Id, Name, Description, CreatedAt)
                VALUES (@Id, @Name, @Description, @CreatedAt);
            ";
            await conn.ExecuteAsync(sql, community);
        }

        public async Task<Community?> GetByIdAsync(Guid id)
        {
            using var conn = _connectionFactory.CreateConnection();
            var sql = @"
                SELECT Id, Name, Description, CreatedAt
                FROM Communities
                WHERE Id = @id;
            ";
            return await conn.QueryFirstOrDefaultAsync<Community>(sql, new { id });
        }

        public async Task<IEnumerable<Community>> GetAllAsync()
        {
            using var conn = _connectionFactory.CreateConnection();
            var sql = @"
                SELECT Id, Name, Description, CreatedAt
                FROM Communities;
            ";
            return await conn.QueryAsync<Community>(sql);
        }
    }
}
