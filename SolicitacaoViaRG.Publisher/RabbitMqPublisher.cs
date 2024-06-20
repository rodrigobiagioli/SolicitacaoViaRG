
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using SolicitacaoViaRG.BLL.DTO;
using SolicitacaoViaRG.Data;

namespace SolicitacaoViaRG.Publisher
{

    public class RabbitMqPublisher
    {
        private readonly ILogger<RabbitMqPublisher> _logger;
        private readonly IConnection _rabbitMqConnection;
        private readonly IModel _channel;

        public RabbitMqPublisher(RabbitMqConnection rabbitMqConnection, ILogger<RabbitMqPublisher> logger)
        {
            _logger = logger;

            try
            {
                _logger.LogInformation("Conectando com o RabbitMQ");
                _rabbitMqConnection = rabbitMqConnection.Connection;
                _channel = _rabbitMqConnection.CreateModel();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao conectar com o RabbitMQ: {ex.Message}");
                throw;
            }
        }

        public async Task PublishProtocolsAsync(string filePath)
        {
            try
            {
                _logger.LogInformation("Lendo arquivo JSON: {FilePath}", filePath);
                var json = await File.ReadAllTextAsync(filePath);
                var protocolos = JsonSerializer.Deserialize<List<ProtocoloDto>>(json);

                if (protocolos == null || protocolos.Count == 0)
                {
                    _logger.LogWarning("Nenhum protocolo encontrado no arquivo JSON.");
                    return;
                }

                foreach (var protocolo in protocolos)
                {
                    var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(protocolo));
                    _channel.BasicPublish(exchange: "", routingKey: "ProtocoloQueue", basicProperties: null, body: body);
                    _logger.LogInformation("Protocolo enviado para a fila: {NumeroProtocolo}", protocolo.NumeroProtocolo);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao publicar protocolos: {ex.Message}");
            }
        }
    }

}