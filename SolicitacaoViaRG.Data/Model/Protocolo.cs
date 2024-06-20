using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SolicitacaoViaRG.Data.Model
{
    public class Protocolo
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        
        [BsonElement("NumeroProtocolo")]
        public string NumeroProtocolo { get; set; }

        [BsonElement("NumeroVia")]
        public int NumeroVia { get; set; }

        [BsonElement("Nome")]
        public string Nome { get; set; }
        
        [BsonElement("Rg")]
        public string Rg { get; set; }
        
        [BsonElement("Cpf")]
        public string Cpf { get; set; }

        [BsonElement("Foto")]
        public string Foto { get; set; }

        [BsonElement("NomeMae")]
        public string NomeMae { get; set; }

        [BsonElement("NomePai")]
        public string NomePai { get; set; }

    }
}
