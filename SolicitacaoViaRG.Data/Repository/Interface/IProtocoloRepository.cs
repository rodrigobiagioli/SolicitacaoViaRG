using System.Collections.Generic;
using System.Threading.Tasks;
using SolicitacaoViaRG.Data.Model;

namespace SolicitacaoViaRG.Data.Repository.Interface
{
    public interface IProtocoloRepository
    {
        Task<IEnumerable<Protocolo>> GetAllAsync();
        Task<Protocolo> GetByNumeroProtocoloAsync(string numeroProtocolo);
        Task<Protocolo> GetByCpfAsync(string cpf);
        Task<Protocolo> GetByRgAsync(string rg);
        Task AddProtocoloAsync(Protocolo protocolo);
        Task<IEnumerable<Protocolo>> BuscarProtocolosAsync(string? numeroProtocolo, string? cpf, string? rg);
    }
}
