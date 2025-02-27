using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using Dapper;
using AzureWarriors.Domain.Entities;
using AzureWarriors.Infrastructure.Data;
using AzureWarriors.Application.Interfaces.Repositories;

namespace AzureWarriors.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public UserRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task CreateAsync(User user)
        {
            using var conn = _connectionFactory.CreateConnection();
            var sql = @"
                INSERT INTO Users (Id, Username, CommunityId, ClanId, Points)
                VALUES (@Id, @Username, @CommunityId, @ClanId, @Points);
            ";
            await conn.ExecuteAsync(sql, user);
        }

        public async Task<User> GetByIdAsync(Guid userId)
        {
            using var conn = _connectionFactory.CreateConnection();
            var sql = @"
                SELECT Id, Username, CommunityId, ClanId, Points
                FROM Users
                WHERE Id = @userId;
            ";
            return await conn.QuerySingleOrDefaultAsync<User>(sql, new { userId });
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            using var conn = _connectionFactory.CreateConnection();
            var sql = @"
                SELECT Id, Username, CommunityId, ClanId, Points
                FROM Users
                WHERE Username = @username;
            ";
            return await conn.QuerySingleOrDefaultAsync<User>(sql, new { username });
        }

        public async Task UpdateAsync(User user)
        {
            using var conn = _connectionFactory.CreateConnection();
            var sql = @"
                UPDATE Users
                SET Username = @Username,
                    CommunityId = @CommunityId,
                    ClanId = @ClanId,
                    Points = @Points
                WHERE Id = @Id;
            ";
            await conn.ExecuteAsync(sql, user);
        }

    }
}
