using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using SolicitacaoViaRG.Data;
using SolicitacaoViaRG.BLL;
using SolicitacaoViaRG.BLL.DTO;
using SolicitacaoViaRG.Data.Repository.Interface;
using SolicitacaoViaRG.Helper;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace SolicitacaoViaRG.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [ApiKeyAuth]
    public class ProtocoloController : ControllerBase
    {
        private readonly IProtocoloRepository _protocoloRepository;
        private readonly IConnection _rabbitMqConnection;
        private readonly IModel _channel;
        private readonly ILogger<ProtocoloController> _logger;
        private readonly ProtocoloBLL _protocoloBLL;

        public ProtocoloController(IProtocoloRepository protocoloRepository, ProtocoloBLL protocoloBLL, ILogger<ProtocoloController> logger, IConfiguration configuration)
        {
            _protocoloRepository = protocoloRepository;
            _protocoloBLL = protocoloBLL;
            _logger = logger;   
            try
            {
                _logger.LogInformation("Conectando com o RabbitMQ");
                _rabbitMqConnection = new RabbitMqConnection(configuration).Connection;
                _channel = _rabbitMqConnection.CreateModel();

                _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false); 
            }
            catch (BrokerUnreachableException ex)
            {
                _logger.LogError($"Não foi possível conectar ao RabbitMQ: {ex.Message}");
                throw;
            }
        }

                [HttpGet("buscar")]
        public async Task<IActionResult> BuscarProtocolos([FromQuery] string? numeroProtocolo, [FromQuery] string? cpf, [FromQuery] string? rg)
        {
            var protocolos = await _protocoloBLL.BuscarProtocolosAsync(numeroProtocolo, cpf, rg);
            return Ok(protocolos);
        }
    }
}
