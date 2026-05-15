using System.ComponentModel;
using ModelContextProtocol.Server;
using Gw2Mcp.Services;

namespace Gw2Mcp.Tools;

[McpServerToolType]
public class HomesteadTool
{
    private readonly Gw2ApiClient _client;

    public HomesteadTool(Gw2ApiClient client) => _client = client;

    [McpServerTool, Description("Get homestead and home instance data — decorations, decoration categories, glyphs, cats, and nodes.")]
    public async Task<string> Gw2Homestead(
        [Description("What to retrieve: 'decorations', 'categories', 'glyphs', 'cats', or 'nodes'")] string endpoint,
        [Description("Comma-separated IDs. Omit to list all.")] string? ids = null,
        [Description("Page number for pagination (0-based)")] int? page = null,
        [Description("Language: en, de, es, fr, zh, ko")] string? lang = null,
        CancellationToken cancellationToken = default)
    {
        var normalized = endpoint.ToLowerInvariant().Trim('/');
        var apiPath = normalized switch
        {
            "decorations" => "v2/homestead/decorations",
            "categories" => "v2/homestead/decorations/categories",
            "glyphs" => "v2/homestead/glyphs",
            "cats" => "v2/home/cats",
            "nodes" => "v2/home/nodes",
            _ => null
        };

        if (apiPath is null)
            return $"{{\"error\": \"Unknown endpoint '{endpoint}'. Valid: decorations, categories, glyphs, cats, nodes\"}}";

        if (page.HasValue && normalized is "decorations")
            return await _client.GetPaginatedWithIdsAsync(apiPath, page.Value, ids: ids, lang: lang, apiKey: null, cancellationToken: cancellationToken);

        return await _client.GetWithIdsAsync(apiPath, ids, lang, apiKey: null, cancellationToken: cancellationToken);
    }
}