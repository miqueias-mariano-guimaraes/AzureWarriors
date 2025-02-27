using System.Net;
using System.Threading.Tasks;
using AzureWarriors.Application.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System;
using AzureWarriors.Application.Interfaces.Services;

namespace AzureWarriors.Functions.Http
{
    public class UserFunctions
    {
        private readonly IUserService _userService;

        public UserFunctions(IUserService userService)
        {
            _userService = userService;
        }

        [Function("CreateUser")]
        public async Task<HttpResponseData> CreateUserAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "CreateUser")] HttpRequestData req)
        {
            var data = await req.ReadFromJsonAsync<CreateUserRequest>();
            if (data == null)
            {
                var badReq = req.CreateResponse(HttpStatusCode.BadRequest);
                await badReq.WriteStringAsync("Invalid request payload.");
                return badReq;
            }

            var response = req.CreateResponse();
            try
            {
                var user = await _userService.CreateUserAsync(data.Username);
                response.StatusCode = HttpStatusCode.Created;
                await response.WriteAsJsonAsync(user);
            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                await response.WriteStringAsync(ex.Message);
            }

            return response;
        }
    }

    public class CreateUserRequest
    {
        public string Username { get; set; }
    }
}
