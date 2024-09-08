using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SnzDiscordBot;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
    })
    // Configure as a Windows Service
    .UseWindowsService(options =>
    {
        options.ServiceName = "Senezh Bot";
    })
    .Build();

host.Run();