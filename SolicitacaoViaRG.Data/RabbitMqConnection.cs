using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
namespace SolicitacaoViaRG.Data;

public class RabbitMqConnection
{
    private readonly IConnection _connection;
    public IConnection Connection  => _connection; 
    public RabbitMqConnection(IConfiguration configuration)
    {
        var factory = new ConnectionFactory()
        {
            
            HostName = configuration["RabbitMq:HostName"],
            UserName = configuration["RabbitMq:UserName"],
            Password = configuration["RabbitMq:Password"],
            Port = 5672
        };

        _connection = factory.CreateConnection();
    }

}
