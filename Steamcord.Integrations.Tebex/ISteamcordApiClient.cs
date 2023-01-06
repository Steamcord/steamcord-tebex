namespace Steamcord.Integrations.Tebex;

public interface ISteamcordApiClient
{
    Task<IEnumerable<string>> GetDiscordIds(string steamId);
}