using MassTransit;
using Worker.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumers(typeof(Program).Assembly);
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("192.168.129.8", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
        cfg.ConfigureEndpoints(context);
        cfg.UseConcurrencyLimit(6);
    });
});
builder.Services.AddSingleton<GrpcService>();

builder.Services.AddHostedService<Worker.Worker>();

var host = builder.Build();
host.Run();
