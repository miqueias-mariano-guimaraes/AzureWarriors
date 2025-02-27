using System;
using System.Threading.Tasks;
using AzureWarriors.Application.DTOs;
using AzureWarriors.Domain.Entities;
using AzureWarriors.Application.Interfaces.Repositories;
using AzureWarriors.Application.Interfaces.Services;

namespace AzureWarriors.Application.Services
{
    public class CommunityService : ICommunityService
    {
        private readonly ICommunityRepository _communityRepository;

        public CommunityService(ICommunityRepository communityRepository)
        {
            _communityRepository = communityRepository;
        }

        public async Task<Community> CreateCommunityAsync(CreateCommunityDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Nome da comunidade é obrigatório.");

            var community = new Community(dto.Name, dto.Description);

            await _communityRepository.CreateAsync(community);
            return community;
        }

        public async Task<Community> GetCommunityAsync(Guid communityId)
        {
            return await _communityRepository.GetByIdAsync(communityId);
        }
    }
}
