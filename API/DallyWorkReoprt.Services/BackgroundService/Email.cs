using Microsoft.Extensions.DependencyInjection;


namespace DallyWorkReoprt.Services.BackgroundService
{
    public class Email : Microsoft.Extensions.Hosting.BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public Email(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await ProcessPendingEmails();
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }

        private async Task ProcessPendingEmails()
        {
            // Activation module has been removed.
            // This background service can be repurposed for future email workflows.
            await Task.CompletedTask;
        }
    }
}

