using System.Net.Http.Headers;
using System.Text.Json;

namespace Steamcord.Integrations.Tebex;

public class SteamcordApiClient : ISteamcordApiClient
{
    private readonly HttpClient _httpClient;

    public SteamcordApiClient(IConfiguration configuration, HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://api.steamcord.io/");
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", configuration["Steamcord:ApiToken"]);
    }

    public async Task<IEnumerable<string>> GetDiscordIds(string steamId)
    {
        var response = await _httpClient.GetAsync($"players?steamId={steamId}");

        response.EnsureSuccessStatusCode();

        return JsonSerializer
            .Deserialize<SteamcordPlayer[]>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            })!
            .SelectMany(player => player.DiscordAccounts)
            .Select(player => player.DiscordId);
    }

    private class DiscordAccount
    {
        public string DiscordId { get; set; }
    }

    private class SteamcordPlayer
    {
        public IEnumerable<DiscordAccount> DiscordAccounts { get; set; }
    }
}