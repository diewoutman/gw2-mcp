using System.ComponentModel;
using ModelContextProtocol.Server;
using Gw2Mcp.Services;

namespace Gw2Mcp.Tools;

[McpServerToolType]
public class CosmeticTool
{
    private readonly Gw2ApiClient _client;
    private static readonly string[] Endpoints =
    [
        "minis", "outfits", "gliders", "finishers", "novelties", "emotes",
        "mailcarriers", "jadebots", "skiffs", "mounts/skins", "mounts/types"
    ];

    public CosmeticTool(Gw2ApiClient client) => _client = client;

    [McpServerTool, Description("Get cosmetic item details — minis, outfits, gliders, finishers, novelties, emotes, mail carriers, jade bots, skiffs, mount skins/types.")]
    public async Task<string> Gw2Cosmetic(
        [Description($"What to retrieve: {nameof(Endpoints)}")] string endpoint,
        [Description("Comma-separated IDs. Omit to list all.")] string? ids = null,
        [Description("Page number for pagination (0-based)")] int? page = null,
        [Description("Language: en, de, es, fr, zh, ko")] string? lang = null,
        CancellationToken cancellationToken = default)
    {
        var normalized = endpoint.ToLowerInvariant().Trim('/');
        var apiPath = normalized switch
        {
            "minis" => "v2/minis",
            "outfits" => "v2/outfits",
            "gliders" => "v2/gliders",
            "finishers" => "v2/finishers",
            "novelties" => "v2/novelties",
            "emotes" => "v2/emotes",
            "mailcarriers" => "v2/mailcarriers",
            "jadebots" => "v2/jadebots",
            "skiffs" => "v2/skiffs",
            "mounts/skins" => "v2/mounts/skins",
            "mounts/types" => "v2/mounts/types",
            _ => null
        };

        if (apiPath is null)
            return $"{{\"error\": \"Unknown endpoint '{endpoint}'. Valid: {string.Join(", ", Endpoints)}\"}}";

        if (normalized == "minis" && page.HasValue)
            return await _client.GetPaginatedWithIdsAsync(apiPath, page.Value, ids: ids, lang: lang, apiKey: null, cancellationToken: cancellationToken);

        return await _client.GetWithIdsAsync(apiPath, ids, lang, apiKey: null, cancellationToken: cancellationToken);
    }
}