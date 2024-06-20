using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SolicitacaoViaRG.Data.Model;
using MongoDB.Driver;

namespace SolicitacaoViaRG.Data
{
    public class AppDbContext
    {
        private readonly IMongoDatabase _database;

        public AppDbContext(string connectionString, string databaseName)
        {
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }

        public IMongoCollection<Protocolo> Protocolo => _database.GetCollection<Protocolo>("Protocolo");

    }
    
}

