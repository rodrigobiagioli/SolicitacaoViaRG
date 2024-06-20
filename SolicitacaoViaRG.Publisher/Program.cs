using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using SolicitacaoViaRG.BLL.DTO;
using SolicitacaoViaRG.Data;
using SolicitacaoViaRG.Publisher;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddSingleton<RabbitMqPublisher>();
        services.AddSingleton(sp =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            return new RabbitMqConnection(configuration);
        });
    })
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();
    })
    .Build();

var publisher = host.Services.GetRequiredService<RabbitMqPublisher>();
await publisher.PublishProtocolsAsync("protocolos.json");
