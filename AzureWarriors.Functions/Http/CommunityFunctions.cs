using System.Net;
using System.Threading.Tasks;
using AzureWarriors.Application.DTOs;
using AzureWarriors.Application.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System;
using AzureWarriors.Application.Interfaces.Services;

namespace AzureWarriors.Functions.Http
{
    public class CommunityFunctions
    {
        private readonly ICommunityService _communityService;
        private readonly IUserService _userService;

        public CommunityFunctions(ICommunityService communityService, IUserService userService)
        {
            _communityService = communityService;
            _userService = userService;
        }

        [Function("CreateCommunity")]
        public async Task<HttpResponseData> CreateCommunityAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "CreateCommunity")] HttpRequestData req)
        {
            var dto = await req.ReadFromJsonAsync<CreateCommunityDto>();
            if (dto == null)
            {
                var badReq = req.CreateResponse(HttpStatusCode.BadRequest);
                await badReq.WriteStringAsync("Invalid payload.");
                return badReq;
            }

            try
            {
                var community = await _communityService.CreateCommunityAsync(dto);

                var response = req.CreateResponse(HttpStatusCode.Created);
                await response.WriteAsJsonAsync(community);
                return response;
            }
            catch (Exception ex)
            {
                var error = req.CreateResponse(HttpStatusCode.BadRequest);
                await error.WriteStringAsync(ex.Message);
                return error;
            }
        }

        [Function("GetCommunity")]
        public async Task<HttpResponseData> GetCommunityAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "GetCommunity/{communityId:guid}")] HttpRequestData req,
            Guid communityId)
        {
            var community = await _communityService.GetCommunityAsync(communityId);
            if (community == null)
            {
                var notFound = req.CreateResponse(HttpStatusCode.NotFound);
                await notFound.WriteStringAsync($"Community {communityId} not found.");
                return notFound;
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(community);
            return response;
        }

        [Function("JoinCommunity")]
        public async Task<HttpResponseData> JoinCommunityAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "JoinCommunity")] HttpRequestData req)
        {
            var data = await req.ReadFromJsonAsync<JoinCommunityRequest>();
            if (data == null)
            {
                var badReq = req.CreateResponse(HttpStatusCode.BadRequest);
                await badReq.WriteStringAsync("Invalid request.");
                return badReq;
            }

            var response = req.CreateResponse();

            try
            {
                await _userService.JoinCommunityAsync(data.UserId, data.CommunityId);

                response.StatusCode = HttpStatusCode.OK;
                await response.WriteStringAsync("User joined the community successfully!");
            }
            catch (Exception ex)
            {
                response = req.CreateResponse(HttpStatusCode.BadRequest);
                await response.WriteStringAsync(ex.Message);
            }

            return response;
        }
    }

    public class JoinCommunityRequest
    {
        public Guid UserId { get; set; }
        public Guid CommunityId { get; set; }
    }
}
