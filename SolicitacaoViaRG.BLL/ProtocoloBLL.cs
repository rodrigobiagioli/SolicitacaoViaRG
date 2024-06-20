using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SolicitacaoViaRG.BLL.DTO;
using SolicitacaoViaRG.Data.Model;
using SolicitacaoViaRG.Data.Repository.Interface;
using SolicitacaoViaRG.Helper;

namespace SolicitacaoViaRG.BLL
{
    public class ProtocoloBLL
    {
        private readonly IProtocoloRepository _protocoloRepository;
        private readonly string _imageDirectory;
        private readonly ILogger<ProtocoloBLL> _logger;

        public ProtocoloBLL(IProtocoloRepository protocoloRepository, string imageDirectory, ILogger<ProtocoloBLL> logger)
        {
            _protocoloRepository = protocoloRepository;
            _imageDirectory = imageDirectory;
            _logger = logger;
        }

        public async Task<IEnumerable<ProtocoloDto>> BuscarProtocolosAsync(string? numeroProtocolo, string? cpf, string? rg)
        {
            _logger.LogInformation("Buscando protocolos com os parâmetros - NumeroProtocolo: {NumeroProtocolo}, CPF: {Cpf}, RG: {Rg}", numeroProtocolo, cpf, rg);
            
            var protocolos = await _protocoloRepository.BuscarProtocolosAsync(numeroProtocolo, cpf, rg);
            return protocolos.Select(MapToDto).ToList();
        }

        public async Task SalvarProtocoloAsync(ProtocoloDto protocoloDto)
        {
            _logger.LogInformation("Iniciando validação e aplicação das regras para o protocolo: {NumeroProtocolo}", protocoloDto.NumeroProtocolo);

            ValidarCamposObrigatorios(protocoloDto);
            await ValidarDuplicidade(protocoloDto);

            // Valida se a imagem é base64
            if (!ImageHelper.IsBase64Image(protocoloDto.Foto))
            {
                _logger.LogDebug("Foto inválida: {@foto}", protocoloDto.Foto);
                throw new Exception("A foto não está em um formato válido.");
            }

            // Salva a imagem no diretório informado
            var filePath = ImageHelper.SaveImage(protocoloDto.Foto, protocoloDto.Cpf, _imageDirectory);
            _logger.LogInformation("Imagem salva no diretório: {ImageDirectory}, Caminho: {FilePath}", _imageDirectory, filePath);

            // Cria o objeto Protocolo e salva no repositório
            var protocolo = MapToEntity(protocoloDto, filePath);
            await _protocoloRepository.AddProtocoloAsync(protocolo);
            _logger.LogInformation("Protocolo salvo no MongoDB: {NumeroProtocolo}", protocoloDto.NumeroProtocolo);
        }

        public async Task<ProtocoloDto> ObterPorNumeroProtocoloAsync(string numeroProtocolo)
        {
            _logger.LogInformation("Buscando protocolo por número: {NumeroProtocolo}", numeroProtocolo);
            var protocolo = await _protocoloRepository.GetByNumeroProtocoloAsync(numeroProtocolo);
            return protocolo != null ? MapToDto(protocolo) : null;
        }

        public async Task<ProtocoloDto> ObterPorCpfAsync(string cpf)
        {
            _logger.LogInformation("Buscando protocolo por CPF: {Cpf}", cpf);
            var protocolo = await _protocoloRepository.GetByCpfAsync(cpf);
            return protocolo != null ? MapToDto(protocolo) : null;
        }

        public async Task<ProtocoloDto> ObterPorRgAsync(string rg)
        {
            _logger.LogInformation("Buscando protocolo por RG: {Rg}", rg);
            var protocolo = await _protocoloRepository.GetByRgAsync(rg);
            return protocolo != null ? MapToDto(protocolo) : null;
        }

        private void ValidarCamposObrigatorios(ProtocoloDto protocoloDto)
        {
            if (string.IsNullOrWhiteSpace(protocoloDto.NumeroProtocolo))
            {
                _logger.LogWarning("Campo obrigatório ausente: NumeroProtocolo");
                throw new Exception("Campo obrigatório ausente: NumeroProtocolo");
            }

            if (string.IsNullOrWhiteSpace(protocoloDto.Cpf))
            {
                _logger.LogWarning("Campo obrigatório ausente: Cpf");
                throw new Exception("Campo obrigatório ausente: Cpf");
            }

            if (string.IsNullOrWhiteSpace(protocoloDto.Rg))
            {
                _logger.LogWarning("Campo obrigatório ausente: Rg");
                throw new Exception("Campo obrigatório ausente: Rg");
            }

            if (string.IsNullOrWhiteSpace(protocoloDto.Nome))
            {
                _logger.LogWarning("Campo obrigatório ausente: Nome");
                throw new Exception("Campo obrigatório ausente: Nome");
            }

            if (string.IsNullOrWhiteSpace(protocoloDto.Foto))
            {
                _logger.LogWarning("Campo obrigatório ausente: Foto");
                throw new Exception("Campo obrigatório ausente: Foto");
            }
        }

        private async Task ValidarDuplicidade(ProtocoloDto protocoloDto)
        {
            var protocoloExistente = await _protocoloRepository.GetByNumeroProtocoloAsync(protocoloDto.NumeroProtocolo);
            if (protocoloExistente != null)
            {
                _logger.LogDebug("Protocolo com o mesmo número já existe: {NumeroProtocolo}", protocoloDto.NumeroProtocolo);
                throw new Exception("Protocolo com o mesmo número já existe.");
            }

            var viaExistenteCpf = await _protocoloRepository.GetByCpfAsync(protocoloDto.Cpf);
            if (viaExistenteCpf != null && viaExistenteCpf.NumeroVia == protocoloDto.NumeroVia)
            {
                _logger.LogDebug("CPF com o mesmo número de via já existe: {Cpf}, Via: {NumeroVia}", protocoloDto.Cpf, protocoloDto.NumeroVia);
                throw new Exception("CPF com o mesmo número de via já existe.");
            }

            var viaExistenteRg = await _protocoloRepository.GetByRgAsync(protocoloDto.Rg);
            if (viaExistenteRg != null && viaExistenteRg.NumeroVia == protocoloDto.NumeroVia)
            {
                _logger.LogDebug("RG com o mesmo número de via já existe: {Rg}, Via: {NumeroVia}", protocoloDto.Rg, protocoloDto.NumeroVia);
                throw new Exception("RG com o mesmo número de via já existe.");
            }
        }

        private ProtocoloDto MapToDto(Protocolo protocolo)
        {
            return new ProtocoloDto
            {
                Nome = protocolo.Nome,
                Rg = protocolo.Rg,
                Cpf = protocolo.Cpf,
                Foto = protocolo.Foto,
                NumeroProtocolo = protocolo.NumeroProtocolo,
                NumeroVia = protocolo.NumeroVia,
                NomeMae = protocolo.NomeMae,
                NomePai = protocolo.NomePai
            };
        }

        private Protocolo MapToEntity(ProtocoloDto protocoloDto, string filePath)
        {
            return new Protocolo
            {
                Nome = protocoloDto.Nome,
                Rg = protocoloDto.Rg,
                Cpf = protocoloDto.Cpf,
                Foto = filePath,
                NumeroProtocolo = protocoloDto.NumeroProtocolo,
                NumeroVia = protocoloDto.NumeroVia,
                NomeMae = protocoloDto.NomeMae,
                NomePai = protocoloDto.NomePai
            };
        }
    }
}
