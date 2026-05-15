using System.ComponentModel;
using ModelContextProtocol.Server;

namespace Gw2Mcp.Tools;

[McpServerToolType]
public class CharacterTools
{
    private readonly Gw2ApiClient _client;

    public CharacterTools(Gw2ApiClient client)
    {
        _client = client;
    }

    [McpServerTool, Description("Get the list of character names on the authenticated account.")]
    public async Task<string> GetCharacters(
        [Description("GW2 API key. If not provided, uses GW2_API_KEY environment variable")] string? apiKey = null,
        CancellationToken cancellationToken = default)
    {
        return await _client.GetAsync("v2/characters", apiKey, cancellationToken);
    }

    [McpServerTool, Description("Get detailed information about a specific character including race, gender, profession, level, equipment, and bags.")]
    public async Task<string> GetCharacter(
        [Description("The character name to look up")] string name,
        [Description("GW2 API key. If not provided, uses GW2_API_KEY environment variable")] string? apiKey = null,
        CancellationToken cancellationToken = default)
    {
        var encodedName = Uri.EscapeDataString(name);
        return await _client.GetAsync($"v2/characters/{encodedName}", apiKey, cancellationToken);
    }
}