namespace Steamcord.Integrations.Tebex;

public interface IDiscordApiClient
{
    Task AddRole(string discordId, string roleId);
    Task RemoveRole(string discordId, string roleId);
}