using MongoDB.Driver;
using SolicitacaoViaRG.Data.Model;
using SolicitacaoViaRG.Data.Repository.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SolicitacaoViaRG.Data.Repository
{
    public class ProtocoloRepository : IProtocoloRepository
    {
        private readonly IMongoCollection<Protocolo> _protocolos;

        public ProtocoloRepository(AppDbContext context)
        {
            _protocolos = context.Protocolo;
        }

        public async Task<IEnumerable<Protocolo>> GetAllAsync()
        {
            return await _protocolos.Find(p => true).ToListAsync();
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

        public async Task AddProtocoloAsync(Protocolo protocolo)
        {
            await _protocolos.InsertOneAsync(protocolo);
        }

        public async Task<IEnumerable<Protocolo>> BuscarProtocolosAsync(string? numeroProtocolo, string? cpf, string? rg)
        {
            var filter = Builders<Protocolo>.Filter.Empty;

            if (!string.IsNullOrEmpty(numeroProtocolo))
                filter &= Builders<Protocolo>.Filter.Eq(p => p.NumeroProtocolo, numeroProtocolo);

            if (!string.IsNullOrEmpty(cpf))
                filter &= Builders<Protocolo>.Filter.Eq(p => p.Cpf, cpf);

            if (!string.IsNullOrEmpty(rg))
                filter &= Builders<Protocolo>.Filter.Eq(p => p.Rg, rg);

            return await _protocolos.Find(filter).ToListAsync();
        }
    }
}
