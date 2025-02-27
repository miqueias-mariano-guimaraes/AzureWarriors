using System;
using System.Net;
using System.Threading.Tasks;
using AzureWarriors.Application.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace AzureWarriors.Functions.Http
{
    public class ClanFunctions
    {
        private readonly ClanService _clanService;
        private readonly UserService _userService;

        public ClanFunctions(ClanService clanService, UserService userService)
        {
            _clanService = clanService;
            _userService = userService;
        }

        [Function("CreateClan")]
        public async Task<HttpResponseData> CreateClanAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "CreateClan")] HttpRequestData req)
        {
            var data = await req.ReadFromJsonAsync<CreateClanRequest>();
            if (data == null)
            {
                var badReq = req.CreateResponse(HttpStatusCode.BadRequest);
                await badReq.WriteStringAsync("Invalid request body.");
                return badReq;
            }

            var response = req.CreateResponse();
            try
            {
                var clan = await _clanService.CreateClanAsync(
                    data.CommunityId,
                    data.LeaderUserId,
                    data.ClanName
                );
                response.StatusCode = HttpStatusCode.Created;
                await response.WriteAsJsonAsync(clan);
            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                await response.WriteStringAsync(ex.Message);
            }

            return response;
        }

        [Function("GetClan")]
        public async Task<HttpResponseData> GetClanAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "GetClan/{clanId:guid}")] HttpRequestData req,
            Guid clanId)
        {
            var clan = await _clanService.GetClanAsync(clanId);
            if (clan == null)
            {
                var notFound = req.CreateResponse(HttpStatusCode.NotFound);
                await notFound.WriteStringAsync($"Clan {clanId} not found.");
                return notFound;
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(clan);
            return response;
        }

        [Function("JoinClan")]
        public async Task<HttpResponseData> JoinClanAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "JoinClan")] HttpRequestData req)
        {
            var data = await req.ReadFromJsonAsync<JoinClanRequest>();
            if (data == null)
            {
                var badReq = req.CreateResponse(HttpStatusCode.BadRequest);
                await badReq.WriteStringAsync("Invalid request body.");
                return badReq;
            }

            var response = req.CreateResponse();
            try
            {
                await _userService.JoinClanAsync(data.UserId, data.ClanId);

                response.StatusCode = HttpStatusCode.OK;
                await response.WriteStringAsync("User joined the clan successfully.");
            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                await response.WriteStringAsync(ex.Message);
            }

            return response;
        }

        [Function("LeaveClan")]
        public async Task<HttpResponseData> LeaveClanAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "LeaveClan")] HttpRequestData req)
        {
            var data = await req.ReadFromJsonAsync<LeaveClanRequest>();
            if (data == null)
            {
                var badReq = req.CreateResponse(HttpStatusCode.BadRequest);
                await badReq.WriteStringAsync("Invalid request body.");
                return badReq;
            }

            var response = req.CreateResponse();
            try
            {
                await _userService.LeaveClanAsync(data.UserId, data.ClanId);

                response.StatusCode = HttpStatusCode.OK;
                await response.WriteStringAsync("User left the clan successfully.");
            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                await response.WriteStringAsync(ex.Message);
            }

            return response;
        }

        [Function("KickMember")]
        public async Task<HttpResponseData> KickMemberAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "KickMember")] HttpRequestData req)
        {
            var data = await req.ReadFromJsonAsync<KickMemberRequest>();
            if (data == null)
            {
                var badReq = req.CreateResponse(HttpStatusCode.BadRequest);
                await badReq.WriteStringAsync("Invalid request body.");
                return badReq;
            }

            var response = req.CreateResponse();
            try
            {
                await _clanService.KickMemberAsync(data.ClanId, data.LeaderUserId, data.TargetUserId);

                response.StatusCode = HttpStatusCode.OK;
                await response.WriteStringAsync("Member kicked from clan.");
            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                await response.WriteStringAsync(ex.Message);
            }

            return response;
        }
    }

    public class CreateClanRequest
    {
        public Guid CommunityId { get; set; }
        public Guid LeaderUserId { get; set; }
        public string ClanName { get; set; }
    }

    public class JoinClanRequest
    {
        public Guid UserId { get; set; }
        public Guid ClanId { get; set; }
    }

    public class LeaveClanRequest
    {
        public Guid UserId { get; set; }
        public Guid ClanId { get; set; }
    }

    public class KickMemberRequest
    {
        public Guid ClanId { get; set; }
        public Guid LeaderUserId { get; set; }
        public Guid TargetUserId { get; set; }
    }
}
