using System.ComponentModel;
using ModelContextProtocol.Server;
using Gw2Mcp.Services;

namespace Gw2Mcp.Tools;

[McpServerToolType]
public class ItemTool
{
    private readonly Gw2ApiClient _client;

    public ItemTool(Gw2ApiClient client) => _client = client;

    [McpServerTool, Description("Get item, skin, material category, or item stat details.")]
    public async Task<string> Gw2Item(
        [Description("What to retrieve: 'items', 'skins', 'materials', or 'itemstats'")] string endpoint,
        [Description("Comma-separated IDs, e.g. '19683,19709'. Omit to list all IDs.")] string? ids = null,
        [Description("Page number for pagination (0-based)")] int? page = null,
        [Description("Language: en, de, es, fr, zh, ko")] string? lang = null,
        CancellationToken cancellationToken = default)
    {
        var normalized = endpoint.ToLowerInvariant().Trim('/');
        var apiPath = normalized switch
        {
            "items" => "v2/items",
            "skins" => "v2/skins",
            "materials" => "v2/materials",
            "itemstats" => "v2/itemstats",
            _ => null
        };

        if (apiPath is null)
            return $"{{\"error\": \"Unknown endpoint '{endpoint}'. Valid: items, skins, materials, itemstats\"}}";

        if (page.HasValue)
            return await _client.GetPaginatedWithIdsAsync(apiPath, page.Value, ids: ids, lang: lang, apiKey: null, cancellationToken: cancellationToken);

        return await _client.GetWithIdsAsync(apiPath, ids, lang, apiKey: null, cancellationToken: cancellationToken);
    }

    [McpServerTool, Description("Get recipe details or search recipes by ingredient or output.")]
    public async Task<string> Gw2Recipe(
        [Description("What to retrieve: 'recipes' for details, 'search' to find by input/output item ID")] string endpoint,
        [Description("Comma-separated recipe IDs (for 'recipes'), or item ID (for 'search')")] string? ids = null,
        [Description("For search: item ID used as ingredient")] int? input = null,
        [Description("For search: item ID of crafted output")] int? output = null,
        [Description("Page number for pagination (0-based)")] int? page = null,
        [Description("Language: en, de, es, fr, zh, ko")] string? lang = null,
        CancellationToken cancellationToken = default)
    {
        var normalized = endpoint.ToLowerInvariant().Trim('/');

        if (normalized == "search")
        {
            if (input.HasValue)
                return await _client.GetAsync($"v2/recipes/search?input={input.Value}", apiKey: null, cancellationToken: cancellationToken);
            if (output.HasValue)
                return await _client.GetAsync($"v2/recipes/search?output={output.Value}", apiKey: null, cancellationToken: cancellationToken);
            return "{\"error\": \"Provide 'input' or 'output' parameter to search recipes.\"}";
        }

        if (normalized == "recipes")
        {
            if (page.HasValue)
                return await _client.GetPaginatedWithIdsAsync("v2/recipes", page.Value, ids: ids, lang: lang, apiKey: null, cancellationToken: cancellationToken);

            return await _client.GetWithIdsAsync("v2/recipes", ids, lang, apiKey: null, cancellationToken: cancellationToken);
        }

        return $"{{\"error\": \"Unknown endpoint '{endpoint}'. Valid: recipes, search\"}}";
    }
}