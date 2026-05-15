using System.ComponentModel;
using ModelContextProtocol.Server;
using Gw2Mcp.Services;

namespace Gw2Mcp.Tools;

[McpServerToolType]
public class CharacterTool
{
    private readonly Gw2ApiClient _client;
    private static readonly string[] Endpoints =
    [
        "backstory", "buildtabs", "buildtabs/active", "core", "crafting",
        "equipment", "equipmenttabs", "equipmenttabs/active", "heropoints",
        "inventory", "quests", "recipes", "skills", "specializations", "training"
    ];

    public CharacterTool(Gw2ApiClient client) => _client = client;

    [McpServerTool, Description("Get the list of character names on the authenticated account.")]
    public async Task<string> Gw2Characters(
        [Description("GW2 API key. Falls back to GW2_API_KEY env var.")] string? apiKey = null,
        CancellationToken cancellationToken = default)
    {
        return await _client.GetAsync("v2/characters", apiKey, cancellationToken);
    }

    [McpServerTool, Description("Get character details — backstory, build tabs, crafting, equipment, inventory, skills, specializations, training, and more.")]
    public async Task<string> Gw2Character(
        [Description("The character name")] string name,
        [Description($"What to retrieve: {nameof(Endpoints)}, or omit for full character details")] string? endpoint = null,
        [Description("GW2 API key")] string? apiKey = null,
        CancellationToken cancellationToken = default)
    {
        var encodedName = Uri.EscapeDataString(name);
        if (endpoint is null || endpoint.Equals("full", StringComparison.OrdinalIgnoreCase))
            return await _client.GetAsync($"v2/characters/{encodedName}", apiKey, cancellationToken);

        var normalized = endpoint.ToLowerInvariant().Trim('/');
        if (!Endpoints.Contains(normalized))
            return $"{{\"error\": \"Unknown endpoint '{endpoint}'. Valid options: full, {string.Join(", ", Endpoints)}\"}}";

        return await _client.GetAsync($"v2/characters/{encodedName}/{normalized}", apiKey, cancellationToken);
    }
}