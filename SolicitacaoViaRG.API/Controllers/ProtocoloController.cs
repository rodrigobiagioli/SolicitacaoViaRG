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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace SolicitacaoViaRG.API.Controllers
{
    /// <summary>
    /// Controlador para gerenciamento de protocolos.
    /// </summary>
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

        /// <summary>
        /// Busca protocolos com base nos parâmetros fornecidos.
        /// </summary>
        /// <param name="numeroProtocolo">Número do protocolo (opcional).</param>
        /// <param name="cpf">CPF do solicitante (opcional).</param>
        /// <param name="rg">RG do solicitante (opcional).</param>
        /// <returns>Retorna uma lista de protocolos que correspondem aos critérios de busca.</returns>
        /// <remarks>
        /// Exemplos de solicitações:
        ///
        ///     GET /protocolo/buscar?numeroProtocolo=12345
        ///     GET /protocolo/buscar?cpf=12345678901
        ///     GET /protocolo/buscar?rg=1234567
        ///
        /// </remarks>
        [HttpGet("buscar")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> BuscarProtocolos([FromQuery] string? numeroProtocolo, [FromQuery] string? cpf, [FromQuery] string? rg)
        {
            _logger.LogInformation("Iniciando busca de protocolos.");
            
            if (string.IsNullOrEmpty(numeroProtocolo) && string.IsNullOrEmpty(cpf) && string.IsNullOrEmpty(rg))
            {
                _logger.LogWarning("Nenhum parâmetro de busca fornecido.");
                return BadRequest("Pelo menos um parâmetro de busca deve ser fornecido.");
            }

            var protocolos = await _protocoloBLL.BuscarProtocolosAsync(numeroProtocolo, cpf, rg);
            _logger.LogInformation("Busca de protocolos concluída com sucesso.");

            return Ok(protocolos);
        }
    }
}
