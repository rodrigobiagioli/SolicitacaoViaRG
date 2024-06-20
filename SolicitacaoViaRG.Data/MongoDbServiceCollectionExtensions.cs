using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using SolicitacaoViaRG.Data;

public static class MongoDbServiceCollectionExtensions
{
    public static IServiceCollection AddMongoDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("MongoDb");
        var databaseName = configuration["MongoDbDatabaseName"];

        services.AddSingleton(sp => new AppDbContext(connectionString, databaseName));
        return services;
    }
}
