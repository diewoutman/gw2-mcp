using System.ComponentModel;
using ModelContextProtocol.Server;

namespace Gw2Mcp.Tools;

[McpServerToolType]
public class MiscTools
{
    private readonly Gw2ApiClient _client;

    public MiscTools(Gw2ApiClient client)
    {
        _client = client;
    }

    [McpServerTool, Description("Get the current Guild Wars 2 build ID. Useful for detecting server restarts.")]
    public async Task<string> GetBuild(
        CancellationToken cancellationToken = default)
    {
        return await _client.GetAsync("v2/build", apiKey: null, cancellationToken: cancellationToken);
    }

    [McpServerTool, Description("Get dye color information including names and RGB color components.")]
    public async Task<string> GetColors(
        [Description("Comma-separated color IDs. Omit to get all colors.")] string? ids = null,
        [Description("Page number for pagination (0-based)")] int? page = null,
        [Description("Language for localized names: en, de, es, fr, zh, ko")] string? lang = null,
        CancellationToken cancellationToken = default)
    {
        if (page.HasValue)
            return await _client.GetPaginatedWithIdsAsync("v2/colors", page.Value, ids: ids, lang: lang, apiKey: null, cancellationToken: cancellationToken);

        return await _client.GetWithIdsAsync("v2/colors", ids, lang, apiKey: null, cancellationToken: cancellationToken);
    }

    [McpServerTool, Description("Get commonly requested in-game asset file URLs (icons, maps, etc.).")]
    public async Task<string> GetFiles(
        [Description("Comma-separated file identifiers. Omit to get all files.")] string? ids = null,
        [Description("Page number for pagination (0-based)")] int? page = null,
        CancellationToken cancellationToken = default)
    {
        if (page.HasValue)
            return await _client.GetPaginatedWithIdsAsync("v2/files", page.Value, ids: ids, apiKey: null, cancellationToken: cancellationToken);

        return await _client.GetWithIdsAsync("v2/files", ids, apiKey: null, cancellationToken: cancellationToken);
    }

    [McpServerTool, Description("Get quaggan image URLs. Pass an identifier to get a specific quaggan, or omit to list all available identifiers.")]
    public async Task<string> GetQuaggans(
        [Description("Comma-separated quaggan identifiers, e.g. 'box,choo'. Omit to list all available.")] string? ids = null,
        CancellationToken cancellationToken = default)
    {
        return await _client.GetWithIdsAsync("v2/quaggans", ids, apiKey: null, cancellationToken: cancellationToken);
    }

    [McpServerTool, Description("Get world/server information including name and population status.")]
    public async Task<string> GetWorlds(
        [Description("Comma-separated world IDs. Omit to get all worlds.")] string? ids = null,
        [Description("Page number for pagination (0-based)")] int? page = null,
        [Description("Language for localized names: en, de, es, fr, zh, ko")] string? lang = null,
        CancellationToken cancellationToken = default)
    {
        if (page.HasValue)
            return await _client.GetPaginatedWithIdsAsync("v2/worlds", page.Value, ids: ids, lang: lang, apiKey: null, cancellationToken: cancellationToken);

        return await _client.GetWithIdsAsync("v2/worlds", ids, lang, apiKey: null, cancellationToken: cancellationToken);
    }
}