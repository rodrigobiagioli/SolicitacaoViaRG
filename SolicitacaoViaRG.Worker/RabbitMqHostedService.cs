using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System.Threading;
using System.Threading.Tasks;
using SolicitacaoViaRG.Data;
using SolicitacaoViaRG.BLL.DTO;
using SolicitacaoViaRG.BLL;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace SolicitacaoViaRG.Services
{
    public class RabbitMqHostedService : BackgroundService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly ILogger<RabbitMqHostedService> _logger;
        private readonly ProtocoloBLL _protocoloBLL;

        public RabbitMqHostedService(ProtocoloBLL protocoloBLL, ILogger<RabbitMqHostedService> logger, IConfiguration configuration)
        {
            _protocoloBLL = protocoloBLL;
            _logger = logger;

            try
            {
                _logger.LogInformation("Conectando com o RabbitMQ");
                _connection = new RabbitMqConnection(configuration).Connection;

                _channel = _connection.CreateModel();

                _logger.LogInformation("Criando as filas RabbitMQ");

                // Declara a fila DLQ
                _channel.QueueDeclare(
                    queue: "ProtocoloQueue-DLQ",
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );

                // Declara a fila principal e configura a DLQ
                _channel.QueueDeclare(
                    queue: "ProtocoloQueue",
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: new Dictionary<string, object>
                    {
                        { "x-dead-letter-exchange", "" }, // Use o exchange padrão
                        { "x-dead-letter-routing-key", "ProtocoloQueue-DLQ" }
                    }
                );

                _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false); 
            }
            catch (BrokerUnreachableException ex)
            {
                _logger.LogError($"Não foi possível conectar ao RabbitMQ: {ex.Message}");
                throw;
            }
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            _logger.LogInformation("Iniciando leitura na fila MQ");

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                _logger.LogInformation("Nova mensagem recebida na fila MQ");
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                try
                {
                    _logger.LogDebug("Deserializa a mensagem: {@mensagem}", message);
                    var protocoloDto = JsonSerializer.Deserialize<ProtocoloDto>(message);

                    if (protocoloDto != null)
                    {
                        _logger.LogInformation("Nova mensagem recebida na fila MQ");
                        await _protocoloBLL.SalvarProtocoloAsync(protocoloDto);
                        _channel.BasicAck(ea.DeliveryTag, false);
                        _logger.LogInformation("Protocolo gravado no MongoDB: {@ProtocoloDto}", protocoloDto.NumeroProtocolo);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("Erro ao tentar gravar os dados da fila MQ: {@erro}", ex.Message);
                    _logger.LogError("Dados da mensagem: {@mensagem}", message);

                    _logger.LogInformation("Enviando para a fila DLQ");
                    _channel.BasicNack(ea.DeliveryTag, false, false);  
                }
            };

            _channel.BasicConsume(queue: "ProtocoloQueue", autoAck: false, consumer: consumer);

            return Task.CompletedTask;
        }
    }
}
