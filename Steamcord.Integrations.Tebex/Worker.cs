using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Steamcord.Integrations.Tebex;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IDiscordApiClient _discordClient;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ISteamcordApiClient _steamcordClient;

    public Worker(ILogger<Worker> logger, IDiscordApiClient discordClient,
        IServiceScopeFactory serviceScopeFactory, ISteamcordApiClient steamcordClient)
    {
        _logger = logger;
        _discordClient = discordClient;
        _serviceScopeFactory = serviceScopeFactory;
        _steamcordClient = steamcordClient;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            await using var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            
            var tasks = await context.Queue.ToListAsync(stoppingToken);
            foreach (var task in tasks)
            {
                if (await TryExecuteTask(task)) context.Queue.Remove(task);
            }

            await context.SaveChangesAsync(stoppingToken);
            await Task.Delay(1000, stoppingToken);
        }
    }

    private async Task<bool> TryExecuteTask(UpdateRoleTask task)
    {
        try
        {
            // this could be batched at some point, though not critical
            var discordIds = await _steamcordClient.GetDiscordIds(task.SteamId);
            foreach (var discordId in discordIds)
            {
                switch (task.Type)
                {
                    case TaskType.AddRole:
                        await _discordClient.AddRole(discordId, task.RoleId);
                        break;
                    case TaskType.RemoveRole:
                        await _discordClient.RemoveRole(discordId, task.RoleId);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                _logger.LogInformation("Completed task {Task}", JsonSerializer.Serialize(task));
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex.ToString());
            return false;
        }

        return true;
    }
}