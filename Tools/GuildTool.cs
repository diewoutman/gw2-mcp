using System.ComponentModel;
using ModelContextProtocol.Server;
using Gw2Mcp.Services;

namespace Gw2Mcp.Tools;

[McpServerToolType]
public class GuildTool
{
    private readonly Gw2ApiClient _client;
    private static readonly string[] GuildEndpoints =
    [
        "details", "log", "members", "ranks", "stash", "storage", "treasury", "teams", "upgrades"
    ];

    public GuildTool(Gw2ApiClient client) => _client = client;

    [McpServerTool, Description("Search for guilds by name. Returns matching guild IDs.")]
    public async Task<string> Gw2GuildSearch(
        [Description("The guild name to search for")] string name,
        CancellationToken cancellationToken = default)
    {
        return await _client.GetAsync($"v2/guild/search?name={Uri.EscapeDataString(name)}", apiKey: null, cancellationToken: cancellationToken);
    }

    [McpServerTool, Description("Get guild information — details, log, members, stash, treasury, and more. Requires guild permissions on API key for most endpoints.")]
    public async Task<string> Gw2Guild(
        [Description("The guild ID")] string guildId,
        [Description($"What to retrieve: {nameof(GuildEndpoints)}")] string endpoint = "details",
        [Description("GW2 API key with guild permissions")] string? apiKey = null,
        CancellationToken cancellationToken = default)
    {
        var normalized = endpoint.ToLowerInvariant().Trim('/');
        var encodedId = Uri.EscapeDataString(guildId);

        if (normalized == "details")
            return await _client.GetAsync($"v2/guild/{encodedId}", apiKey, cancellationToken);

        if (!GuildEndpoints.Contains(normalized))
            return $"{{\"error\": \"Unknown endpoint '{endpoint}'. Valid: {string.Join(", ", GuildEndpoints)}\"}}";

        return await _client.GetAsync($"v2/guild/{encodedId}/{normalized}", apiKey, cancellationToken);
    }

    [McpServerTool, Description("Get guild permission or upgrade details.")]
    public async Task<string> Gw2GuildData(
        [Description("What to retrieve: 'permissions' or 'upgrades'")] string endpoint,
        [Description("Comma-separated IDs. Omit to list all.")] string? ids = null,
        [Description("Language: en, de, es, fr, zh, ko")] string? lang = null,
        CancellationToken cancellationToken = default)
    {
        var normalized = endpoint.ToLowerInvariant().Trim('/');
        return normalized switch
        {
            "permissions" => await _client.GetWithIdsAsync("v2/guild/permissions", ids, lang, apiKey: null, cancellationToken: cancellationToken),
            "upgrades" => await _client.GetWithIdsAsync("v2/guild/upgrades", ids, lang, apiKey: null, cancellationToken: cancellationToken),
            _ => $"{{\"error\": \"Unknown endpoint '{endpoint}'. Valid: permissions, upgrades\"}}"
        };
    }
}