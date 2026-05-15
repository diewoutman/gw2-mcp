using System.ComponentModel;
using ModelContextProtocol.Server;
using Gw2Mcp.Services;

namespace Gw2Mcp.Tools;

[McpServerToolType]
public class MiscTool
{
    private readonly Gw2ApiClient _client;

    public MiscTool(Gw2ApiClient client) => _client = client;

    [McpServerTool, Description("Get the current Guild Wars 2 build ID.")]
    public async Task<string> Gw2Build(CancellationToken cancellationToken = default)
        => await _client.GetAsync("v2/build", apiKey: null, cancellationToken: cancellationToken);

    [McpServerTool, Description("Get miscellaneous game data: colors, files, quaggans, worlds, or create a temporary API subtoken.")]
    public async Task<string> Gw2Misc(
        [Description("What to retrieve: 'colors', 'files', 'quaggans', 'worlds', or 'subtoken'")] string endpoint,
        [Description("Comma-separated IDs. Omit to list all.")] string? ids = null,
        [Description("Page number for pagination (0-based)")] int? page = null,
        [Description("Language: en, de, es, fr, zh, ko")] string? lang = null,
        [Description("GW2 API key (required for subtoken). Falls back to GW2_API_KEY env var.")] string? apiKey = null,
        CancellationToken cancellationToken = default)
    {
        var normalized = endpoint.ToLowerInvariant().Trim('/');

        if (normalized == "subtoken")
            return await _client.GetAsync("v2/createsubtoken", apiKey, cancellationToken);

        var apiPath = normalized switch
        {
            "colors" => "v2/colors",
            "files" => "v2/files",
            "quaggans" => "v2/quaggans",
            "worlds" => "v2/worlds",
            _ => null
        };

        if (apiPath is null)
            return $"{{\"error\": \"Unknown endpoint '{endpoint}'. Valid: colors, files, quaggans, worlds, subtoken\"}}";

        if (page.HasValue && normalized is "colors" or "files" or "worlds")
            return await _client.GetPaginatedWithIdsAsync(apiPath, page.Value, ids: ids, lang: lang, apiKey: null, cancellationToken: cancellationToken);

        return await _client.GetWithIdsAsync(apiPath, ids, lang, apiKey: null, cancellationToken: cancellationToken);
    }
}