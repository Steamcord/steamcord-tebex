using System.Net.Http.Headers;

namespace Steamcord.Integrations.Tebex;

public class DiscordApiClient : IDiscordApiClient
{
    private readonly HttpClient _httpClient;

    public DiscordApiClient(IConfiguration configuration, HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri($"https://discord.com/api/guilds/{configuration["Discord:GuildId"]}/");
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bot", configuration["Discord:BotToken"]);
    }

    public async Task AddRole(string discordId, string roleId)
    {
        var response = await _httpClient.PutAsync($"members/{discordId}/roles/{roleId}", null);

        response.EnsureSuccessStatusCode();
    }

    public async Task RemoveRole(string discordId, string roleId)
    {
        var response = await _httpClient.DeleteAsync($"members/{discordId}/roles/{roleId}");

        response.EnsureSuccessStatusCode();
    }
}