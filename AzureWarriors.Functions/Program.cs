using AzureWarriors.Application.Interfaces.Repositories;
using AzureWarriors.Application.Interfaces.Services;
using AzureWarriors.Application.Services;
using AzureWarriors.Infrastructure.Data;
using AzureWarriors.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json.Serialization;
using System.Text.Json;
using AzureWarriors.Functions.Configurations;

namespace AzureWarriors.Functions
{
    public class Program
    {
        public static void Main()
        {
            var host = new HostBuilder()
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                          .AddEnvironmentVariables();
                })
                .ConfigureFunctionsWebApplication()
                .ConfigureServices(services =>
                {
                    // Conexão
                    services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();

                    // Registra os repositórios.
                    services.AddScoped<ICommunityRepository, CommunityRepository>();
                    services.AddScoped<IClanRepository, ClanRepository>();
                    services.AddScoped<IUserRepository, UserRepository>();
                    services.AddScoped<IInvitationRepository, InvitationRepository>();

                    // Registra os serviços
                    services.AddScoped<ICommunityService, CommunityService>();
                    services.AddScoped<IClanService, ClanService>();
                    services.AddScoped<IUserService, UserService>();
                    services.AddScoped<IInvitationService, InvitationService>();

                }).ConfigureFunctionsWorkerDefaults(worker =>
                {
                    // Registra o Middleware de Serialização customizada
                    worker.UseMiddleware<CustomSerializationMiddleware>();
                })
                .Build();

            host.Run();
        }
    }
}
