using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using SolicitacaoViaRG.BLL;
using SolicitacaoViaRG.Data;
using SolicitacaoViaRG.Data.Repository;
using SolicitacaoViaRG.Data.Repository.Interface;
using SolicitacaoViaRG.Helper;
using System.Collections.Generic;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath, true);

    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "ApiKey",
        Type = SecuritySchemeType.ApiKey,
        Description = "API Key needed to access the endpoints. ApiKey: MySuperSecretApiKey12345!"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                }
            },
            new string[] { }
        }
    });
});

// Add custom services
var configuration = builder.Configuration;
var imageDirectory = configuration["ImageDirectory"];

builder.Services.AddMongoDbContext(configuration);
builder.Services.AddSingleton<IProtocoloRepository, ProtocoloRepository>();
builder.Services.AddSingleton(sp =>
{
    var logger = sp.GetRequiredService<ILogger<ProtocoloBLL>>();
    return new ProtocoloBLL(sp.GetRequiredService<IProtocoloRepository>(), imageDirectory, logger);
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
