using Command;
using Command.Context;
using Command.Services;
using Google.Protobuf.WellKnownTypes;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);//Host.CreateApplicationBuilder(args);
var config = builder.Configuration;

Console.WriteLine(config.GetConnectionString("default"));

builder.Services.AddDbContext<CommandContext>(opt => {
    opt.UseSqlite(config.GetConnectionString("default")).ConfigureWarnings(w => w.Ignore(RelationalEventId.CommandExecuted));
    opt.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

builder.Services.AddMemoryCache();
builder.Services.AddMassTransit(x =>
{
    x.AddConsumers(typeof(Program).Assembly);
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
        cfg.ConfigureEndpoints(context);
    });
});
builder.Services.AddHostedService<Worker>();
builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<PriceService>();
app.MapGrpcService<CollectionService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
