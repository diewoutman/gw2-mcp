using System.ComponentModel;
using ModelContextProtocol.Server;

namespace Gw2Mcp.Tools;

[McpServerToolType]
public class MapTools
{
    private readonly Gw2ApiClient _client;

    public MapTools(Gw2ApiClient client)
    {
        _client = client;
    }

    [McpServerTool, Description("Get continent information including name, dimensions, and available floors.")]
    public async Task<string> GetContinents(
        [Description("Language for localized names: en, de, es, fr, zh, ko")] string? lang = null,
        CancellationToken cancellationToken = default)
    {
        return await _client.GetWithIdsAsync("v2/continents", lang: lang, apiKey: null, cancellationToken: cancellationToken);
    }

    [McpServerTool, Description("Get map details including name, level range, floors, region, and continent info.")]
    public async Task<string> GetMaps(
        [Description("Comma-separated map IDs. Omit to get all available IDs.")] string? ids = null,
        [Description("Page number for pagination (0-based)")] int? page = null,
        [Description("Language for localized names: en, de, es, fr, zh, ko")] string? lang = null,
        CancellationToken cancellationToken = default)
    {
        if (page.HasValue)
            return await _client.GetPaginatedWithIdsAsync("v2/maps", page.Value, ids: ids, lang: lang, apiKey: null, cancellationToken: cancellationToken);

        return await _client.GetWithIdsAsync("v2/maps", ids, lang, apiKey: null, cancellationToken: cancellationToken);
    }
}