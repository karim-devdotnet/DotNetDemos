using BackgroundServiceDemo;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<NotificationWorker>();
    })
    .Build();

//var app = host.Services.GetRequiredService<IHostApplicationLifetime>();
//app.ApplicationStarted.Register(() =>
//{
//    Console.WriteLine("Application started.");
//});

//app.ApplicationStopping.Register(() =>
//{
//    Console.WriteLine("Application stopping.");
//});

//app.ApplicationStopped.Register(() =>
//{
//    Console.WriteLine("Application stopped.");
//});

//app.Run();

await host.RunAsync();
