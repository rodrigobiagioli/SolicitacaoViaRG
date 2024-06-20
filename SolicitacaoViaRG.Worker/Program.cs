using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SolicitacaoViaRG.Data;
using SolicitacaoViaRG.Data.Repository;
using SolicitacaoViaRG.Data.Repository.Interface;
using SolicitacaoViaRG.Services;
using SolicitacaoViaRG.BLL;
using Microsoft.Extensions.Configuration;

public class Program
{
    public static void Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        host.Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
            })
            .ConfigureServices((hostContext, services) =>
            {
                // Configuração de serviços
                var configuration = hostContext.Configuration;
                var imageDirectory = configuration["ImageDirectory"];
                
                services.AddMongoDbContext(configuration);
                services.AddSingleton<IProtocoloRepository, ProtocoloRepository>();
                services.AddSingleton(sp =>
                {
                    var logger = sp.GetRequiredService<ILogger<ProtocoloBLL>>();
                    return new ProtocoloBLL(sp.GetRequiredService<IProtocoloRepository>(), imageDirectory, logger);
                });
                services.AddHostedService<RabbitMqHostedService>();
            });
}
