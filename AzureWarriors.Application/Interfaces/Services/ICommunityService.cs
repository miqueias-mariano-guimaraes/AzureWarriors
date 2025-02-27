using AzureWarriors.Application.DTOs;
using AzureWarriors.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureWarriors.Application.Interfaces.Services
{
    public interface ICommunityService
    {
        Task<Community> CreateCommunityAsync(CreateCommunityDto dto);
        Task<Community> GetCommunityAsync(Guid communityId);
    }
}
