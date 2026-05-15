using System.ComponentModel;
using ModelContextProtocol.Server;

namespace Gw2Mcp.Tools;

[McpServerToolType]
public class ItemTools
{
    private readonly Gw2ApiClient _client;

    public ItemTools(Gw2ApiClient client)
    {
        _client = client;
    }

    [McpServerTool, Description("Get item details such as name, icon, description, type, rarity, level, vendor value, and flags.")]
    public async Task<string> GetItems(
        [Description("Comma-separated item IDs, e.g. '19683,19709'. Omit to get all available IDs.")] string? ids = null,
        [Description("Page number for pagination (0-based)")] int? page = null,
        [Description("Language for localized names: en, de, es, fr, zh, ko")] string? lang = null,
        CancellationToken cancellationToken = default)
    {
        if (page.HasValue)
            return await _client.GetPaginatedWithIdsAsync("v2/items", page.Value, ids: ids, lang: lang, apiKey: null, cancellationToken: cancellationToken);

        return await _client.GetWithIdsAsync("v2/items", ids, lang, apiKey: null, cancellationToken: cancellationToken);
    }

    [McpServerTool, Description("Get material category names and item IDs for each material storage tab.")]
    public async Task<string> GetMaterials(
        [Description("Comma-separated material category IDs. Omit to get all categories.")] string? ids = null,
        [Description("Language for localized names: en, de, es, fr, zh, ko")] string? lang = null,
        CancellationToken cancellationToken = default)
    {
        return await _client.GetWithIdsAsync("v2/materials", ids, lang, apiKey: null, cancellationToken: cancellationToken);
    }

    [McpServerTool, Description("Get recipe details including type, output item, crafting time, disciplines, min rating, and ingredients.")]
    public async Task<string> GetRecipes(
        [Description("Comma-separated recipe IDs. Omit to get all available IDs.")] string? ids = null,
        [Description("Page number for pagination (0-based)")] int? page = null,
        [Description("Language for localized names: en, de, es, fr, zh, ko")] string? lang = null,
        CancellationToken cancellationToken = default)
    {
        if (page.HasValue)
            return await _client.GetPaginatedWithIdsAsync("v2/recipes", page.Value, ids: ids, lang: lang, apiKey: null, cancellationToken: cancellationToken);

        return await _client.GetWithIdsAsync("v2/recipes", ids, lang, apiKey: null, cancellationToken: cancellationToken);
    }

    [McpServerTool, Description("Search for recipes by input ingredient or output item. Provide either 'input' or 'output', not both.")]
    public async Task<string> SearchRecipes(
        [Description("Item ID to search for recipes that use this item as an ingredient")] int? input = null,
        [Description("Item ID to search for recipes that craft this item")] int? output = null,
        CancellationToken cancellationToken = default)
    {
        if (input.HasValue)
            return await _client.GetAsync($"v2/recipes/search?input={input.Value}", apiKey: null, cancellationToken: cancellationToken);
        if (output.HasValue)
            return await _client.GetAsync($"v2/recipes/search?output={output.Value}", apiKey: null, cancellationToken: cancellationToken);

        return "{\"error\": \"Provide either 'input' or 'output' parameter to search recipes.\"}";
    }

    [McpServerTool, Description("Get skin details including name, type, flags, restrictions, and icon URL.")]
    public async Task<string> GetSkins(
        [Description("Comma-separated skin IDs. Omit to get all available IDs.")] string? ids = null,
        [Description("Page number for pagination (0-based)")] int? page = null,
        [Description("Language for localized names: en, de, es, fr, zh, ko")] string? lang = null,
        CancellationToken cancellationToken = default)
    {
        if (page.HasValue)
            return await _client.GetPaginatedWithIdsAsync("v2/skins", page.Value, ids: ids, lang: lang, apiKey: null, cancellationToken: cancellationToken);

        return await _client.GetWithIdsAsync("v2/skins", ids, lang, apiKey: null, cancellationToken: cancellationToken);
    }
}