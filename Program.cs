using Asp.Versioning;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using PBCartaoCredito.Consumers;
using PBCartaoCredito.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("PBCartoesDb"));

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<PropostaGeradaConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ReceiveEndpoint("fila-proposta-gerada", e =>
        {
            e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(2)));
            e.ConfigureConsumer<PropostaGeradaConsumer>(context);
        });
    });
});

var app = builder.Build();


var versionSet = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1))
    .Build();

var cartoesGroup = app.MapGroup("/api/v{version:apiVersion}/cartoes")
    .WithApiVersionSet(versionSet);

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

cartoesGroup.MapGet("/{clienteId}", async (Guid clienteId, AppDbContext db) => 
{
    var cartoes = await db.Cartoes.Where(c => c.ClienteId == clienteId).ToListAsync();
    
    if (!cartoes.Any())
        return Results.NotFound(new { message = "Nenhum cartão encontrado para este cliente." });
        
    return Results.Ok(cartoes);
});

app.Run();