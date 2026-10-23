using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BackgroundServiceDemo;

internal class NotificationWorker : BackgroundService
{
    private readonly ILogger<NotificationWorker> _logger;

    public NotificationWorker(
        ILogger<NotificationWorker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation(
                "Background worker is running at {Time}",
                DateTimeOffset.UtcNow);

            await Task.Delay(
                TimeSpan.FromSeconds(30),
                stoppingToken);
        }
    }
}
