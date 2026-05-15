using System.ComponentModel;
using ModelContextProtocol.Server;
using Gw2Mcp.Services;

namespace Gw2Mcp.Tools;

[McpServerToolType]
public class AccountTool
{
    private readonly Gw2ApiClient _client;
    private static readonly string[] Endpoints =
    [
        "account", "bank", "materials", "achievements", "dyes", "minis", "skins",
        "wallet", "titles", "raids", "masteries", "recipes", "finishers", "outfits",
        "luck", "wvw", "inventory", "buildstorage", "dailycrafting", "dungeons",
        "gliders", "emotes", "mailcarriers", "mapchests", "mastery/points",
        "mounts/skins", "mounts/types", "novelties", "pvp/heroes", "legendaryarmory",
        "worldbosses", "homestead/decorations", "homestead/glyphs",
        "wizardsvault/daily", "wizardsvault/weekly", "wizardsvault/special",
        "wizardsvault/listings", "home/cats", "home/nodes", "skiffs", "jadebots"
    ];

    public AccountTool(Gw2ApiClient client) => _client = client;

    [McpServerTool, Description("Access Guild Wars 2 account data. Returns account info, bank, materials, wallet, achievements, unlocked collections, and more.")]
    public async Task<string> Gw2Account(
        [Description($"What to retrieve: {nameof(Endpoints)}. Use 'account' for general account info, 'bank' for bank contents, 'wallet' for currencies, etc.")] string endpoint,
        [Description("GW2 API key. Falls back to GW2_API_KEY env var.")] string? apiKey = null,
        CancellationToken cancellationToken = default)
    {
        var normalized = endpoint.ToLowerInvariant().Trim('/');
        if (!Endpoints.Contains(normalized))
            return $"{{\"error\": \"Unknown endpoint '{endpoint}'. Valid options: {string.Join(", ", Endpoints)}\"}}";

        return await _client.GetAsync($"v2/account/{normalized}", apiKey, cancellationToken);
    }

    [McpServerTool, Description("Get information about the supplied API key — name, permissions, and ID.")]
    public async Task<string> Gw2TokenInfo(
        [Description("GW2 API key. Falls back to GW2_API_KEY env var.")] string? apiKey = null,
        CancellationToken cancellationToken = default)
    {
        return await _client.GetAsync("v2/tokeninfo", apiKey, cancellationToken);
    }
}