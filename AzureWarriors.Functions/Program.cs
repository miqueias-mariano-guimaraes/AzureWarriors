using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AzureWarriors.Application.Interfaces;
using AzureWarriors.Application.Services;
using AzureWarriors.Infrastructure.Data;
using AzureWarriors.Infrastructure.Repositories;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

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
                    // 1) Injetar a fábrica de conexão (opcional) ou IDbConnection diretamente
                    services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();

                    // Caso prefira injetar IDbConnection diretamente:
                    // services.AddScoped<IDbConnection>(provider =>
                    // {
                    //     var configuration = provider.GetRequiredService<IConfiguration>();
                    //     var connString = configuration.GetConnectionString("SqlConnection");
                    //     return new SqlConnection(connString);
                    // });

                    // 2) Registrar repositórios (Infrastructure)

                    services.AddScoped<ICommunityRepository, CommunityRepository>();
                    services.AddScoped<IClanRepository, ClanRepository>();
                    services.AddScoped<IUserRepository, UserRepository>();
                    services.AddScoped<IInvitationRepository, InvitationRepository>();


                    // 3) Registrar serviços (Application)
                    services.AddScoped<CommunityService>();
                    services.AddScoped<ClanService>();
                    services.AddScoped<UserService>();
                    services.AddScoped<InvitationService>();
                })
                .Build();

            host.Run();
        }
    }
}
