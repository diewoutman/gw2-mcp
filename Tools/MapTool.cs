using System.ComponentModel;
using ModelContextProtocol.Server;
using Gw2Mcp.Services;

namespace Gw2Mcp.Tools;

[McpServerToolType]
public class MapTool
{
    private readonly Gw2ApiClient _client;

    public MapTool(Gw2ApiClient client) => _client = client;

    [McpServerTool, Description("Get continent or map details.")]
    public async Task<string> Gw2Map(
        [Description("What to retrieve: 'continents' or 'maps'")] string endpoint,
        [Description("Comma-separated IDs. Omit to list all.")] string? ids = null,
        [Description("Page number for pagination (0-based)")] int? page = null,
        [Description("Language: en, de, es, fr, zh, ko")] string? lang = null,
        CancellationToken cancellationToken = default)
    {
        var normalized = endpoint.ToLowerInvariant().Trim('/');
        var apiPath = normalized switch
        {
            "continents" => "v2/continents",
            "maps" => "v2/maps",
            _ => null
        };

        if (apiPath is null)
            return $"{{\"error\": \"Unknown endpoint '{endpoint}'. Valid: continents, maps\"}}";

        if (page.HasValue)
            return await _client.GetPaginatedWithIdsAsync(apiPath, page.Value, ids: ids, lang: lang, apiKey: null, cancellationToken: cancellationToken);

        return await _client.GetWithIdsAsync(apiPath, ids, lang, apiKey: null, cancellationToken: cancellationToken);
    }
}