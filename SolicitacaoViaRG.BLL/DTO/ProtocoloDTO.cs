namespace SolicitacaoViaRG.BLL.DTO
{
    public class ProtocoloDto
    {
        public string NumeroProtocolo { get; set; }
        public int NumeroVia { get; set; }
        public string Nome { get; set; }
        public string Rg { get; set; }
        public string Cpf { get; set; }
        public string Foto { get; set; } // Base64 string
        public string NomeMae { get; set; }
        public string NomePai { get; set; }
    }
}
