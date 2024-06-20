using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using SolicitacaoViaRG.Data;
using SolicitacaoViaRG.Data.Model;
using SolicitacaoViaRG.Data.Repository.Interface;

namespace SolicitacaoViaRG.Data.Repository
{
    public class ProtocoloRepository : IProtocoloRepository
    {
        private readonly IMongoCollection<Protocolo> _protocolos;

        public ProtocoloRepository(AppDbContext context)
        {
            _protocolos = context.Protocolo;
        }

        public async Task<IEnumerable<Protocolo>> BuscarProtocolosAsync(string? numeroProtocolo, string? cpf, string? rg)
        {
            var filterBuilder = Builders<Protocolo>.Filter;
            var filter = filterBuilder.Empty;

            if (!string.IsNullOrEmpty(numeroProtocolo))
            {
                filter &= filterBuilder.Eq(p => p.NumeroProtocolo, numeroProtocolo);
            }

            if (!string.IsNullOrEmpty(cpf))
            {
                filter &= filterBuilder.Eq(p => p.Cpf, cpf);
            }

            if (!string.IsNullOrEmpty(rg))
            {
                filter &= filterBuilder.Eq(p => p.Rg, rg);
            }

            return await _protocolos.Find(filter).ToListAsync();
        }

        public async Task AddProtocoloAsync(Protocolo protocolo)
        {
            await _protocolos.InsertOneAsync(protocolo);
        }

        public async Task<Protocolo> GetByNumeroProtocoloAsync(string numeroProtocolo)
        {
            return await _protocolos.Find(p => p.NumeroProtocolo == numeroProtocolo).FirstOrDefaultAsync();
        }

        public async Task<Protocolo> GetByCpfAsync(string cpf)
        {
            return await _protocolos.Find(p => p.Cpf == cpf).FirstOrDefaultAsync();
        }

        public async Task<Protocolo> GetByRgAsync(string rg)
        {
            return await _protocolos.Find(p => p.Rg == rg).FirstOrDefaultAsync();
        }
    }
}
