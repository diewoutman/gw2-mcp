using System.ComponentModel;
using ModelContextProtocol.Server;

namespace Gw2Mcp.Tools;

[McpServerToolType]
public class WvwTools
{
    private readonly Gw2ApiClient _client;

    public WvwTools(Gw2ApiClient client)
    {
        _client = client;
    }

    [McpServerTool, Description("Get all currently running WvW matches with participating world IDs and timestamps.")]
    public async Task<string> GetMatches(
        CancellationToken cancellationToken = default)
    {
        return await _client.GetAsync("v2/wvw/matches", apiKey: null, cancellationToken: cancellationToken);
    }

    [McpServerTool, Description("Get detailed information about a specific WvW match including scores and map details.")]
    public async Task<string> GetMatchDetails(
        [Description("The WvW match ID to query, e.g. '1-1'")] string matchId,
        CancellationToken cancellationToken = default)
    {
        var encodedId = Uri.EscapeDataString(matchId);
        return await _client.GetAsync($"v1/wvw/match_details?match_id={encodedId}", apiKey: null, cancellationToken: cancellationToken);
    }

    [McpServerTool, Description("Get localized WvW objective names.")]
    public async Task<string> GetObjectiveNames(
        [Description("Language for localized names: en, de, es, fr, zh, ko")] string? lang = null,
        CancellationToken cancellationToken = default)
    {
        var url = "v1/wvw/objective_names";
        if (!string.IsNullOrEmpty(lang))
            url += $"?lang={Uri.EscapeDataString(lang)}";

        return await _client.GetAsync(url, apiKey: null, cancellationToken: cancellationToken);
    }
}