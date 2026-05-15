using System.ComponentModel;
using ModelContextProtocol.Server;
using Gw2Mcp.Services;

namespace Gw2Mcp.Tools;

[McpServerToolType]
public class WvwTool
{
    private readonly Gw2ApiClient _client;

    public WvwTool(Gw2ApiClient client) => _client = client;

    [McpServerTool, Description("Get WvW match data, objectives, abilities, ranks, upgrades, and more.")]
    public async Task<string> Gw2Wvw(
        [Description("What to retrieve: 'matches', 'abilities', 'objectives', 'ranks', 'upgrades', 'rewardtracks', 'timers'")] string endpoint,
        [Description("Comma-separated IDs. Omit to list all.")] string? ids = null,
        [Description("Language: en, de, es, fr, zh, ko")] string? lang = null,
        CancellationToken cancellationToken = default)
    {
        var normalized = endpoint.ToLowerInvariant().Trim('/');
        var apiPath = normalized switch
        {
            "matches" => "v2/wvw/matches",
            "abilities" => "v2/wvw/abilities",
            "objectives" => "v2/wvw/objectives",
            "ranks" => "v2/wvw/ranks",
            "upgrades" => "v2/wvw/upgrades",
            "rewardtracks" => "v2/wvw/rewardtracks",
            "timers" => "v2/wvw/timers",
            _ => null
        };

        if (apiPath is null)
            return $"{{\"error\": \"Unknown endpoint '{endpoint}'. Valid: matches, abilities, objectives, ranks, upgrades, rewardtracks, timers\"}}";

        return await _client.GetWithIdsAsync(apiPath, ids, lang, apiKey: null, cancellationToken: cancellationToken);
    }

    [McpServerTool, Description("Get WvW match details — overview, scores, or stats for a specific world.")]
    public async Task<string> Gw2WvwMatchDetail(
        [Description("What to retrieve: 'overview', 'scores', or 'stats'")] string detail,
        [Description("World ID, e.g. '1001' for match lookup")] string worldId,
        CancellationToken cancellationToken = default)
    {
        var normalized = detail.ToLowerInvariant().Trim('/');
        return normalized switch
        {
            "overview" => await _client.GetAsync($"v2/wvw/matches/overview?world_id={Uri.EscapeDataString(worldId)}", apiKey: null, cancellationToken: cancellationToken),
            "scores" => await _client.GetAsync($"v2/wvw/matches/scores?world_id={Uri.EscapeDataString(worldId)}", apiKey: null, cancellationToken: cancellationToken),
            "stats" => await _client.GetAsync($"v2/wvw/matches/stats?world_id={Uri.EscapeDataString(worldId)}", apiKey: null, cancellationToken: cancellationToken),
            _ => $"{{\"error\": \"Unknown detail '{detail}'. Valid: overview, scores, stats\"}}"
        };
    }

    [McpServerTool, Description("Get localized WvW objective names (v1 endpoint).")]
    public async Task<string> Gw2WvwObjectiveNames(
        [Description("Language: en, de, es, fr, zh, ko")] string? lang = null,
        CancellationToken cancellationToken = default)
    {
        var url = "v1/wvw/objective_names";
        if (!string.IsNullOrEmpty(lang)) url += $"?lang={Uri.EscapeDataString(lang)}";
        return await _client.GetAsync(url, apiKey: null, cancellationToken: cancellationToken);
    }
}