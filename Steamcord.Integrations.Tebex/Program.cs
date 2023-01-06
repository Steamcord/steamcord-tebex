using System.Collections.Immutable;
using Microsoft.EntityFrameworkCore;
using Steamcord.Integrations.Tebex;

var host = Host
    .CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        var connectionString = hostContext.Configuration.GetConnectionString("MySql");

        services
            .AddDbContext<AppDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)))
            .AddSingleton<ISteamcordApiClient, SteamcordApiClient>()
            .AddHostedService<Worker>();

        services.AddHttpClient<ISteamcordApiClient, SteamcordApiClient>();
        services.AddHttpClient<IDiscordApiClient, DiscordApiClient>();
    })
    .Build();

await using var context = host.Services.GetRequiredService<AppDbContext>();
await context.Database.MigrateAsync();

await host.RunAsync();