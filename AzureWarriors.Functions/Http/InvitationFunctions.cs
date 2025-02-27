using System;
using System.Net;
using System.Threading.Tasks;
using AzureWarriors.Application.Interfaces.Services;
using AzureWarriors.Application.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace AzureWarriors.Functions.Http
{
    public class InvitationFunctions
    {
        private readonly IInvitationService _invitationService;

        public InvitationFunctions(IInvitationService invitationService)
        {
            _invitationService = invitationService;
        }

        [Function("InviteToClan")]
        public async Task<HttpResponseData> InviteToClanAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "InviteToClan")] HttpRequestData req)
        {
            var data = await req.ReadFromJsonAsync<InviteToClanRequest>();
            if (data == null)
            {
                var badReq = req.CreateResponse(HttpStatusCode.BadRequest);
                await badReq.WriteStringAsync("Invalid request body.");
                return badReq;
            }

            var response = req.CreateResponse();
            try
            {
                var invitation = await _invitationService.InviteUserToClanAsync(data.ClanId, data.LeaderUserId, data.UserId);
                response.StatusCode = HttpStatusCode.Created;
                await response.WriteAsJsonAsync(invitation);
            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                await response.WriteStringAsync(ex.Message);
            }

            return response;
        }

        [Function("RespondInvitation")]
        public async Task<HttpResponseData> RespondInvitationAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "RespondInvitation")] HttpRequestData req)
        {
            var data = await req.ReadFromJsonAsync<RespondInvitationRequest>();
            if (data == null)
            {
                var badReq = req.CreateResponse(HttpStatusCode.BadRequest);
                await badReq.WriteStringAsync("Invalid request body.");
                return badReq;
            }

            var response = req.CreateResponse();
            try
            {
                await _invitationService.RespondInvitationAsync(data.InvitationId, data.Accept);
                response.StatusCode = HttpStatusCode.OK;
                await response.WriteStringAsync("Invitation response updated.");
            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                await response.WriteStringAsync(ex.Message);
            }

            return response;
        }
    }

    public class InviteToClanRequest
    {
        public Guid ClanId { get; set; }
        public Guid LeaderUserId { get; set; }
        public Guid UserId { get; set; }
    }

    public class RespondInvitationRequest
    {
        public Guid InvitationId { get; set; }
        public bool Accept { get; set; }
    }
}
