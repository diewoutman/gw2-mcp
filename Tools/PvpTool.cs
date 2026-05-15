using System.ComponentModel;
using ModelContextProtocol.Server;
using Gw2Mcp.Services;

namespace Gw2Mcp.Tools;

[McpServerToolType]
public class PvpTool
{
    private readonly Gw2ApiClient _client;
    private static readonly string[] PublicEndpoints = ["amulets", "heroes", "ranks", "rewardtracks", "runes", "sigils", "seasons"];
    private static readonly string[] AuthEndpoints = ["games", "standings", "stats"];

    public PvpTool(Gw2ApiClient client) => _client = client;

    [McpServerTool, Description("Get PvP game data — amulets, heroes, ranks, reward tracks, runes, sigils, seasons.")]
    public async Task<string> Gw2Pvp(
        [Description($"What to retrieve: {nameof(PublicEndpoints)}")] string endpoint,
        [Description("Comma-separated IDs. Omit to list all.")] string? ids = null,
        [Description("Language: en, de, es, fr, zh, ko")] string? lang = null,
        CancellationToken cancellationToken = default)
    {
        var normalized = endpoint.ToLowerInvariant().Trim('/');
        var apiPath = normalized switch
        {
            "amulets" => "v2/pvp/amulets",
            "heroes" => "v2/pvp/heroes",
            "ranks" => "v2/pvp/ranks",
            "rewardtracks" => "v2/pvp/rewardtracks",
            "runes" => "v2/pvp/runes",
            "sigils" => "v2/pvp/sigils",
            "seasons" => "v2/pvp/seasons",
            _ => null
        };

        if (apiPath is null)
            return $"{{\"error\": \"Unknown endpoint '{endpoint}'. Valid: {string.Join(", ", PublicEndpoints)}\"}}";

        return await _client.GetWithIdsAsync(apiPath, ids, lang, apiKey: null, cancellationToken: cancellationToken);
    }

    [McpServerTool, Description("Get authenticated PvP data — game history, standings, or stats. Requires API key.")]
    public async Task<string> Gw2PvpAccount(
        [Description("What to retrieve: 'games', 'standings', or 'stats'")] string endpoint,
        [Description("GW2 API key with pvp permission.")] string? apiKey = null,
        CancellationToken cancellationToken = default)
    {
        var normalized = endpoint.ToLowerInvariant().Trim('/');
        return normalized switch
        {
            "games" => await _client.GetAsync("v2/pvp/games", apiKey, cancellationToken),
            "standings" => await _client.GetAsync("v2/pvp/standings", apiKey, cancellationToken),
            "stats" => await _client.GetAsync("v2/pvp/stats", apiKey, cancellationToken),
            _ => $"{{\"error\": \"Unknown endpoint '{endpoint}'. Valid: {string.Join(", ", AuthEndpoints)}\"}}"
        };
    }
}