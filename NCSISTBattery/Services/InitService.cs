
namespace NCSISTBattery.Services
{
    public class InitService : BackgroundService
    {
        private readonly IServiceScopeFactory scopeFactory;
        public InitService(IServiceScopeFactory scopeFactory)
        {
            this.scopeFactory = scopeFactory;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var machineService = scope.ServiceProvider.GetRequiredService<DataService>();
                await machineService.InitAllJigs();
            }
        }
    }
}
